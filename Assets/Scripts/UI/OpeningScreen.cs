using System;
using System.Collections;
using Managers;
using Player;
using UnityEngine;

namespace UI
{
	public class OpeningScreen : MonoBehaviour
	{
		[SerializeField] private float transitionTime = 0.5f;
		[SerializeField] private MinMax scale = new MinMax(1, 5);
		[SerializeField] private float interval = 30;
		[SerializeField] private AudioSource fireSound;

		private readonly MinMax _alpha = new MinMax(0, 1);
		private RectTransform _rectTransform;
		private CanvasGroup _canvasGroup;
		private Vector3 _originalScale;
		private Coroutine _timer;

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

		private void OnDestroy()
		{
			if (_timer != null)
				StopCoroutine(_timer);
		}

		public void Enter()
		{
			StartCoroutine(EnterScene());
			AudioManager.TutorialScreenMusic();
			if (_timer != null)
				StopCoroutine(_timer);
			_timer = StartCoroutine(Timer());
			fireSound.Play();
		}

		private IEnumerator Timer()
		{
			while (true)
			{
				yield return new WaitForSeconds(interval);
				if (!PlayerController.Dirty)
					GameManager.Shared.ExitToOpeningScreen();
				PlayerController.Dirty = false;
			}
		}

		public void Exit()
		{
			StartCoroutine(ExitScene());
			AudioManager.OpeningScreenMusic();
			StopCoroutine(_timer);
			fireSound.Stop();
		}

		private IEnumerator EnterScene()
		{
			yield return null;
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
			yield return null;
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