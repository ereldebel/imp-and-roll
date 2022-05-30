using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class HomingBall : PowerUp, IBallPowerUp
	{
		private readonly float _attractionRate;
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball.Ball _ball;

		private Transform _target;
		private Rigidbody _ballRigidbody;
		private IEnumerable<GameObject> _otherPlayers;
		private const CollectibleType PowerUpType = CollectibleType.HomingBall;

		public HomingBall(float attractionRate, Mesh mesh, Material material) : base(PowerUpType)
		{
			_attractionRate = -1 / attractionRate;
			_mesh = mesh;
			_material = material;
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
			_otherPlayers = GameManager.Shared.GetOpposingPlayer(collector);
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
		}

		public void OnCharge(Ball.Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterial(_material);
			_ballRigidbody = ball.GetComponent<Rigidbody>();
			ball.Grow();
			_ball = ball;
		}

		public void OnThrow(Vector3 velocity)
		{
			_target = null;
			float bestAngle = 180;
			foreach (var player in _otherPlayers)
			{
				var playerTransform = player.transform;
				var dirToPlayer = playerTransform.position - _ball.transform.position;
				var angle = Vector3.Angle(velocity, dirToPlayer);
				if (angle >= bestAngle && _target) continue;
				bestAngle = angle;
				_target = playerTransform;
			}
			_ball.Shrink();
		}

		public void OnLateUpdate()
		{
			var diff = _target.position - _ballRigidbody.position;
			if (diff == Vector3.zero) return;
			var diffNorm = diff.magnitude;
			var currVelocity = _ballRigidbody.velocity;
			_ballRigidbody.velocity =
				Vector3.Slerp(currVelocity, diff * (currVelocity.magnitude / diffNorm),
					Mathf.Pow(diffNorm + 1, _attractionRate));
		}

		public void OnHit(Collision collision)
		{
			collision.gameObject.GetComponent<IHittable>()
				?.TakeHit(collision.relativeVelocity, IsUncatchableWithRoll());
		}
	}
}