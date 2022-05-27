namespace Collectibles.PowerUp
{
	public interface ITimedPowerUp: ICollectible
	{
		float StartAndGetDuration();
	}
}