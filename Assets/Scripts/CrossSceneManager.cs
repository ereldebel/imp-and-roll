using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.Mathematics;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CrossSceneManager : MonoBehaviour
{ 
    [SerializeField] private PlayerInfo[] _playerInfos ;
    [SerializeField] private AnimatorOverrideController redController;
    [SerializeField] private float waitTime = 2;
    private List<GameObject> _players = new List<GameObject>();
    private List<bool> _playerReadyStatus = new List<bool>();
    private int _numPlayers = 0;

    public static List<GameObject> Players => Shared._players;
    public static CrossSceneManager Shared { get; private set; }

    public void AddPlayer(PlayerInput input)
    {
        var player = input.gameObject;
        DontDestroyOnLoad(player);
        _players.Add(player);
        _playerReadyStatus.Add(false);
        SetUpPlayerForStartScene(player, _numPlayers);
        _numPlayers++;
    }

    private void SetUpPlayerForStartScene(GameObject player, int playerID)
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = _playerInfos[playerID].LocationOpeningScene;
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Start Menu");// To keep Playability without entry scene
        if (playerID == 1) MakePlayerRed(player);
    }
    private void MakePlayerRed(GameObject player)
    {
        player.GetComponent<Animator>().runtimeAnimatorController = redController;
    }
    private void SetUpPlayerForGameScene(GameObject player, int playerID)
    {
        player.GetComponent<CharacterController>().enabled = true;
        player.transform.position = _playerInfos[playerID].LocationGameScene;
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");
    }

    public void PlayerWon(bool rightLost)
    {

        SceneManager.LoadScene(rightLost ? "P2 won":"P1 won");
        Invoke(nameof(StartGameTwoPlayers),3.5f);
    }
    public void PlayerReady(GameObject player)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i] == player)
            {
                _playerReadyStatus[i] = !_playerReadyStatus[i];
            }
        }

        var allReady = true;
        foreach (var status in _playerReadyStatus.Where(status => !status))
        {
            allReady = false;
        }

        if (!allReady) return;
        if (_numPlayers > 1)
        {
            StartGameTwoPlayers();   
        }
        else
        {
            StartCoroutine(OnePlayerReady());
        }

    }

    private void StartGameTwoPlayers()
    {
        SceneManager.LoadScene("Game");
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].transform.position = _playerInfos[i].LocationGameScene;
            SetUpPlayerForGameScene(_players[i], i);
        }
    }
    private void StartGameOnePlayer()
    {
        CreateBot();
        StartGameTwoPlayers();
    }

    private void CreateBot()
    {
        var player = Instantiate(_players[0], _playerInfos[1].LocationOpeningScene, quaternion.identity);
        _players[1].AddComponent<AIController>();
        MakePlayerRed(_players[1]);
    }
    // Start is called before the first frame update
    private void Start()
    {
        _numPlayers = 0;
        Shared = this;
        DontDestroyOnLoad(Shared);
    }

    private IEnumerator OnePlayerReady()
    {
        yield return new WaitForSeconds(waitTime);
        if (_numPlayers != 1) yield break;
        StartGameOnePlayer();
    }

}
