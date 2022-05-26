using System.Transactions;
using Managers;
using Player;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class SuperThrows : PowerUp, IBallPowerUp
	{
		private PlayerBrain _target;
		private Collider _ball;

		private const CollectibleType PowerUpType = CollectibleType.SuperThrow;

		
		public override void Collect(GameObject collector)
		{
			var targetObject = GameManager.Shared.GetOpposingPlayer(collector);
			base.Collect(targetObject);
			_target = targetObject.GetComponent<PlayerBrain>();
		}

		public SuperThrows(float duration) : base(duration,PowerUpType)
		{

		}

		public void OnThrow(Ball ball)
		{
			_ball = ball.GetComponent<Collider>();
		}

		public void OnLateUpdate()
		{
		}

		public bool OnHit()
		{
			return true;
		}

		public void End()
		{
		}
	}
}