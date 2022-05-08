using System;
using UnityEngine;

namespace Environment
{
	public class Hand : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float yMaxVal;
		[SerializeField] private bool followX;
		[SerializeField] private GameObject objectToFollow;
		// [SerializeField] private string textureMaterialValueName = "BorderY";
		[SerializeField] private HandType type;
		
		#endregion

		private Vector3 _startingPos;
		private Material _handMaterial;
		private float _yMinVal;
		private int _shaderBorderYVar;
		private Collider _myCollider;
		private float[] borderValues = {7,-8,11.5f,-15.5f};
		private enum HandType
		{
			Top = 0,
			Bottom = 1,
			Left = 2,
			Right = 3
		}

		private void Awake()
		{
			_startingPos = transform.localPosition;
			_yMinVal = _startingPos.z;
			_handMaterial = GetComponent<Renderer>().material;
			_myCollider = GetComponent<Collider>();
			print(_startingPos);
		}

		private void FixedUpdate()
		{
			var pos = _startingPos;
			if (followX)
			{
				pos.x = objectToFollow.transform.position.x;
				var t = Math.Abs(borderValues[(int)type] - objectToFollow.transform.position.z);
				if (t < 5)
				{
					pos.z = Mathf.Lerp(yMaxVal, _yMinVal, t/5);
				}
			}
			else
			{
				pos.z = objectToFollow.transform.position.z;
				pos.y = -objectToFollow.transform.position.z;
				var t = Math.Abs(borderValues[(int)type] - objectToFollow.transform.position.x);
				if (t < 3)
				{
					pos.y = Mathf.Lerp(yMaxVal, _yMinVal, t/3);
				}
			}

			transform.localPosition = pos;
		}
	}
}