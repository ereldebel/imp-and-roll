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

		#endregion

		#region Private Fields

		private readonly List<GameObject> _players = new List<GameObject>();
		private readonly Dictionary<GameObject, bool> _playerReadyStatus = new Dictionary<GameObject, bool>();
		private int _numPlayers = 0;
		private bool _gameStarted = false;
		private bool _AIPlaying;
		private static readonly int PlayerAnimatorX = Animator.StringToHash("X Direction");
		private Dictionary<int, Action> _scenesEvents = new Dictionary<int, Action>();

		#endregion

		#region Public Properties

		public static IReadOnlyList<GameObject> Players => Shared._players;
		public static GameManager Shared { get; private set; }

		#endregion

		#region Public C# Events

		public event Action Scene1IsOver;

		#endregion

		#region Function Events

		private void Awake()
		{
			_numPlayers = 0;
			Shared = this;
			DontDestroyOnLoad(Shared);
		}

		#endregion

		#region Public Methods

		public void AddPlayer(PlayerInput input)
		{
			var player = input.gameObject;
			DontDestroyOnLoad(player);
			_players.Add(player);
			_playerReadyStatus.Add(player, false);
			SetUpPlayerForStartScene(player, _numPlayers);
			_numPlayers++;
		}

		public void PlayerWon(bool rightLost)
		{
			if (!_gameStarted) return;
			HaltAI();
			_gameStarted = false;
			SceneManager.LoadSceneAsync(rightLost ? "P2 won" : "P1 won");
			StartCoroutine(ResetTimer(3.5f));
			StartCoroutine(SceneEnded(1));
		}

		public void PlayerReady(GameObject player)
		{
			_playerReadyStatus[player] = !_playerReadyStatus[player];
			player.GetComponent<Animator>().SetFloat(PlayerAnimatorX, _playerReadyStatus[player] ? 1 : 0);
			if (_playerReadyStatus.Any(status => !status.Value)) return;
			if (_gameStarted) return;
			_gameStarted = true;
			if (_numPlayers > 1)
				StartGameTwoPlayers();
			else
				StartCoroutine(OnePlayerReady());
		}

		public void AwakeAI()
		{
			if (_AIPlaying)
				_players[1].GetComponent<AIController>().enabled = true;
		}

		public GameObject GetOpposingPlayer(GameObject callingPlayer)
		{
			return _players.Find(player => player != callingPlayer);
		}

		#endregion

		#region Private Methods

		private IEnumerator ResetTimer(float time)
		{
			yield return new WaitForSeconds(time);
			StartGameTwoPlayers();
			_gameStarted = true;
		}

		private void HaltAI()
		{
			if (_AIPlaying)
				_players[1].GetComponent<AIController>().enabled = false;
		}

		private void SetUpPlayerForStartScene(GameObject player, int playerID)
		{
			player.GetComponent<CharacterController>().enabled = false;
			player.transform.position = playerInfos[playerID].locationOpeningScene;
			player.GetComponent<PlayerInput>()
				?.SwitchCurrentActionMap("Start Menu"); // To keep Playability without entry scene
			if (playerID == 1)
				MakePlayerRed(player);
		}

		private void MakePlayerRed(GameObject player)
		{
			player.GetComponent<Animator>().runtimeAnimatorController = redController;
		}

		private void SetUpPlayerForGameScene(GameObject player, int playerID)
		{
			player.GetComponent<CharacterController>().enabled = true;
			player.transform.position = playerInfos[playerID].locationGameScene;
			player.GetComponent<PlayerInput>()?.SwitchCurrentActionMap("Player");
		}

		private void StartGameTwoPlayers()
		{
			SceneManager.LoadScene("Game");
			for (var i = 0; i < _players.Count; i++)
				SetUpPlayerForGameScene(_players[i], i);
		}

		private void StartGameOnePlayer()
		{
			CreateAI();
			StartGameTwoPlayers();
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

		private IEnumerator SceneEnded(int SceneID)
		{
			yield return 0;
			// Action cur = _shared._scenesEvents[SceneID]; Tried doing an int, Action Dict to make it a bit future proof, failed
			// _shared._scenesEvents[SceneID]?.Invoke();
			Scene1IsOver?.Invoke();
		}

		#endregion
	}
}