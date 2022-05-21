using Managers;
using Player;
using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float dangerZoneRadius = 1;
		[SerializeField] private float baseSpeed = 1;
		[SerializeField] private float baseYSpeed = 2;
		[SerializeField] private float maxHeight = 5;
		[SerializeField] private float maxXDistFromMonster = 1;
		[SerializeField] private Transform divider;
		[SerializeField] private bool constantSpeeds;
		[SerializeField] private float timeToSpeedUp = 60;
		[SerializeField] private float speedUpMultiplier = 3;
		[SerializeField] private float colliderEnableDistance = 1;

		#endregion;

		#region Private Fields

		private LineRenderer _lineRenderer;
		private Collider _collider;
		private Animator _animator;

		private Transform _ball;
		private float _fixedBaseSpeed;
		private float _fixedBaseYSpeed;
		private float _startTime;
		private float _speed = 1;
		private float _ySpeed = 0.5f;

		private static readonly int AnimatorX = Animator.StringToHash("X");
		private static readonly int AnimatorZ = Animator.StringToHash("Z");

		#endregion

		#region Function Events

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_collider = GetComponent<Collider>();
			_animator = GetComponent<Animator>();
			_lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, Vector3.zero);
			_startTime = Time.time;
			OnValidate();
			enabled = false;
		}

		private void Start()
		{
			_ball = MatchManager.BallTransform;
		}

		private void OnValidate()
		{
			_fixedBaseSpeed = baseSpeed * Time.fixedDeltaTime;
			_fixedBaseYSpeed = baseYSpeed * Time.fixedDeltaTime;
		}

		private void OnBecameVisible()
		{
			enabled = true;
		}

		private void Update()
		{
			if (constantSpeeds) return;
			// ReSharper disable once PossibleLossOfFraction
			var speedMultiplier = (int)((Time.time - _startTime) / timeToSpeedUp)*speedUpMultiplier + 1; // TODO keep looking at values
			_speed = _fixedBaseSpeed * speedMultiplier;
			_ySpeed = _fixedBaseYSpeed * speedMultiplier;
		}

		private void FixedUpdate()
		{
			Move();
			UpdateSpectralChain();
		}

		private void OnTriggerEnter(Collider other)
		{
			other.gameObject.SetActive(false);
			MatchManager.GameOver(transform.position.x > divider.position.x);
		}

		#endregion

		#region Private Methods

		private void Move()
		{
			var pos = transform.position;
			var ballPos = _ball.position;
			var dividerPos = divider.position;
			var ballDir = ballPos - pos;
			UpdateAnimator(ballDir);
			_collider.enabled = ballDir.sqrMagnitude < colliderEnableDistance;
			var ballDist = ballPos.x - pos.x;
			var xMovement = Mathf.Abs(ballDist) > 0.01f ? Mathf.Sign(ballDist) * _speed : 0;
			var yMovement = Mathf.Max(Mathf.Abs(ballDir.x), Mathf.Abs(ballDir.z)) < dangerZoneRadius
				? ballDir.y * _ySpeed
				: Mathf.Min(maxHeight - pos.y, 2 * _ySpeed);
			var zMovement = ballDir.z * _speed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) > 0;
			if (movingTowardsDivider)
				xMovement *= 2;
			var movement = new Vector3(xMovement, yMovement, zMovement);
			pos += movement;
			transform.position = pos;
			if (Mathf.Abs(pos.x - dividerPos.x) < maxXDistFromMonster || movingTowardsDivider) return;
			dividerPos.x += xMovement;
			divider.position = dividerPos;
		}

		private void UpdateAnimator(Vector3 direction)
		{
			direction.y = 0;
			direction = direction.normalized;
			var z = Mathf.Round(direction.z);
			var x = Mathf.Round(direction.x);
			if (x == 0 && z == 0) return;
			_animator.SetFloat(AnimatorZ, z);
			if (z > 0)
				_animator.SetFloat(AnimatorX, direction.x > 0 ? 1 : -1);
			else
				_animator.SetFloat(AnimatorX, x);
		}

		private void UpdateSpectralChain()
		{
			var closestDividerPoint = divider.position;
			closestDividerPoint.z = transform.position.z;
			_lineRenderer.SetPosition(1, transform.InverseTransformPoint(closestDividerPoint));
		}

		#endregion
	}
}