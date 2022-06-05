using UnityEngine;

public class Shadow : MonoBehaviour
{
	[SerializeField] private float modifier = 0.25f;
	[SerializeField] private float constant = 1.075f;
	[SerializeField] private bool staticObj;
	[SerializeField] private Vector3 staticOffset;

	private Transform _parent;
	private Transform _transform;
	private Quaternion _rotation;

	private void Awake()
	{
		_transform = transform;
		_parent = _transform.parent;
		_rotation = _transform.rotation;
		if (!staticObj) return;
		var pos = _parent.position;
		var objHeight = pos.y;
		pos.y = 0;
		_transform.position = pos + staticOffset;
		_transform.localScale = (modifier * objHeight + constant) * Vector3.one;
		_transform.rotation = _rotation;
		Destroy(this);
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