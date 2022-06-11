using System;
using System.Collections;
using UnityEngine;

namespace UI
{
	public class OpeningScreen : MonoBehaviour
	{
		[SerializeField] private float transitionTime = 0.5f;
		[SerializeField] private MinMax scale = new MinMax(1, 5);

		private readonly MinMax _alpha = new MinMax(0, 1);
		private RectTransform _rectTransform;
		private CanvasGroup _canvasGroup;
		private Vector3 _originalScale;

		[Serializable]
		private struct MinMax
		{
			public float min, max;

			public MinMax(float min, float max)
			{
				this.min = min;
				this.max = max;
			}
		}

		public bool Showing { get; private set; } = true;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_originalScale = _rectTransform.localScale;
		}

		public void Enter()
		{
			StartCoroutine(EnterScene());
		}

		public void Exit()
		{
			StartCoroutine(ExitScene());
		}

		private IEnumerator EnterScene()
		{
			for (var time = 0f; time < transitionTime; time += Time.deltaTime)
			{
				_canvasGroup.alpha = Mathf.Lerp(_alpha.max, _alpha.min, time / transitionTime);
				_rectTransform.localScale = _originalScale * Mathf.Lerp(scale.min, scale.max, time / transitionTime);
				yield return null;
			}

			_canvasGroup.alpha = 0;
			Showing = false;
		}

		private IEnumerator ExitScene()
		{
			for (var time = 0f; time < transitionTime; time += Time.deltaTime)
			{
				_canvasGroup.alpha = Mathf.Lerp(_alpha.min, _alpha.max, time / transitionTime);
				_rectTransform.localScale = _originalScale * Mathf.Lerp(scale.max, scale.min, time / transitionTime);
				yield return null;
			}

			_canvasGroup.alpha = 1;
			Showing = true;
		}
	}
}