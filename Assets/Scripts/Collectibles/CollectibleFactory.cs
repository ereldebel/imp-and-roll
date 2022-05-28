using System;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.PlayerPowerUps;
using UnityEngine;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Collectible Magnet")] [SerializeField]
		private float baseAttractionSpeed = 0.1f;

		[SerializeField] private float attractionRadius = 10;
		[SerializeField] private Sprite collectibleMagnetSprite;
		[SerializeField] private Sprite collectibleMagnetIcon;

		[Header("Homing Ball Collectibles")] [SerializeField]
		private float homingRate = 1;

		[SerializeField] private Mesh homingBallMesh;
		[SerializeField] private Material homingBallMaterial;
		[SerializeField] private Sprite homingBallSprite;
		[SerializeField] private Sprite homingBallIcon;

		[Header("Super Throw")] [SerializeField]
		private float superThrowSpeedBoost = 1.1f;

		[SerializeField] private Mesh superThrowMesh;
		[SerializeField] private Material superThrowMaterial;
		[SerializeField] private Sprite superThrowSprite;
		[SerializeField] private Sprite superThrowIcon;

		[Header("Exploding Ball")] [SerializeField]
		private float explosionStunRadius;

		[SerializeField] private float explosionKnockBackOuterRingWidth;
		[SerializeField] private float knockBackVelocityMultiplier;
		[SerializeField] private LayerMask playerLayerMask;
		[SerializeField] private Mesh explodingBallMesh;
		[SerializeField] private Material explodingBallMaterial;
		[SerializeField] private Sprite explodingBallSprite;
		[SerializeField] private Sprite explodingBallIcon;

		// [Space(5)] [Tooltip("random weight of each power up to spawn by their order of appearance")] [SerializeField]
		// private float[] powerUpWeights = new float[3];


		// private Random _random = new Random();

		// public ICollectible GetRandomWeightedPowerUp()
		// {
		// 	// Get the total sum of all the weights.
		// 	int weightSum = 0f;
		// 	for (int i = 0; i < powerUpWeights.Length; ++i)
		// 	{
		// 		weightSum += weights[i];
		// 	}
		//
		// 	// Step through all the possibilities, one by one, checking to see if each one is selected.
		// 	int index = 0;
		// 	int lastIndex = elementCount - 1;
		// 	while (index < lastIndex)
		// 	{
		// 		// Do a probability check with a likelihood of weights[index] / weightSum.
		// 		if (Random.Range(0, weightSum) < weights[index])
		// 		{
		// 			return index;
		// 		}
		//
		// 		// Remove the last item from the sum of total untested weights and try again.
		// 		weightSum -= weights[index++];
		// 	}
		//
		// 	// No other item was selected, so return very last index.
		// 	return index;
		// }

		public ICollectible Create(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.CollectibleMagnet => new CollectibleMagnet(baseAttractionSpeed,
					attractionRadius),
				CollectibleType.HomingBall => new HomingBall(homingRate, homingBallMesh,
					homingBallMaterial),
				CollectibleType.SuperThrow => new SuperThrow(superThrowSpeedBoost, superThrowMesh, superThrowMaterial),
				CollectibleType.ExplodingBall => new ExplodingBall(
					explosionStunRadius + explosionKnockBackOuterRingWidth, explosionStunRadius,
					knockBackVelocityMultiplier, playerLayerMask, explodingBallMesh, explodingBallMaterial),
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null),
			};
		}

		public Sprite Sprite(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.CollectibleMagnet => collectibleMagnetSprite,
				CollectibleType.HomingBall => homingBallSprite,
				CollectibleType.SuperThrow => superThrowSprite,
				CollectibleType.ExplodingBall => explodingBallSprite,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}

		public Sprite Icon(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.CollectibleMagnet => collectibleMagnetIcon,
				CollectibleType.HomingBall => homingBallIcon,
				CollectibleType.SuperThrow => superThrowIcon,
				CollectibleType.ExplodingBall => explodingBallIcon,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}
	}
}