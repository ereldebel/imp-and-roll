using System.Collections;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps.Effects
{
	public class ExplosionShadow : MonoBehaviour
	{
		[SerializeField] private CollectibleFactory collectibleFactory;
		[SerializeField] private float scaleFixedStepSize = 0.5f;
		[SerializeField] private float secondStageScaleFixedStepSize = 0.1f;
		[SerializeField] private float fadeFixedStepSize = 0.01f;

		private SpriteRenderer _spriteRenderer;

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void Start()
		{
			StartCoroutine(Expand());
		}

		private IEnumerator Expand()
		{
			var maxRadius = collectibleFactory.explosionStunRadius;
			var size = scaleFixedStepSize;
			for (; size <= maxRadius; size += scaleFixedStepSize)
			{
				transform.localScale = new Vector3(size, size, size);
				yield return new WaitForFixedUpdate();
			}

			for (var opacity = _spriteRenderer.color.a;
			     opacity > 0;
			     opacity -= fadeFixedStepSize, size += secondStageScaleFixedStepSize)
			{
				var color = _spriteRenderer.color;
				color.a = opacity;
				_spriteRenderer.color = color;
				transform.localScale = new Vector3(size, size, size);
				yield return new WaitForFixedUpdate();
			}

			Destroy(gameObject);
		}
	}
}