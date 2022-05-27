namespace Collectibles.PowerUp.GlobalPowerUps
{
	public interface IGlobalPowerUp : ITimedPowerUp
	{
		void OnUpdate();
		void End();
	}
}