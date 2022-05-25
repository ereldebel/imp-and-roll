using System.Transactions;
using Managers;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class HomingBall : PowerUp, IBallPowerUp
	{
		private readonly float _attractionRate;
		private Transform _target;
		private Rigidbody _ball;
		private const CollectibleType PowerUpType = CollectibleType.HomingBall;

		public HomingBall(float duration, float attractionRate) : base(duration,PowerUpType)
		{
			_attractionRate = -1 / attractionRate;
		}

		public override void Collect(GameObject collector)
		{
			var targetObject = GameManager.Shared.GetOpposingPlayer(collector);
			base.Collect(targetObject);
			_target = targetObject.GetComponent<Transform>();
		}

		public void OnThrow(Ball ball)
		{
			_ball = ball.GetComponent<Rigidbody>();
		}

		public void OnLateUpdate()
		{
			var diff = _target.position - _ball.position;
			if (diff == Vector3.zero) return;
			var diffNorm = diff.magnitude;
			var currVelocity = _ball.velocity;
			_ball.velocity =
				Vector3.Slerp(currVelocity, diff * (currVelocity.magnitude / diffNorm),
					Mathf.Pow(diffNorm + 1, _attractionRate));
		}

		public bool OnHit()
		{
			return false;
		}

		public void End()
		{
		}

		public CollectibleType GetMyType()
		{
			return PowerUpType;
		}
	}
}