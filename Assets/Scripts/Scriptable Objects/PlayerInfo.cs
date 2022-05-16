using UnityEngine;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "PlayerInfo", menuName = "Create PlayerInfo")]
	public class PlayerInfo : ScriptableObject
	{
		public Vector3 locationOpeningScene, locationGameScene;
	}
}