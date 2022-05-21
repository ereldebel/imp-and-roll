namespace Collectibles.PowerUp.GlobalPowerUps
{
	public interface IGlobalPowerUp: IPowerUp
	{
		void OnUpdate();
		void End();
	}
}