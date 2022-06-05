using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
	public class SceneTransitioner : MonoBehaviour
	{
		[SerializeField] private Image transitionScreen;
		[SerializeField] private float transitionSpeed = 0.5f;

		public void TransitionToScene(string sceneName)
		{
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneName)));
		}

		public void TransitionToScene(int sceneBuildIndex)
		{
			StartCoroutine(PerformTransition(SceneManager.LoadSceneAsync(sceneBuildIndex)));
		}

		private IEnumerator PerformTransition(AsyncOperation asyncLoad)
		{
			transitionScreen.gameObject.SetActive(true);
			var color = transitionScreen.color;
			while (!asyncLoad.isDone && color.a > 0)
			{
				color.a -= transitionSpeed * Time.deltaTime;
				transitionScreen.color = color;
				yield return null;
			}

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
					AudioManager.MatchMusic();
					break;
					
			}
			while (color.a < 1)
			{
				color.a += transitionSpeed * Time.deltaTime;
				transitionScreen.color = color;
				yield return null;
			}

			transitionScreen.gameObject.SetActive(false);
		}
	}
}