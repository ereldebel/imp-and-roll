using System;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using UnityEngine;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Invert Controls")] [SerializeField]
		private float invertControlsDuration = 10;

		[Header("Attract Collectibles")] [SerializeField]
		private float attractCollectiblesDuration = 10;
		[SerializeField] private float attractCollectiblesBaseAttractionSpeed = 0.1f;
		[SerializeField] private float attractCollectiblesAttractionRadius = 10;

		[Header("Homing Ball Collectibles")] [SerializeField]
		private float homingBallDuration = 10;
		[SerializeField] private float homingBallAttractionRate = 1;

		[Space(2)] [Tooltip("if positive overrides all durations to this duration")] [SerializeField]
		private float uniformDuration = -1;

		private void OnValidate()
		{
			if (uniformDuration <= 0) return;
			invertControlsDuration = uniformDuration;
			attractCollectiblesDuration = uniformDuration;
			homingBallDuration = uniformDuration;
		}

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
	}
}