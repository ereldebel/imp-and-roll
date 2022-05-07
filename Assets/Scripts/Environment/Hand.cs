using System;
using UnityEngine;

namespace Environment
{
	public class Hand : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float yMaxVal;
		[SerializeField] private bool followX;
		[SerializeField] private bool followZ;
		[SerializeField] private GameObject objectToFollow;
		[SerializeField] private string arenaMaterialBorderYValueName = "BorderY";

		#endregion

		private Vector3 _startingPos;
		private Material _handMaterial;
		private float _yMinVal;
		private int _shaderBorderYVar;
		private Collider _myCollider;

		private void Awake()
		{
			_startingPos = transform.position;
			_yMinVal = _startingPos.y;
			_handMaterial = GetComponent<Renderer>().material;
			_shaderBorderYVar = Shader.PropertyToID(arenaMaterialBorderYValueName);
			_myCollider = GetComponent<Collider>();
		}

		private void FixedUpdate()
		{
			var pos = _startingPos;
			if (followX)
			{
				pos.x = objectToFollow.transform.position.x;

				if (Math.Abs(transform.position.z - objectToFollow.transform.position.z) < 5)
				{
					// print(Math.Abs(transform.position.z - object_to_follow.transform.position.z));
					var t = Math.Abs(transform.position.z - objectToFollow.transform.position.z) / 5;
					pos.y = Mathf.Lerp(yMaxVal, _yMinVal, t);
					_handMaterial.SetFloat(_shaderBorderYVar, 1 - t);
				}
			}
			else
			{
				pos.z = objectToFollow.transform.position.z;
			}

			transform.position = pos;
		}
	}
}