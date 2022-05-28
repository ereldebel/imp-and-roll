using Player;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class ExplodingBall : PowerUp, IBallPowerUp
	{
		private readonly float _explosionKnockBackRadius;
		private readonly float _explosionsStunRadius;
		private readonly LayerMask _playerLayerMask;
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball.Ball _ball;
		
		private const CollectibleType PowerUpType = CollectibleType.ExplodingBall;

		private static readonly Collider[] Hits = new Collider[2];

		public ExplodingBall(float explosionKnockBackRadius, float explosionsStunRadius, Mesh mesh, Material material, LayerMask playerLayerMask) : base(PowerUpType)
		{
			_explosionKnockBackRadius = explosionKnockBackRadius;
			_mesh = mesh;
			_material = material;
			_playerLayerMask = playerLayerMask;
			_explosionsStunRadius = explosionsStunRadius;
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
		}

		public void OnCharge(Ball.Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterial(_material);
			ball.Grow();
			_ball = ball;
		}

		public void OnThrow()
		{
			_ball.Shrink();
		}

		public void OnLateUpdate()
		{
		}

		public void OnHit(Collision collision)
		{
			if (Physics.OverlapSphereNonAlloc(_ball.transform.position, _explosionKnockBackRadius, Hits, _playerLayerMask) ==
			    0) return;
			foreach (var hit in Hits)
			{
				var player = hit.GetComponent<PlayerBrain>();
				var playerDir = hit.transform.position - _ball.transform.position ;
				var relativeVelocity = playerDir / playerDir.sqrMagnitude;
				if (playerDir.magnitude <= _explosionsStunRadius)
					player.TakeHit(relativeVelocity, false);
				else
					player.ApplyKnockBack(relativeVelocity);
			}
		}
	}
}