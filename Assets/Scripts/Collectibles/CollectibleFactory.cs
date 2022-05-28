﻿using System;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using UnityEngine;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Attract Collectibles")] [SerializeField]
		private float attractCollectiblesBaseAttractionSpeed = 0.1f;

		[SerializeField] private float attractCollectiblesAttractionRadius = 10;
		[SerializeField] private Sprite attractCollectiblesIcon;

		[Header("Homing Ball Collectibles")] [SerializeField]
		private float homingBallAttractionRate = 1;

		[SerializeField] private Mesh homingBallMesh;
		[SerializeField] private Material homingBallMaterial;
		[SerializeField] private Sprite homingBallIcon;

		[Header("Attract Collectibles")] [SerializeField]
		private float superThrowSpeedBoost = 1.1f;

		[SerializeField] private Mesh superThrowMesh;
		[SerializeField] private Material superThrowMaterial;
		[SerializeField] private Sprite superThrowIcon;

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
				CollectibleType.AttractCollectibles => new AttractCollectibles(attractCollectiblesBaseAttractionSpeed,
					attractCollectiblesAttractionRadius),
				CollectibleType.HomingBall => new HomingBall(homingBallAttractionRate, homingBallMesh,
					homingBallMaterial),
				CollectibleType.SuperThrow => new SuperThrow(superThrowSpeedBoost, superThrowMesh, superThrowMaterial),
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null),
			};
		}

		public Sprite Icon(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.AttractCollectibles => attractCollectiblesIcon,
				CollectibleType.HomingBall => homingBallIcon,
				CollectibleType.SuperThrow => superThrowIcon,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}
	}
}