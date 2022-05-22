using System;
using UnityEngine;

namespace Collectibles.PowerUp
{
	public abstract class PowerUp : IPowerUp
	{
		private readonly CollectibleType _type;
		private readonly float _duration;
		private GameObject _target;

		public static event Action<GameObject, CollectibleType, float> PowerUpActivated;

		protected PowerUp(float duration, CollectibleType type)
		{
			_duration = duration;
			_type = type;
		}

		public virtual void Collect(GameObject target)
		{
			_target = target;
		}

		public virtual float StartAndGetDuration()
		{
			PowerUpActivated?.Invoke(_target, _type, _duration);
			return _duration;
		}
	}
}