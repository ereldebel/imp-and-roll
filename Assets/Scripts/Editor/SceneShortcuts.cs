using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
	public class SceneShortcuts : MonoBehaviour
	{
		[MenuItem("Scenes/Play-Stop, From Opening Screen &#p")]
		public static void PlayFromOpening()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene("Assets/Scenes/Opening Screen.unity");
			EditorApplication.isPlaying = true;
		}

		[MenuItem("Scenes/To Game Scene #TAB")]
		public static void SwitchToGameScene()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
		}
	}
}