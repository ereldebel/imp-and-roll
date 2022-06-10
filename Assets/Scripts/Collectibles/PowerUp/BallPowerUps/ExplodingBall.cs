using Ball;
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
		private readonly GameObject _sparksPrefab;
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private Ball.Ball _ball;
		private GameObject _thrower;

		private const CollectibleType PowerUpType = CollectibleType.ExplodingBall;

		private static readonly Collider[] Hits = new Collider[4];
		private BallAudio _audio;
		private GameObject _sparks;

		public ExplodingBall(float explosionKnockBackRadius, float explosionStunRadius,
			float knockBackVelocityMultiplier, LayerMask playerLayerMask, GameObject explosionPrefab,
			GameObject sparksPrefab, Mesh mesh, Material[] materials) : base(PowerUpType)
		{
			_explosionSqrKnockBackRadius = Mathf.Pow(explosionKnockBackRadius, 2);
			_explosionStunSqrRadius = Mathf.Pow(explosionStunRadius, 2);
			_knockBackVelocityMultiplier = knockBackVelocityMultiplier;
			_explosionPrefab = explosionPrefab;
			_sparksPrefab = sparksPrefab;
			_playerLayerMask = playerLayerMask;
			_mesh = mesh;
			_materials = materials;
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
			ball.SetMaterials(_materials);
			ball.transform.rotation = Quaternion.Euler(90, 0, 0);
			ball.Grow();
			_ball = ball;
			_audio = ball.GetComponent<BallAudio>();
			_sparks = Object.Instantiate(_sparksPrefab, ball.transform);
		}

		public void OnThrow(Vector3 velocity)
		{
			_ball.Shrink();
		}

		public void OnLateUpdate()
		{
		}

		public void OnHit(Collision collision)
		{
			_audio.Explosion();
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

		public override void OnRemove()
		{
			Object.Destroy(_sparks);
			base.OnRemove();
		}
	}
}