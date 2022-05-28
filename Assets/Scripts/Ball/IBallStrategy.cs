using UnityEngine;

namespace Ball
{
	public interface IBallStrategy
	{
		bool IsUncatchableWithRoll();
		void OnCharge(Ball ball);
		void OnThrow();
		void OnLateUpdate();
		void OnHit(Collision collision);
	}
}