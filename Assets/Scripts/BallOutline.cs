using System;
using TMPro;
using UnityEngine;

public class BallOutline : MonoBehaviour
{
	[SerializeField] private float width = 0.1f;
	private Transform _transform;
	private Transform _parentTransform;
	private float _targetScale;

	private void Awake()
	{
		OnValidate();
	}

	private void OnValidate()
	{
		_transform = transform;
		_parentTransform = _transform.parent;
		_targetScale = 1 + width;
		LateUpdate();
	}

	private void LateUpdate()
	{
		//TODO: should keep an eye on this transformation - it should work but documentation is worrisome
		_transform.localScale = Vector3.one;
		var lossyScale = _transform.lossyScale;
		var parentScale = _parentTransform.lossyScale;
		_transform.localScale = new Vector3(_targetScale * parentScale.x / lossyScale.x,
			_targetScale * parentScale.y / lossyScale.y,
			_targetScale * parentScale.z / lossyScale.z);
	}
}