namespace Collectibles.PowerUp.PlayerPowerUps
{
	public interface IPlayerPowerUp : ICollectible, IRemovable
	{
		void OnUpdate();
	}
}