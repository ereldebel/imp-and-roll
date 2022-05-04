using UnityEngine;

public class Shadow : MonoBehaviour
{
	[SerializeField] private float modifier = 0.25f;
	[SerializeField] private float constant = 1.075f;

	private Transform _parent;
	private Transform _transform;
	private Quaternion _rotation;

	private void Awake()
	{
		_transform = transform;
		_parent = _transform.parent;
		_rotation = _transform.rotation;
	}

	private void LateUpdate()
	{
		var pos = _parent.position;
		var objHeight = pos.y;
		pos.y = 0;
		_transform.position = pos;
		_transform.localScale = (modifier * objHeight + constant) * Vector3.one;
		_transform.rotation = _rotation;
	}
}