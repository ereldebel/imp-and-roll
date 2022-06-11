using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class SceneTransitioner : MonoBehaviour
	{
		[SerializeField] private float transitionSpeed = 0.5f;
		private CanvasGroup transitionScreen;

		private void Awake()
		{
			transitionScreen = GetComponent<CanvasGroup>();
		}

		public void TransitionToScene(string sceneName, Action preSceneOrganizing = null)
		{
			StartCoroutine(PerformTransition(() => SceneManager.LoadSceneAsync(sceneName), preSceneOrganizing));
		}

		public void TransitionToScene(int sceneBuildIndex, Action preSceneOrganizing = null)
		{
			StartCoroutine(PerformTransition(() => SceneManager.LoadSceneAsync(sceneBuildIndex), preSceneOrganizing));
		}

		private IEnumerator PerformTransition(Func<AsyncOperation> asyncLoadFunc, Action preSceneOrganizing)
		{
			float alpha = 0;
			transitionScreen.alpha = alpha;
			while (alpha < 1)
			{
				alpha += transitionSpeed * Time.deltaTime;
				transitionScreen.alpha = alpha;
				yield return null;
			}

			transitionScreen.alpha = alpha = 1;
			var asyncLoad = asyncLoadFunc();
			preSceneOrganizing?.Invoke();
			while (!asyncLoad.isDone)
				yield return null;
			var sceneIndex = SceneManager.GetActiveScene().buildIndex;
			switch (sceneIndex)
			{
				case 0:
					AudioManager.StartScreenMusic();
					break;
				case var _ when sceneIndex > 3:
					AudioManager.WinMusic();
					break;
				default:
					AudioManager.MatchStart();
					AudioManager.MatchMusic();
					break;
			}

			while (alpha > 0)
			{
				alpha -= transitionSpeed * Time.deltaTime;
				transitionScreen.alpha = alpha;
				yield return null;
			}

			transitionScreen.alpha = 0;
		}
	}
}