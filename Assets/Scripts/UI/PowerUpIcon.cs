using System.Collections.Generic;
using Collectibles;
using Collectibles.PowerUp;
using UnityEngine;

namespace UI
{
	public class PowerUpIcon : MonoBehaviour
	{
		[SerializeField] private CollectibleFactory collectibleFactory;
		private SpriteRenderer _spriteRenderer;
		private Sprite _defaultSprite;
		private List<CollectibleType> _activatedPowerUps = new List<CollectibleType>();

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_defaultSprite = _spriteRenderer.sprite;
			PowerUp.PowerUpActivated += PowerUpActivated;
			PowerUp.PowerUpDeactivated += PowerUpDeactivated;
		}

		private void OnDestroy()
		{
			PowerUp.PowerUpActivated -= PowerUpActivated;
			PowerUp.PowerUpDeactivated -= PowerUpDeactivated;
		}

		private void PowerUpActivated(GameObject player, CollectibleType type)
		{
			if (player != transform.parent.gameObject) return;
			_spriteRenderer.sprite = collectibleFactory.Icon(type);
			_activatedPowerUps.Add(type);
		}

		private void PowerUpDeactivated(GameObject player, CollectibleType type)
		{
			if (player != transform.parent.gameObject) return;
			_activatedPowerUps.Remove(type);
			print($"removed {type}. {_activatedPowerUps.Count == 0}");
			if (_activatedPowerUps.Count == 0)
				_spriteRenderer.sprite = _defaultSprite;
		}
	}
}