using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Create PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public Vector3 LocationOpeningScene, LocationGameScene;
}
