using UnityEngine;

namespace Ball
{
	public class DefaultBallStrategy : IBallStrategy
	{
		private readonly Mesh _mesh;
		private readonly Material _material;
		private Ball _ball;

		public DefaultBallStrategy(Mesh mesh,Material material)
		{
			_mesh = mesh;
			_material = material;
		}

		public void OnCharge(Ball ball)
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

		public bool OnHit()
		{
			return false;
		}
	}
}