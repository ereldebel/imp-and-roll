using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CrossSceneManager : MonoBehaviour
{
	[SerializeField] private PlayerInfo[] playerInfos = new PlayerInfo[2];
	[SerializeField] private AnimatorOverrideController redController;
	[SerializeField] private float waitTime = 2;
	[SerializeField] private GameObject AIPlayerPrefab;
	private readonly List<GameObject> _players = new List<GameObject>();
	private readonly Dictionary<GameObject, bool> _playerReadyStatus = new Dictionary<GameObject, bool>();
	private int _numPlayers = 0;

	public static List<GameObject> Players => Shared._players;
	public static CrossSceneManager Shared { get; private set; }

	private void Awake()
	{
		_numPlayers = 0;
		Shared = this;
		DontDestroyOnLoad(Shared);
	}
	
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
		StartCoroutine(MatchEnd(rightLost ? "P2 won" : "P1 won"));
	}

	private IEnumerator MatchEnd(string winScene)
	{
		var job = SceneManager.LoadSceneAsync(winScene, LoadSceneMode.Additive);
		yield return new WaitWhile(() => !job.isDone);
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(winScene));
		yield return new WaitForSeconds(3.5f);
		StartGameTwoPlayers();
	}

	public void PlayerReady(GameObject player)
	{
		_playerReadyStatus[player] = !_playerReadyStatus[player];
		if (_playerReadyStatus.Any(status => !status.Value)) return;
		SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
		if (_numPlayers > 1)
			StartGameTwoPlayers();
		else
			StartCoroutine(OnePlayerReady());
	}

	private void SetUpPlayerForStartScene(GameObject player, int playerID)
	{
		player.GetComponent<CharacterController>().enabled = false;
		player.transform.position = playerInfos[playerID].locationOpeningScene;
		player.GetComponent<PlayerInput>()
			.SwitchCurrentActionMap("Start Menu"); // To keep Playability without entry scene
		if (playerID == 1) MakePlayerRed(player);
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
		SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		MainCamera.ToGamePosition();
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
		var player = Instantiate(AIPlayerPrefab, playerInfos[1].locationOpeningScene, AIPlayerPrefab.transform.rotation);
		DontDestroyOnLoad(player);
		_players.Add(player);
		MakePlayerRed(player);
	}

	private IEnumerator OnePlayerReady()
	{
		for (var i = waitTime; i > 0; i--)
		{
			print(i);
			yield return new WaitForSeconds(1);
			if (_numPlayers != 1) yield break;
		}

		StartGameOnePlayer();
	}
}