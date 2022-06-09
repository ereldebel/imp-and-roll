using UnityEngine;

namespace UI
{
	public class UICanvas : MonoBehaviour
	{
		private static UICanvas _instance;
		private void Awake()
		{
			if (_instance)
				Destroy(_instance.gameObject);
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}
