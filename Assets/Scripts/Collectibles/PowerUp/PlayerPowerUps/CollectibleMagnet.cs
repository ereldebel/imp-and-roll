﻿using Collectibles.PowerUp.GlobalPowerUps;
using Managers;
using UnityEngine;

namespace Collectibles.PowerUp.PlayerPowerUps
{
	public class CollectibleMagnet : PowerUp, IPlayerPowerUp
	{
		private Transform _attractingPlayer;
		private readonly float _baseAttractionSpeed;
		private readonly float _attractionSqrRadius;
		private const CollectibleType PowerUpType = CollectibleType.CollectibleMagnet;

		public CollectibleMagnet(float baseAttractionSpeed, float attractionRadius) : base(PowerUpType)
		{
			_baseAttractionSpeed = baseAttractionSpeed;
			_attractionSqrRadius = Mathf.Pow(attractionRadius, 2);
		}

		public override void Collect(GameObject collector)
		{
			base.Collect(collector);
			_attractingPlayer = collector.transform;
		}

		public void OnUpdate()
		{
			foreach (var collectible in MatchManager.CollectibleCollection)
			{
				var diff = _attractingPlayer.position - collectible.position;
				if (diff == Vector3.zero) break;
				var sqrNorm = diff.sqrMagnitude;
				if (sqrNorm >= _attractionSqrRadius) break;
				collectible.position += diff * (_baseAttractionSpeed / sqrNorm);
			}
		}
	}
}