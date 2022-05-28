namespace Ball
{
	public interface IBallStrategy
	{
		void OnCharge(Ball ball);
		void OnThrow();
		void OnLateUpdate();
		bool OnHit();
	}
}