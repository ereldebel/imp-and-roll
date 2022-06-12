using System.Collections.Generic;
using Collectibles;
using Collectibles.PowerUp;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class PowerUpIcon : MonoBehaviour
	{
		[SerializeField] private int playerNumber;
		[SerializeField] private CollectibleFactory collectibleFactory;

		private Image _image;
		private readonly List<CollectibleType> _activatedPowerUps = new List<CollectibleType>();
		private GameObject _owner;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		private void OnEnable()
		{
			_image.enabled = false;
			_owner = GameManager.Players[playerNumber];
			PowerUp.PowerUpActivated += PowerUpActivated;
			PowerUp.PowerUpDeactivated += PowerUpDeactivated;
		}

		private void OnDisable()
		{
			PowerUp.PowerUpActivated -= PowerUpActivated;
			PowerUp.PowerUpDeactivated -= PowerUpDeactivated;
		}

		private void PowerUpActivated(GameObject player, CollectibleType type)
		{
			if (player != _owner) return;
			if (_activatedPowerUps.Count == 0)
				_image.enabled = true;
			_image.sprite = collectibleFactory.Sprite(type);
			_activatedPowerUps.Add(type);
		}

		private void PowerUpDeactivated(GameObject player, CollectibleType type)
		{
			if (player != _owner) return;
			_activatedPowerUps.Remove(type);
			if (_activatedPowerUps.Count == 0)
				_image.enabled = false;
		}
	}
}