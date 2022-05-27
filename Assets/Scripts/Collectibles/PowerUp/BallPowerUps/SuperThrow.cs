using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class SuperThrow : PowerUp, IBallPowerUp
	{
		private readonly float _speedBoostFactor;
		private readonly Mesh _mesh;
		private readonly Material _material;
		
		private Rigidbody _ballRigidBody;

		private const CollectibleType PowerUpType = CollectibleType.SuperThrow;

		public SuperThrow(float speedBoostFactor, Mesh mesh, Material material) : base(PowerUpType)
		{
			_speedBoostFactor = speedBoostFactor;
			_mesh = mesh;
			_material = material;
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
		}

		public void OnApply()
		{
			Start();
		}

		public void OnCharge(Ball.Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterial(_material);
			_ballRigidBody = ball.GetComponent<Rigidbody>();
		}

		public void OnThrow()
		{
			_ballRigidBody.velocity *= _speedBoostFactor;
		}

		public void OnLateUpdate()
		{
		}

		public bool OnHit()
		{
			return true;
		}

		public void OnRemove()
		{
			End();
		}
	}
}