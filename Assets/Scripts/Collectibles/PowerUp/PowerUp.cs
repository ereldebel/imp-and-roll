using System;
using UnityEngine;

namespace Collectibles.PowerUp
{
	public abstract class PowerUp
	{
		private readonly CollectibleType _type;
		private GameObject _target;

		public static event Action<GameObject, CollectibleType> PowerUpActivated;
		public static event Action<GameObject, CollectibleType> PowerUpDeactivated;

		protected PowerUp(CollectibleType type)
		{
			_type = type;
		}

		public virtual void Collect(GameObject target)
		{
			_target = target;
		}

		protected void Start()
		{
			PowerUpActivated?.Invoke(_target, _type);
		}
		
		protected void End()
		{
			PowerUpDeactivated?.Invoke(_target, _type);
		}
	}
}