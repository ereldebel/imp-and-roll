using Managers;
using Player;
using UnityEngine;

namespace Collectibles.GlobalPowerUps
{
	public class InvertControls: IGlobalPowerUp
	{
		private PlayerBrain _invertedPlayer;
		private readonly float _duration;

		public InvertControls(float duration)
		{
			_duration = duration;
		}

		public void Collect(GameObject collector)
		{
			_invertedPlayer = GameManager.Shared.GetOpposingPlayer(collector).GetComponent<PlayerBrain>();
		}
		
		public float StartAndGetDuration()
		{
			_invertedPlayer.InvertJoysticks = true;
			return _duration;
		}

		public void OnUpdate(){}

		public void End()
		{
			_invertedPlayer.InvertJoysticks = false;
		}
	}
}