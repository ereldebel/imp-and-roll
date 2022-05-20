using UnityEngine;

namespace Player
{
	[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Create Player Info")]
	public class PlayerInfo : ScriptableObject
	{
		public Vector3 locationOpeningScene, locationGameScene;
	}
}