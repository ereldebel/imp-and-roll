using UnityEngine;

namespace Ball
{
	public class DefaultBallStrategy : IBallStrategy
	{
		private readonly Mesh _mesh;
		private readonly Material[] _materials;
		private Ball _ball;

		public DefaultBallStrategy(Mesh mesh, Material[] materials)
		{
			_mesh = mesh;
			_materials = materials;
		}

		public bool IsUncatchableWithRoll()
		{
			return false;
		}

		public void Apply(Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterials(_materials);
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