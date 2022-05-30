using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private PlayerInfo[] playerInfos = new PlayerInfo[2];
		[SerializeField] private AnimatorOverrideController redController;
		[SerializeField] private float waitTime = 2;
		[SerializeField] private GameObject AIPlayerPrefab;
		[SerializeField] private GameObject pressStartCanvas;
		[SerializeField] private GameObject pauseCanvas;
		[SerializeField] private TargetScene chooseTargetScene;
		#endregion

		#region Private Fields

		private readonly List<GameObject> _players = new List<GameObject>();
		private readonly Dictionary<GameObject, bool> _playerReadyStatus = new Dictionary<GameObject, bool>();
		private int _numPlayers = 0;
		private bool _gameStarted = false;
		private bool _AIPlaying;
		private static readonly int PlayerAnimatorX = Animator.StringToHash("X Direction");
		private Dictionary<int, Action> _scenesEvents = new Dictionary<int, Action>();

		private float _prevTimeScale;
		private bool _paused;
		private PlayerInput _pausedBy;
		private readonly Dictionary<PlayerInput, string> _playerInputs = new Dictionary<PlayerInput, string>();
		
		private List<string> _sceneNames = new List<string>() {"Original Arena","Icy Arena", "Volcanic Arena"};
		private enum TargetScene
		{
			OriginalArena = 0,
			IcyArena = 1,
			VolcanicArena = 2
		}

		#endregion

		#region Public Properties

		public static IReadOnlyList<GameObject> Players => Shared._players;
		public static GameManager Shared { get; private set; }

		#endregion

		#region Function Events

		private void Awake()
		{
			if (Shared)
			{
				Destroy(gameObject);
				return;
			}

			_numPlayers = 0;
			Shared = this;
			DontDestroyOnLoad(Shared);
		}

		#endregion

		#region Public Methods

		public void AddPlayer(PlayerInput input)
		{
			if (pressStartCanvas.activeSelf)
				pressStartCanvas.SetActive(false);

			var player = input.gameObject;
			DontDestroyOnLoad(player);
			_players.Add(player);
			_playerReadyStatus.Add(player, false);
			SetUpPlayerForStartScene(player, _numPlayers);
			_numPlayers++;
			_playerInputs.Add(input, "");
		}

		public void PlayerWon(bool rightLost)
		{
			if (!_gameStarted) return;
			HaltAI();
			_gameStarted = false;
			SceneManager.LoadSceneAsync(rightLost ? "P2 won" : "P1 won");
			StartCoroutine(ResetTimer(3.5f));
		}

		public void PlayerReady(GameObject player)
		{
			_playerReadyStatus[player] = !_playerReadyStatus[player];
			player.GetComponent<Animator>().SetFloat(PlayerAnimatorX, _playerReadyStatus[player] ? 1 : 0);
			if (_playerReadyStatus.Any(status => !status.Value)) return;
			if (_gameStarted || _numPlayers == 3) return;
			if (_numPlayers > 1)
				StartGameMultiplePlayers();
			else
				StartCoroutine(OnePlayerReady());
			_gameStarted = true;
		}

		public void AwakeAI()
		{
			if (_AIPlaying)
				_players[1].GetComponent<AIController>().enabled = true;
		}

		public IEnumerable<GameObject> GetOpposingPlayer(GameObject callingPlayer)
		{
			return _players.Where(player => player != callingPlayer);
		}

		public void Pause(PlayerInput playerInput)
		{
			if (_paused) return;
			_prevTimeScale = Time.timeScale;
			Time.timeScale = 0;
			_paused = true;
			_pausedBy = playerInput;
			pauseCanvas.SetActive(true);
			foreach (var player in _playerInputs.Keys.Where(player => player).ToList())
			{
				_playerInputs[player] = playerInput.currentActionMap.name;
				player.SwitchCurrentActionMap("Game Paused");
			}
		}

		public void Resume(PlayerInput playerInput)
		{
			if (!_paused || _pausedBy != playerInput) return;
			Time.timeScale = _prevTimeScale;
			_paused = false;
			_pausedBy = null;
			pauseCanvas.SetActive(false);
			foreach (var playerAndActionMap in _playerInputs.Where(player => player.Key).ToList())
				playerAndActionMap.Key.SwitchCurrentActionMap(playerAndActionMap.Value);
		}

		public void Quit(PlayerInput playerInput)
		{
			pauseCanvas.SetActive(false);
			foreach (var playerAndActionMap in _playerInputs.Where(player => player.Key).ToList())
				playerAndActionMap.Key.SwitchCurrentActionMap("Start Menu");
			SceneManager.LoadScene("Opening Screen");
		}

		#endregion

		#region Private Methods

		private IEnumerator ResetTimer(float time)
		{
			yield return new WaitForSeconds(time);
			StartGameMultiplePlayers();
			_gameStarted = true;
		}

		private void HaltAI()
		{
			if (_AIPlaying)
				_players[1].GetComponent<AIController>().enabled = false;
		}

		private void SetUpPlayerForStartScene(GameObject player, int playerID)
		{
			player.GetComponent<CharacterController>().enabled = true;
			player.transform.position = playerInfos[playerID].locationOpeningScene;
			player.GetComponent<PlayerInput>()
				?.SwitchCurrentActionMap("Player Tutorial Area"); // To keep Playability without entry scene
			if (playerID % 2 == 1)
				MakePlayerRed(player);
		}

		private void MakePlayerRed(GameObject player)
		{
			player.GetComponent<Animator>().runtimeAnimatorController = redController;
		}

		private void SetUpPlayerForGameScene(GameObject player, int playerID)
		{
			player.GetComponent<PlayerBrain>().Reset();
			player.transform.position = playerInfos[playerID].locationGameScene;
			player.GetComponent<CharacterController>().enabled = true;
			player.GetComponent<PlayerInput>()?.SwitchCurrentActionMap("Player");
			player.GetComponent<PlayerController>()?.OnMatchStart();
		}

		private void StartGameMultiplePlayers()
		{
			SceneManager.LoadScene(_sceneNames[(int)chooseTargetScene]);
			for (var i = 0; i < _players.Count; i++)
				SetUpPlayerForGameScene(_players[i], i);
		}

		private void StartGameOnePlayer()
		{
			CreateAI();
			StartGameMultiplePlayers();
		}

		private void CreateAI()
		{
			_AIPlaying = true;
			var player = Instantiate(AIPlayerPrefab, playerInfos[1].locationOpeningScene,
				AIPlayerPrefab.transform.rotation);
			DontDestroyOnLoad(player);
			_players.Add(player);
			MakePlayerRed(player);
		}

		#endregion

		#region Private Coroutines

		private IEnumerator OnePlayerReady()
		{
			for (var i = waitTime; i > 0; i--)
			{
				yield return new WaitForSeconds(1);
				if (_numPlayers == 1) continue;
				_gameStarted = false;
				yield break;
			}

			StartGameOnePlayer();
		}

		#endregion
	}
}