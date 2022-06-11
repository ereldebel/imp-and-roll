using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
	public class SceneTransitioner : MonoBehaviour
	{
		[SerializeField] private float transitionSpeed = 0.5f;
		[SerializeField] private Sprite[] tips;
		[SerializeField] private Image tipImage;

		private static int _tipIndex;
		private CanvasGroup transitionScreen;

		private void Awake()
		{
			transitionScreen = GetComponent<CanvasGroup>();
		}

		public void QuickTransitionToScene(string sceneName) =>
			StartCoroutine(PerformQuickTransition(SceneManager.LoadSceneAsync(sceneName)));

		public void QuickTransitionToScene(int sceneBuildIndex) =>
			StartCoroutine(PerformQuickTransition(SceneManager.LoadSceneAsync(sceneBuildIndex)));

		public void TransitionToScene(string sceneName) =>
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneName), false, null));

		public void TransitionToScene(int sceneBuildIndex) =>
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneBuildIndex), false, null));

		public void TransitionToScene(string sceneName, Action preSceneOrganizing) =>
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneName), true, preSceneOrganizing));

		public void TransitionToScene(int sceneBuildIndex, Action preSceneOrganizing) =>
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneBuildIndex), true, preSceneOrganizing));

		private static IEnumerator PerformQuickTransition(AsyncOperation asyncLoad)
		{
			while (!asyncLoad.isDone)
				yield return null;
			SwitchMusicByScene();
		}
		
		private IEnumerator PerformTransition(AsyncOperation asyncLoad, bool needsOrganizing, Action preSceneOrganizing)
		{
			tipImage.sprite = tips[_tipIndex++];
			_tipIndex %= tips.Length;
			float alpha = 0;
			transitionScreen.alpha = alpha;
			while (alpha < 1)
			{
				alpha += transitionSpeed * Time.deltaTime;
				transitionScreen.alpha = alpha;
				yield return null;
			}

			transitionScreen.alpha = alpha = 1;
			if (needsOrganizing)
				preSceneOrganizing();
			while (!asyncLoad.isDone)
				yield return null;
			SwitchMusicByScene();

			while (alpha > 0)
			{
				alpha -= transitionSpeed * Time.deltaTime;
				transitionScreen.alpha = alpha;
				yield return null;
			}

			transitionScreen.alpha = 0;
		}

		private static void SwitchMusicByScene()
		{
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
		}
	}
}