﻿using System;
using Collectibles.PowerUp.BallPowerUps;
using UnityEngine;

namespace Collectibles
{
	[Serializable]
	[CreateAssetMenu(fileName = "CollectibleFactory", menuName = "Create Collectible Factory")]
	public class CollectibleFactory : ScriptableObject
	{
		[Header("Homing Ball Collectibles")] [SerializeField]
		private float homingRate = 1;

		[SerializeField] private Mesh homingBallMesh;
		[SerializeField] private Material[] homingBallMaterials;
		[SerializeField] private Sprite homingBallSprite;
		[SerializeField] private Sprite homingBallIcon;

		[Header("Super Throw")] [SerializeField]
		private float superThrowSpeedBoost = 1.1f;

		[SerializeField] private Mesh superThrowMesh;
		[SerializeField] private Material[] superThrowMaterials;
		[SerializeField] private Sprite superThrowSprite;
		[SerializeField] private Sprite superThrowIcon;

		[Header("Exploding Ball")] [SerializeField]
		public float explosionStunRadius;

		[SerializeField] private float explosionKnockBackOuterRingWidth;
		[SerializeField] private float knockBackVelocityMultiplier;
		[SerializeField] private LayerMask playerLayerMask;
		[SerializeField] private GameObject explosionPrefab;
		[SerializeField] private GameObject sparksPrefab;
		[SerializeField] private Mesh explodingBallMesh;
		[SerializeField] private Material[] explodingBallMaterials;
		[SerializeField] private Sprite explodingBallSprite;
		[SerializeField] private Sprite explodingBallIcon;

		[Header("Fire Trail")] [SerializeField]
		public float fireTrailParticleLifeSpan = 10;

		[SerializeField] private GameObject fireDropperPrefab;
		[SerializeField] private Mesh fireTrailMesh;
		[SerializeField] private Material[] fireTrailMaterials;
		[SerializeField] private Sprite fireTrailSprite;
		[SerializeField] private Sprite fireTrailIcon;

		public ICollectible Create(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.HomingBall => new HomingBall(homingRate, homingBallMesh,
					homingBallMaterials),
				CollectibleType.SuperThrow => new SuperThrow(superThrowSpeedBoost, superThrowMesh, superThrowMaterials),
				CollectibleType.ExplodingBall => new ExplodingBall(
					explosionStunRadius + explosionKnockBackOuterRingWidth, explosionStunRadius,
					knockBackVelocityMultiplier, playerLayerMask, explosionPrefab, sparksPrefab, explodingBallMesh,
					explodingBallMaterials),
				CollectibleType.FireTrail => new FireTrail(fireDropperPrefab, fireTrailMesh, fireTrailMaterials),
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null),
			};
		}

		public Sprite Sprite(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.HomingBall => homingBallSprite,
				CollectibleType.SuperThrow => superThrowSprite,
				CollectibleType.ExplodingBall => explodingBallSprite,
				CollectibleType.FireTrail => fireTrailSprite,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}

		public Sprite Icon(CollectibleType collectibleType)
		{
			return collectibleType switch
			{
				CollectibleType.HomingBall => homingBallIcon,
				CollectibleType.SuperThrow => superThrowIcon,
				CollectibleType.ExplodingBall => explodingBallIcon,
				CollectibleType.FireTrail => fireTrailIcon,
				_ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
			};
		}
	}
}