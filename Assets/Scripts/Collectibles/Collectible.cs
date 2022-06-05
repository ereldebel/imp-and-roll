using Managers;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Collectibles
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class Collectible : MonoBehaviour
	{
		[SerializeField] private CollectibleType collectibleType;
		[SerializeField] private CollectibleFactory collectibleFactory;

		private ICollectible _collectible;
		private bool _inCollectibleCollection;

		public CollectibleType CollectibleType
		{
			set
			{
				collectibleType = value;
				_collectible = collectibleFactory.Create(value);
				GetComponent<SpriteRenderer>().sprite = collectibleFactory.Sprite(value);
			}
		}

		private void OnValidate() => CollectibleType = collectibleType;

		private void Start()
		{
			OnValidate();
			if (GameManager.CurScene == 0) return;
			MatchManager.AddToCollectibleCollection(transform);
			_inCollectibleCollection = true;
		}

		private void OnDisable()
		{
			if (_inCollectibleCollection)
				MatchManager.RemoveFromCollectibleCollection(transform);
		}

		private void OnTriggerEnter(Collider other)
		{
			_collectible.Collect(other.gameObject);
			if (_collectible is PowerUp.PowerUp powerUp)
				other.GetComponent<PlayerBrain>()?.SetPowerUp(powerUp);
			if (GameManager.CurScene != 0)
				Destroy(gameObject);
			else
				OnValidate();
		}
	}
}