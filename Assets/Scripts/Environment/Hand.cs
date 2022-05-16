using System;
using UnityEngine;

namespace Environment
{
	public class Hand : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float yMaxVal;
		[SerializeField] private bool followX;
		[SerializeField] private bool followHeight;

		[SerializeField] private Target target;
		[SerializeField] private HandType type;
		[SerializeField] private float positionFix;

		#endregion

		private Transform _objectToFollow;
		private SpriteRenderer _spriteRenderer;
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

		private enum Target
		{
			RightPlayer,
			LeftPlayer,
			Ball
		}

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_startingPos = transform.localPosition;
			_yMinVal = _startingPos.y;
		}

		private void Start()
		{
			_objectToFollow = target switch
			{
				Target.RightPlayer => GameManager.Players[0].transform,
				Target.LeftPlayer => GameManager.Players[1].transform,
				Target.Ball => ArenaManager.BallTransform,
				_ => _objectToFollow
			};
		}

		private void Update()
		{
			if (_spriteRenderer.enabled && !_objectToFollow.gameObject.activeSelf)
				_spriteRenderer.enabled = false;
			if (!_spriteRenderer.enabled && _objectToFollow.gameObject.activeSelf)
				_spriteRenderer.enabled = true;
		}

		private void FixedUpdate()
		{
			var pos = _startingPos;
			var objectPos = _objectToFollow.position;
			var currMaxY = followHeight ? yMaxVal + objectPos.y : yMaxVal;
			if (followX)
			{
				pos.x = objectPos.x + positionFix;
				var t = Math.Abs(_borderValues[(int) type] - objectPos.z);
				if (t < 5)
					pos.y = Mathf.Lerp(currMaxY, _yMinVal, t / 5);
			}
			else
			{
				pos.z = objectPos.z + positionFix;
				var t = Math.Abs(_borderValues[(int) type] - objectPos.x);
				if (t < 3)
					pos += transform.rotation * Vector3.up * Mathf.Lerp(currMaxY, _yMinVal, t / 3);
			}

			transform.localPosition = pos;
		}
	}
}