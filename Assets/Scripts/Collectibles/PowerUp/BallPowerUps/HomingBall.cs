using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class HomingBall : PowerUp, IBallPowerUp
	{
		private readonly float _attractionRate;
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private Ball.Ball _ball;

		private Transform _target;
		private Rigidbody _ballRigidbody;
		private IEnumerable<GameObject> _otherPlayers;

		private static readonly Quaternion RotationOffset = new Quaternion(0, -0.6f, -0.8f, 0);
		private Transform _ballTransform;
		private const CollectibleType PowerUpType = CollectibleType.HomingBall;

		public HomingBall(float attractionRate, Mesh mesh, Material[] materials) : base(PowerUpType)
		{
			_attractionRate = -1 / attractionRate;
			_mesh = mesh;
			_materials = materials;
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
			_otherPlayers = GameManager.Shared.GetOpposingPlayers(collector);
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
		}

		public void OnCharge(Ball.Ball ball)
		{
			_ballTransform = ball.transform;
			ball.SetMesh(_mesh);
			ball.SetMaterials(_materials);
			_ballTransform.LookAt(_target);
			_ballTransform.rotation = ball.transform.parent.rotation * RotationOffset * _ballTransform.rotation;
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
			if (_target == null) return;
			var diff = _target.position - _ballRigidbody.position;
			if (diff == Vector3.zero) return;
			var diffNorm = diff.magnitude;
			var currVelocity = _ballRigidbody.velocity;
			var currYVelocity = currVelocity.y;
			currVelocity.y = 0;
			var newVelocity = Vector3.Slerp(currVelocity, diff * (currVelocity.magnitude / diffNorm),
				Mathf.Pow(diffNorm + 1, _attractionRate));
			newVelocity.y = currYVelocity;
			_ballRigidbody.velocity = newVelocity;
			_ballTransform.LookAt(_target);
			_ballTransform.rotation = RotationOffset * _ballTransform.rotation;
		}

		public void OnHit(Collision collision)
		{
			collision.gameObject.GetComponent<IHittable>()
				?.TakeHit(collision.relativeVelocity, IsUncatchableWithRoll());
		}
	}
}