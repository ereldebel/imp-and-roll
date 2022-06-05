using Collectibles.PowerUp.BallPowerUps.Effects;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class FireTrail : PowerUp, IBallPowerUp
	{
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private Ball.Ball _ball;

		private readonly GameObject _fireDropperPrefab;
		private GameObject _fireDropper;

		private const CollectibleType PowerUpType = CollectibleType.FireTrail;

		public FireTrail(GameObject fireDropperPrefab, Mesh mesh, Material[] materials) : base(PowerUpType)
		{
			_fireDropperPrefab = fireDropperPrefab;
			_mesh = mesh;
			_materials = materials;
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
		}

		public void OnCharge(Ball.Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterials(_materials);
			ball.Grow();
			_ball = ball;
		}

		public void OnThrow(Vector3 velocity)
		{
			_fireDropper = Object.Instantiate(_fireDropperPrefab);
			_fireDropper.SetActive(false);
			_fireDropper.transform.parent = _ball.transform;
			_fireDropper.transform.position = _ball.transform.position;
			_fireDropper.SetActive(true);
			_ball.Shrink();
		}

		public void OnLateUpdate()
		{
		}

		public void OnHit(Collision collision)
		{
			collision.gameObject.GetComponent<IHittable>()
				?.TakeHit(collision.relativeVelocity, IsUncatchableWithRoll());
			_fireDropper.transform.parent = null;
			_fireDropper.GetComponent<FireDropper>().Stop();
		}
	}
}