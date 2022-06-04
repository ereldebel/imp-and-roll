using UnityEngine;

namespace Ball
{
	public class DefaultBallStrategy : IBallStrategy
	{
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball _ball;

		public DefaultBallStrategy(Mesh mesh, Material material)
		{
			_mesh = mesh;
			_material = material;
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
		}

		public void Apply(Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterial(_material);
		}

		public void OnCharge(Ball ball)
		{
			Apply(ball);
			ball.Grow();
			_ball = ball;
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
			collision.gameObject.GetComponent<IHittable>()
				?.TakeHit(collision.relativeVelocity, IsUncatchableWithRoll());
		}

		public void OnRemove()
		{
		}
	}
}