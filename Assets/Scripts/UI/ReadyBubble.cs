using UnityEngine;

namespace UI
{
	public class ReadyBubble : MonoBehaviour
	{
		[SerializeField] private Vector3 localPosition;
		[SerializeField] private Quaternion rotation;
		
		private Transform _parent;
		private Transform _transform;

		private void Awake()
		{
			_transform = transform;
			_parent = _transform.parent;
			_transform.rotation = rotation;
		}

		private void LateUpdate()
		{
			_transform.position = _parent.position + localPosition;
			_transform.rotation = rotation;
		}
	}
}