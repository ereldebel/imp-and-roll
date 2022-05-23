using System;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using UnityEngine;
using Random = System.Random;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Invert Controls")] [SerializeField]
		private float invertControlsDuration = 10;

		[SerializeField] private Sprite invertControlsIcon;

		[Header("Attract Collectibles")] [SerializeField]
		private float attractCollectiblesDuration = 10;

		[SerializeField] private float attractCollectiblesBaseAttractionSpeed = 0.1f;
		[SerializeField] private float attractCollectiblesAttractionRadius = 10;
		[SerializeField] private Sprite attractCollectiblesIcon;

		[Header("Homing Ball Collectibles")] [SerializeField]
		private float homingBallDuration = 10;

		[SerializeField] private float homingBallAttractionRate = 1;
		[SerializeField] private Sprite homingBallIcon;

		// [Space(5)] [Tooltip("random weight of each power up to spawn by their order of appearance")] [SerializeField]
		// private float[] powerUpWeights = new float[3];

		[Space(10)] [Tooltip("if positive overrides all durations to this duration")] [SerializeField]
		private float uniformDuration = -1;


		// private Random _random = new Random();

		private void OnValidate()
		{
			if (uniformDuration <= 0) return;
			invertControlsDuration = uniformDuration;
			attractCollectiblesDuration = uniformDuration;
			homingBallDuration = uniformDuration;
		}

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
				CollectibleType.InvertControls => new InvertControls(invertControlsDuration),
				CollectibleType.AttractCollectibles => new AttractCollectibles(attractCollectiblesDuration,
					attractCollectiblesBaseAttractionSpeed, attractCollectiblesAttractionRadius),
				CollectibleType.HomingBall => new HomingBall(homingBallDuration, homingBallAttractionRate),
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}

		public Sprite Icon(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.InvertControls => invertControlsIcon,
				CollectibleType.AttractCollectibles => attractCollectiblesIcon,
				CollectibleType.HomingBall => homingBallIcon,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}
	}
}