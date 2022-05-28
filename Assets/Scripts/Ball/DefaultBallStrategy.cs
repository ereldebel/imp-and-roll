using System;
using UnityEngine;

namespace Ball
{
	public class DefaultBallStrategy : IBallStrategy
	{
		private readonly Mesh _mesh;
		private readonly Material _material;

		public DefaultBallStrategy(Mesh mesh,Material material)
		{
			_mesh = mesh;
			_material = material;
		}

		public void OnCharge(Ball ball)
		{
			ball.SetMesh(_mesh);
			ball.SetMaterial(_material);
		}

		public void OnThrow()
		{
			
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