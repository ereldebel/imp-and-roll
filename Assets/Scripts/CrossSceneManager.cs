using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossSceneManager : MonoBehaviour
{
    [SerializeField] private PlayerInfo[] _playerInfos;
    [SerializeField] private Component aiBrain;
    private List<GameObject> _players = new List<GameObject>();
    private int _numPlayers = 0;
    
    
    public void AddPlayer(PlayerInput input)
    {
        var player = input.gameObject;
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = _playerInfos[_numPlayers].LocationOpeningScene;
        player.GetComponent<PlayerInput>().SwitchCurrentActionMap("Start Menu");// To keep Playability without entry scene
        _numPlayers++;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
