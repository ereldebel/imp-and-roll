using Player;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class ExplodingBall : PowerUp, IBallPowerUp
	{
		private readonly float _explosionSqrKnockBackRadius;
		private readonly float _explosionStunSqrRadius;
		private readonly float _knockBackVelocityMultiplier;
		private readonly LayerMask _playerLayerMask;
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball.Ball _ball;
		private GameObject _thrower;

		private const CollectibleType PowerUpType = CollectibleType.ExplodingBall;

		private static readonly Collider[] Hits = new Collider[4];

		public ExplodingBall(float explosionKnockBackRadius, float explosionStunRadius,
			float knockBackVelocityMultiplier, LayerMask playerLayerMask, Mesh mesh, Material material) :
			base(PowerUpType)
		{
			_explosionSqrKnockBackRadius = Mathf.Pow(explosionKnockBackRadius, 2);
			_explosionStunSqrRadius = Mathf.Pow(explosionStunRadius, 2);
			_knockBackVelocityMultiplier = knockBackVelocityMultiplier;
			_mesh = mesh;
			_material = material;
			_playerLayerMask = playerLayerMask;
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
			_thrower = collector;
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
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
			
			var numOfHits = Physics.OverlapSphereNonAlloc(_ball.transform.position, _explosionSqrKnockBackRadius, Hits,
				_playerLayerMask.value);
			for (var i = 0; i < numOfHits; ++i)
			{
				var player = Hits[i].GetComponent<PlayerBrain>();
				var playerDir = Hits[i].transform.position - _ball.transform.position;
				var playerSqrDist = playerDir.sqrMagnitude;
				var relativeVelocity =
					playerDir * (_explosionStunSqrRadius * _knockBackVelocityMultiplier / playerSqrDist);
				var tookHit = false;
				if (playerSqrDist <= _explosionStunSqrRadius && Hits[i].gameObject != _thrower)
					tookHit = player.TakeHit(relativeVelocity, IsUncatchableWithRoll());
				if (!tookHit)
					player.ApplyKnockBack(relativeVelocity);
			}
		}
	}
}