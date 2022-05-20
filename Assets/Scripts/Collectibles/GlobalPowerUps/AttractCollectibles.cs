using Managers;
using UnityEngine;

namespace Collectibles.GlobalPowerUps
{
	public class AttractCollectibles : IGlobalPowerUp
	{
		private Transform _attractingPlayer;
		private readonly float _duration;
		private readonly float _baseAttractionSpeed;
		private readonly float _attractionSqrRadius;

		public AttractCollectibles(float duration, float baseAttractionSpeed, float attractionRadius)
		{
			_duration = duration;
			_baseAttractionSpeed = baseAttractionSpeed;
			_attractionSqrRadius = Mathf.Pow(attractionRadius,2);
		}

		public void Collect(GameObject collector)
		{
			_attractingPlayer = collector.transform;
		}

		public float StartAndGetDuration()
		{
			return _duration;
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

		public void End()
		{
		}
	}
}