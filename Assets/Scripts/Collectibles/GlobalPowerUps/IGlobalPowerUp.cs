namespace Collectibles.GlobalPowerUps
{
	public interface IGlobalPowerUp: ICollectible
	{
		float StartAndGetDuration();
		void OnUpdate();
		void End();
	}
}