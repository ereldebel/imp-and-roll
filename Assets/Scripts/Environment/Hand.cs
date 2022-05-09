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
		[SerializeField] private HandType type;
		[SerializeField] private float positionFix;

		#endregion

		private Vector3 _startingPos;
		private float _yMinVal;
		private readonly float[] _borderValues = {7, -8, -11.5f, 11.5f};

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
			_yMinVal = _startingPos.y;
		}

		private void FixedUpdate()
		{
			var pos = _startingPos;
			var objectPos = objectToFollow.transform.position;
			if (followX)
			{
				pos.x = objectPos.x + positionFix;
				var t = Math.Abs(_borderValues[(int) type] - objectPos.z);
				if (t < 5)
					pos.y = Mathf.Lerp(yMaxVal, _yMinVal, t / 5);
			}
			else
			{
				pos.z = objectPos.z + positionFix;
				var t = Math.Abs(_borderValues[(int) type] - objectPos.x);
				if (t < 3)
					pos += transform.rotation * Vector3.up * Mathf.Lerp(yMaxVal, _yMinVal, t / 3);
			}

			transform.localPosition = pos;
		}
	}
}