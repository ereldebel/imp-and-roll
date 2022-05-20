using System;
using Collectibles.GlobalPowerUps;
using Managers;
using UnityEngine;

namespace Collectibles
{
	[RequireComponent(typeof(Collider))]
	public class Collectible : MonoBehaviour
	{
		[SerializeField] private CollectibleType collectibleType;
		[SerializeField] private CollectibleFactory collectibleFactory;

		private ICollectible _collectible;
		private void Awake()
		{
			_collectible = collectibleFactory.Create(collectibleType);
		}

		private void OnEnable()
		{
			MatchManager.AddToCollectibleCollection(transform);
		}
		
		private void OnDisable()
		{
			MatchManager.RemoveFromCollectibleCollection(transform);
		}

		private void OnTriggerEnter(Collider other)
		{
			_collectible.Collect(other.gameObject);
			if (_collectible is IGlobalPowerUp globalPowerUp)
				MatchManager.AddGlobalPowerUp(globalPowerUp);
			// if (_collectible is IBallPowerUp ballPowerUp)
			// 	other.GetComponent<PlayerBrain>()?.AddBallPowerUp(ballPowerUp);
			Destroy(gameObject);
		}
	}
}