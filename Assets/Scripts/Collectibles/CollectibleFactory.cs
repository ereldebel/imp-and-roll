using System;
using Collectibles.GlobalPowerUps;
using Collectibles.BallPowerUps;
using UnityEngine;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Invert Controls")] [SerializeField]
		private float invertControlsDuration;

		[Header("Attract Collectibles")] [SerializeField]
		private float attractCollectiblesDuration;

		[SerializeField] private float attractCollectiblesBaseAttractionSpeed;
		[SerializeField] private float attractCollectiblesAttractionRadius;

		public ICollectible Create(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.InvertControls => new InvertControls(invertControlsDuration),
				CollectibleType.AttractCollectibles => new AttractCollectibles(attractCollectiblesDuration,
					attractCollectiblesBaseAttractionSpeed, attractCollectiblesAttractionRadius),
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}
	}
}