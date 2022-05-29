using System.Collections;
using Collectibles.PowerUp.BallPowerUps.Effects;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps
{
	public class FireTrail : PowerUp, IBallPowerUp
	{
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball.Ball _ball;
		
		private static GameObject _fireDropper;
		private static FireDropper _fireDropperScript;

		private const CollectibleType PowerUpType = CollectibleType.FireTrail;

		public FireTrail(GameObject fireDropperPrefab, Mesh mesh, Material material) : base(PowerUpType)
		{
			if (_fireDropper == null)
			{
				_fireDropper = Object.Instantiate(fireDropperPrefab);
				_fireDropper.SetActive(false);
				_fireDropperScript = _fireDropper.GetComponent<FireDropper>();
			}
			_mesh = mesh;
			_material = material;
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
			_fireDropperScript.Stop();
		}
	}
}