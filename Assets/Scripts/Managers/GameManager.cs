using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI;
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
		[SerializeField] private OpeningScreen openingScreen;
		[SerializeField] private GameObject powerUps;
		[SerializeField] private GameObject pauseCanvas;
		[SerializeField] private SceneTransitioner transitioner;
		[SerializeField] private GameObject[] huds;
		[SerializeField] private Animator emptyHudAnimator;
		[SerializeField] private GameObject AILabel;

		#endregion

		#region Private Fields

		private List<GameObject> _players;
		private Dictionary<GameObject, bool> _playerReadyStatus;
		private int _numPlayers;
		private bool _gameStarted;
		private bool _AIPlaying;

		private float _prevTimeScale;
		private bool _paused;
		private PlayerInput _pausedBy;
		private Dictionary<PlayerInput, string> _playerInputs;
		private bool _quit = false;

		private readonly string[] _sceneNames = {"Opening Scene", "Original Arena", "Icy Arena", "Volcanic Arena"};
		private int _curScene;

		private int _blueScore, _redScore;
		private AIController _AIController;

		private enum TargetScene
		{
			OpeningScene = 0,
			OriginalArena = 1,
			IcyArena = 2,
			VolcanicArena = 3
		}

		#endregion

		#region Public Properties

		public static GameManager Shared { get; private set; }
		public static IReadOnlyList<GameObject> Players => Shared._players;
		public static int BlueScore => Shared ? Shared._blueScore : -1;
		public static int RedScore => Shared ? Shared._redScore : -1;
		public static int CurScene => Shared ? Shared._curScene : -1;

		#endregion

		#region Function Events

		private void Awake()
		{
			openingScreen.gameObject.SetActive(true);
			_curScene = 0;
			if (Shared != null && Shared._quit)
			{
				Destroy(Shared.GetComponent<PlayerInputManager>());
				foreach (var player in Shared._players)
					Destroy(player);
				Destroy(Shared.gameObject);
				Shared = null;
			}
			if (Shared == null)
			{
				if (SceneManager.GetActiveScene().buildIndex != 0)
				{
					Destroy(gameObject);
					return;
				}

				_playerReadyStatus = new Dictionary<GameObject, bool>();
				_players = new List<GameObject>();
				_playerInputs = new Dictionary<PlayerInput, string>();
				_numPlayers = 0;
				Shared = this;
				DontDestroyOnLoad(Shared.gameObject);
			}
			else
			{
					_playerReadyStatus = Shared._playerReadyStatus;
					_players = Shared._players;
					_playerInputs = Shared._playerInputs;
					_numPlayers = Shared._numPlayers;
					if (Shared != this)
						Destroy(Shared.gameObject);
					Shared = this;
					openingScreen.Enter();
					if (_numPlayers == 1)
					{
						huds[0].SetActive(true);
						emptyHudAnimator.gameObject.SetActive(true);
					}
					else
					{
						foreach (var hud in huds)
							hud.SetActive(true);
					}
			}
			powerUps.SetActive(true);
			
		}

		#endregion

		#region Public Methods

		public void AddPlayer(PlayerInput input)
		{
			if (_curScene != 0 || _players == null)
			{
				Destroy(input.gameObject);
				return;
			}

			if (openingScreen.Showing)
				openingScreen.Enter();
			if (!powerUps.activeSelf)
				powerUps.SetActive(true);

			var player = input.gameObject;
			DontDestroyOnLoad(player);
			_players.Add(player);
			_playerReadyStatus.Add(player, false);
			SetUpPlayerForStartScene(player, _numPlayers);
			_numPlayers++;
			_playerInputs.Add(input, "");
			switch (_numPlayers)
			{
				case 1:
					emptyHudAnimator.gameObject.SetActive(true);
					break;
				case 2:
					emptyHudAnimator.enabled = true;
					break;
			}
		}

		public void RemovePlayer(PlayerInput input)
		{
			RemovePlayer(input.gameObject);
		}

		private void RemovePlayer(GameObject player)
		{
			if (_curScene != 0 || _players == null)
			{
				Destroy(player);
				return;
			}

			var playerIndex = _players.FindIndex(somePlayer => somePlayer == player);
			if (playerIndex == 0)
			{
				foreach (var somePlayer in _players)
					Destroy(somePlayer);
				foreach (var hud in huds)
					hud.SetActive(false);
				_players = new List<GameObject>();
				_playerReadyStatus = new Dictionary<GameObject, bool>();
				_playerInputs = new Dictionary<PlayerInput, string>();
				_numPlayers = 0;
				openingScreen.Exit();
				powerUps.SetActive(false);
				emptyHudAnimator.gameObject.SetActive(false);
				return;
			}

			_players.RemoveAt(playerIndex);
			huds[playerIndex].SetActive(false);
			_playerReadyStatus.Remove(player);
			_numPlayers = player.GetComponent<AIController>() != null ? _numPlayers : _numPlayers - 1;
			var playerInput = player.GetComponent<PlayerInput>();
			if (playerInput != null)
				_playerInputs.Remove(playerInput);

			if (_numPlayers == 1)
				emptyHudAnimator.gameObject.SetActive(true);
			Destroy(player);
		}

		public void PlayerWon(bool blueLost)
		{
			if (!_gameStarted) return;
			HaltAI();
			_gameStarted = false;
			if (blueLost)
				++_redScore;
			else
				++_blueScore;
			if (Mathf.Abs(_redScore - _blueScore) > 1 || _curScene == 3)
			{
				var winner = _redScore > _blueScore ? "Red" : "Blue";
				StartCoroutine(DisplayWinner(winner, 3f));
			}
			else
				StartCoroutine(ResetTimer(3f));
		}

		public void PlayerReady(GameObject player)
		{
			_playerReadyStatus[player] = !_playerReadyStatus[player];
			if (_playerReadyStatus.Any(status => !status.Value)) return;
			if (_gameStarted || _numPlayers == 3) return;
			if (_numPlayers > 1)
				StartGameMultiplePlayers(++_curScene);
			else
				StartCoroutine(OnePlayerReady());
			_gameStarted = true;
		}

		public void AwakeAI()
		{
			if (_AIPlaying)
				_AIController.enabled = true;
		}

		public IEnumerable<GameObject> GetOpposingPlayers(GameObject callingPlayer)
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
			AudioManager.Pause();
			HaltAI();
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
			AudioManager.Resume();
			AwakeAI();
			foreach (var playerAndActionMap in _playerInputs.Where(player => player.Key).ToList())
				playerAndActionMap.Key.SwitchCurrentActionMap(playerAndActionMap.Value);
		}

		public void Quit()
		{
			HaltAI();
			Time.timeScale = _prevTimeScale;
			_paused = false;
			_pausedBy = null;
			pauseCanvas.SetActive(false);
			transitioner.TransitionToScene(0);
			Destroy(GetComponent<PlayerInputManager>());
			_quit = true;
		}

		#endregion

		#region Private Methods

		private void HaltAI()
		{
			if (_AIPlaying)
				_AIController.enabled = false;
		}


		private void MakePlayerRed(GameObject player)
		{
			player.GetComponent<Animator>().runtimeAnimatorController = redController;
		}

		private void SetUpPlayerForGameScene(GameObject player, int playerID)
		{
			var playerBrain = player.GetComponent<PlayerBrain>();
			var characterController = playerBrain.CharacterController;
			characterController.enabled = false;
			playerBrain.Reset(CurScene == 2);
			player.transform.position = playerInfos[playerID].locationGameScene;
			characterController.enabled = true;
			if (playerBrain.PlayerInput)
				playerBrain.PlayerInput.SwitchCurrentActionMap("Player");
			player.GetComponent<PlayerController>()?.OnMatchStart();
		}

		private void StartGameMultiplePlayers(int sceneToStart, Action preparationAction = null)
		{
			transitioner.TransitionToScene(_sceneNames[sceneToStart], () =>
			{
				preparationAction?.Invoke();
				for (var i = 0; i < _players.Count; i++)
					SetUpPlayerForGameScene(_players[i], i);
				AwakeAI();
			});
		}

		private void StartGameOnePlayer()
		{
			StartGameMultiplePlayers(++_curScene, CreateAI);
		}

		private void CreateAI()
		{
			_AIPlaying = true;
			AILabel.SetActive(true);
			var player = Instantiate(AIPlayerPrefab, playerInfos[1].locationOpeningScene,
				AIPlayerPrefab.transform.rotation);
			_AIController = player.GetComponent<AIController>();
			DontDestroyOnLoad(player);
			_players.Add(player);
			emptyHudAnimator.enabled = true;
			SetUpPlayerForStartScene(player, _numPlayers);
		}

		private void SetUpPlayerForStartScene(GameObject player, int playerID)
		{
			huds[playerID].SetActive(true);
			player.transform.position = playerInfos[playerID].locationOpeningScene;
			_playerReadyStatus[player] = false;
			player.GetComponent<PlayerInput>()
				?.SwitchCurrentActionMap("Player Tutorial Area"); // To keep Playability without entry scene
			if (playerID % 2 == 1)
				MakePlayerRed(player);
		}

		private void ResetGameKeepPlayers()
		{
			_redScore = 0;
			_blueScore = 0;
			_curScene = 0;
			if (_players[1].GetComponent<AIController>() != null)
				RemovePlayer(_players[1]);
			for (var i = 0; i < _players.Count; i++)
			{
				var playerBrain = _players[i].GetComponent<PlayerBrain>();
				playerBrain.Reset(false);
				playerBrain.CharacterController.enabled = false;
				SetUpPlayerForStartScene(_players[i], i);
				playerBrain.CharacterController.enabled = true;
			}

			transitioner.TransitionToScene(0);
		}

		private void SetUpPlayersForWinningScene()
		{
			for (var i = 0; i < _players.Count; i++)
				SetUpPlayerForWinningScene(_players[i], i);
		}

		private void SetUpPlayerForWinningScene(GameObject player, int playerID)
		{
			player.transform.position = playerInfos[playerID].locationGameScene;
			player.GetComponent<CharacterController>().enabled = false;
		}

		#endregion

		#region Private Coroutines

		private IEnumerator OnePlayerReady()
		{
			for (var i = waitTime; i > 0; i--)
			{
				yield return new WaitForSeconds(1);
				if (_playerReadyStatus.Count > 0 && _playerReadyStatus.All(pair => pair.Value)) continue;
				_gameStarted = false;
				yield break;
			}

			StartGameOnePlayer();
		}

		private IEnumerator ResetTimer(float time)
		{
			yield return new WaitForSeconds(time);
			StartGameMultiplePlayers(++_curScene);
			_gameStarted = true;
		}

		private IEnumerator DisplayWinner(string winner, float delay)
		{
			yield return new WaitForSeconds(delay);
			transitioner.QuickTransitionToScene($"{winner} won, arena {_curScene}");
			SetUpPlayersForWinningScene();
			Invoke(nameof(ResetGameKeepPlayers), 6f);
		}

		private static IEnumerator ResetPC(GameObject player, float time)
		{
			yield return new WaitForSeconds(time);
			player.GetComponent<CharacterController>().enabled = true;
		}

		#endregion
	}
}