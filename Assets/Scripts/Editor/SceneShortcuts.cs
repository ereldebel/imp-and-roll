using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
	public class SceneShortcuts : MonoBehaviour
	{
		private static int _scene;
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
			switch (_scene)
			{
				case 0:
					EditorSceneManager.OpenScene("Assets/Scenes/Original Arena.unity");
					break;
				case 1:
					EditorSceneManager.OpenScene("Assets/Scenes/Icy Arena.unity");
					break;
				case 2:
					EditorSceneManager.OpenScene("Assets/Scenes/Volcanic Arena.unity");
					break;
				case 3:
					EditorSceneManager.OpenScene("Assets/Scenes/Opening Screen.unity");
					break;
			}

			_scene = (_scene + 1) % 4;
		}
	}
}