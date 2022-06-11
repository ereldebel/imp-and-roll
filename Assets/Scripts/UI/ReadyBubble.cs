using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class ReadyBubble : MonoBehaviour
	{
		[SerializeField] private Vector3 localPosition;
		[SerializeField] private Quaternion rotation;
		
		private Transform _parent;

		private void Awake()
		{
			_parent = transform.parent;
			transform.SetParent(null);
			transform.rotation = rotation;
			SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
		}

		private void LateUpdate()
		{
			if (!transform)
				Destroy(gameObject);
			transform.position = _parent.position + localPosition;
		}
	}
}