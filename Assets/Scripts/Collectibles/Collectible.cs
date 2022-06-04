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

		public CollectibleType CollectibleType
		{
			set
			{
				_collectible = collectibleFactory.Create(value);
				GetComponent<SpriteRenderer>().sprite = collectibleFactory.Sprite(value);
			}
		}

		[SerializeField] private CollectibleFactory collectibleFactory;

		private ICollectible _collectible;

		private void OnValidate() => CollectibleType = collectibleType;

		private void Start()
		{
			OnValidate();
			if (GameManager.CurScene != 0)
			{
				MatchManager.AddToCollectibleCollection(transform);				
			}
		}

		private void OnDisable()
		{
			if (GameManager.CurScene!= 0)
			{
				MatchManager.RemoveFromCollectibleCollection(transform);
			}
			
		}

		private void OnTriggerEnter(Collider other)
		{
			_collectible.Collect(other.gameObject);
			if (_collectible is PowerUp.PowerUp powerUp)
				other.GetComponent<PlayerBrain>()?.SetPowerUp(powerUp);
			if(GameManager.CurScene != 0)
				Destroy(gameObject);
		}
	}
}