using System;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using Managers;
using Player;
using UnityEngine;

namespace Collectibles
{
	[RequireComponent(typeof(Collider))]
	public class Collectible : MonoBehaviour
	{
		public CollectibleType collectibleType;

		[SerializeField] private CollectibleFactory collectibleFactory;

		private ICollectible _collectible;

		private void Awake() => OnValidate();

		private void OnValidate() => _collectible = collectibleFactory.Create(collectibleType);
		
		private void OnEnable() => MatchManager.AddToCollectibleCollection(transform);
		
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