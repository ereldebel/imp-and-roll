namespace Collectibles.PowerUp.BallPowerUps
{
	public interface IBallPowerUp: IPowerUp
	{
		void OnThrow(Ball ball);
		void OnLateUpdate();
		void OnHit();
		void End();
	}
}