using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
	public class SceneShortcuts : MonoBehaviour
	{
		private static int _scene;
		private static List<string> _scenes;

		[MenuItem("Scenes/Play-Stop, From Opening Screen &#p")]
		public static void PlayFromOpening()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene("Assets/Scenes/Opening Scene.unity");
			EditorApplication.isPlaying = true;
		}

		[MenuItem("Scenes/To Game Scene #TAB")]
		public static void SwitchToGameScene()
		{
			if (EditorApplication.isPlaying)
			{
				EditorApplication.isPlaying = false;
			}

			_scenes = new List<string>(Directory.GetFiles("Assets/Scenes")
				.Where(filename => filename.EndsWith(".unity")));
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(_scenes[_scene]);
			_scene = (_scene + 1) % _scenes.Count;
		}
	}
}