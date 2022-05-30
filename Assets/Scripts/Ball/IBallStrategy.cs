using UnityEngine;

namespace Ball
{
	public interface IBallStrategy : IRemovable
	{
		bool IsUncatchableWithRoll();
		void OnCharge(Ball ball);
		void OnThrow(Vector3 velocity);
		void OnLateUpdate();
		void OnHit(Collision collision);
	}
}