using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using Managers;
using Player;
using UnityEngine;

namespace Collectibles
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class Collectible : MonoBehaviour
	{
		[SerializeField] private CollectibleType collectibleType;

		public CollectibleType CollectibleType
		{
			set
			{
				_collectible = collectibleFactory.Create(value);
				GetComponent<SpriteRenderer>().sprite = collectibleFactory.Icon(value);
			}
		}

		[SerializeField] private CollectibleFactory collectibleFactory;

		private ICollectible _collectible;

		private void OnValidate() => CollectibleType = collectibleType;

		private void OnEnable()
		{
			OnValidate();
			MatchManager.AddToCollectibleCollection(transform);
		}

		private void OnDisable() => MatchManager.RemoveFromCollectibleCollection(transform);

		private void OnTriggerEnter(Collider other)
		{
			_collectible.Collect(other.gameObject);
			switch (_collectible)
			{
				case IGlobalPowerUp globalPowerUp:
					MatchManager.AddGlobalPowerUp(globalPowerUp);
					break;
				case IBallPowerUp ballPowerUp:
					other.GetComponent<PlayerBrain>()?.AddBallPowerUp(ballPowerUp);
					break;
			}

			Destroy(gameObject);
		}
	}
}