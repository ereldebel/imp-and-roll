using UnityEngine;

namespace UI
{
	public class UICanvas : MonoBehaviour
	{
		public static UICanvas instance;
		private void Awake()
		{
			if (instance)
				Destroy(instance.gameObject);
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}
