using System;
using System.Collections;
using Managers;
using Player;
using UnityEngine;

namespace Collectibles
{
	[RequireComponent(typeof(Collider))]
	[RequireComponent(typeof(SpriteRenderer))]
	public class Collectible : MonoBehaviour
	{
		[SerializeField] private CollectibleType collectibleType;
		[SerializeField] private CollectibleFactory collectibleFactory;
		[SerializeField] private float transitionSpeed = 5;

		private ICollectible _collectible;
		private bool _inCollectibleCollection;
		private Coroutine _disappearance;

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
			_disappearance = GameManager.Shared.StartCoroutine(Disappear());
		}

		private void OnDestroy()
		{
			if (_disappearance != null)
				StopCoroutine(_disappearance);
		}

		private IEnumerator Disappear()
		{
			var scale = transform.localScale;
			for (float scaleFactor = 1; scaleFactor > 0; scaleFactor -= transitionSpeed * Time.deltaTime)
			{
				transform.localScale = scale * scaleFactor;
				yield return null;
			}

			if (GameManager.CurScene != 0)
			{
				Destroy(gameObject);
				yield break;
			}

			transform.localScale = scale;
			yield return WaitAndRespawn(2f);
		}

		private IEnumerator WaitAndRespawn(float delay)
		{
			print("started");
			gameObject.SetActive(false);
			yield return new WaitForSeconds(delay);
			gameObject.SetActive(true);
			OnValidate();
		}
	}
}