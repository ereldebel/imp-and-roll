﻿using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class SuperThrow : PowerUp, IBallPowerUp
	{
		private readonly float _speedBoostFactor;
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private Ball.Ball _ball;

		private Rigidbody _ballRigidBody;

		private const CollectibleType PowerUpType = CollectibleType.SuperThrow;

		public SuperThrow(float speedBoostFactor, Mesh mesh, Material[] materials) : base(PowerUpType)
		{
			_speedBoostFactor = speedBoostFactor;
			_mesh = mesh;
			_materials = materials;
		}

		public bool IsUncatchableWithRoll()
		{
			return true;
		}

		public void OnCharge(Ball.Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterials(_materials);
			ball.transform.rotation=Quaternion.identity;
			_ballRigidBody = ball.GetComponent<Rigidbody>();
			ball.Grow();
			_ball = ball;
		}

		public void OnThrow(Vector3 velocity)
		{
			_ballRigidBody.velocity *= _speedBoostFactor;
		}

		public void OnLateUpdate()
		{
		}

		public void OnHit(Collision collision)
		{
			_ball.Shrink();
			collision.gameObject.GetComponent<IHittable>()
				?.TakeHit(collision.relativeVelocity, IsUncatchableWithRoll());
		}
	}
}