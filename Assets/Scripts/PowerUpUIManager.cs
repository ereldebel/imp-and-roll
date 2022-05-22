using System;
using System.Collections;
using System.Collections.Generic;
using Collectibles;
using Managers;
using UnityEngine;
using Collectibles;
using Image = UnityEngine.UI.Image;

public class PowerUpUIManager : MonoBehaviour
{
    [SerializeField] private CollectibleType[] powerUps = {CollectibleType.AttractCollectibles,CollectibleType.HomingBall,CollectibleType.InvertControls};
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Image[] redPlayerIcons;
    [SerializeField] private Image[] bluePlayerIcons;
    private List<CollectibleType> _redPlayerList = new List<CollectibleType>();
    private List<CollectibleType> _bluePlayerList = new List<CollectibleType>();

    private GameObject _redPlayer, _bluePlayer;

    private void OnEnable()
    {  
        _redPlayer = GameManager.Players[0];
        _redPlayer = GameManager.Players[1];
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
