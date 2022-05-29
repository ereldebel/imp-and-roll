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
		private readonly GameObject _explosionPrefab;
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball.Ball _ball;
		private GameObject _thrower;

		private const CollectibleType PowerUpType = CollectibleType.ExplodingBall;

		private static readonly Collider[] Hits = new Collider[4];

		public ExplodingBall(float explosionKnockBackRadius, float explosionStunRadius,
			float knockBackVelocityMultiplier, LayerMask playerLayerMask, GameObject explosionPrefab, Mesh mesh,
			Material material) : base(PowerUpType)
		{
			_explosionSqrKnockBackRadius = Mathf.Pow(explosionKnockBackRadius, 2);
			_explosionStunSqrRadius = Mathf.Pow(explosionStunRadius, 2);
			_knockBackVelocityMultiplier = knockBackVelocityMultiplier;
			_explosionPrefab = explosionPrefab;
			_playerLayerMask = playerLayerMask;
			_mesh = mesh;
			_material = material;
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
			var explosionPos = _ball.transform.position;
			explosionPos.y = 0;
			Object.Instantiate(_explosionPrefab, explosionPos, _explosionPrefab.transform.rotation);
			var numOfHits = Physics.OverlapSphereNonAlloc(explosionPos, _explosionSqrKnockBackRadius, Hits,
				_playerLayerMask.value);
			for (var i = 0; i < numOfHits; ++i)
			{
				var player = Hits[i].GetComponent<PlayerBrain>();
				if (!player) continue;
				var playerDir = explosionPos - Hits[i].transform.position;
				var playerSqrDist = playerDir.sqrMagnitude;
				var relativeVelocity =
					playerDir * (_explosionStunSqrRadius * _knockBackVelocityMultiplier / (playerSqrDist + 0.1f) + 2);
				var tookHit = false;
				if (playerSqrDist <= _explosionStunSqrRadius && Hits[i].gameObject != _thrower)
					tookHit = player.TakeHit(relativeVelocity, IsUncatchableWithRoll());
				if (!tookHit)
					player.ApplyKnockBack(relativeVelocity);
			}
		}
	}
}