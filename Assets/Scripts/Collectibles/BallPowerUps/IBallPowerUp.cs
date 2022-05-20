namespace Collectibles.BallPowerUps
{
	public interface IBallPowerUp: ICollectible
	{
		void OnThrow(Ball ball);
		void OnUpdate(Ball ball);
		void OnHit(Ball ball);
	}
}