namespace Ball
{
	public interface IBallStrategy
	{
		void OnApply();
		void OnCharge(Ball ball);
		void OnThrow();
		void OnLateUpdate();
		bool OnHit();
		void OnRemove();
	}
}