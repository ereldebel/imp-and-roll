using Managers;
using Player;
using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float dangerZoneRadius = 1;
		[SerializeField] private float alarmZoneRadius = 2;
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
		private bool _gotEye = false;
		private Vector3 _originalPosition;

		private static readonly int AnimatorX = Animator.StringToHash("X");
		private static readonly int AnimatorSpotX = Animator.StringToHash("Spot X");
		private static readonly int AnimatorZ = Animator.StringToHash("Z");
		private static readonly int AnimatorDangerLevel = Animator.StringToHash("Danger Level");

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
			_originalPosition = transform.position;
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
			if (_gotEye && Vector3.Distance(transform.position, _originalPosition) < 0.1f)
				enabled = false;
			if (constantSpeeds) return;
			// ReSharper disable once PossibleLossOfFraction
			var speedMultiplier = Mathf.Log10(1+(int) ((Time.time - _startTime) / timeToSpeedUp)) 
				* speedUpMultiplier + 1; // TODO keep looking at values
			_speed = _fixedBaseSpeed * speedMultiplier;
			_ySpeed = _fixedBaseYSpeed * speedMultiplier;
		}

		private void FixedUpdate()
		{
			Move(_gotEye ? _originalPosition : _ball.position);
			UpdateSpectralChain();
		}

		private void OnTriggerEnter(Collider other)
		{
			var otherObject = other.gameObject;
			if (otherObject.CompareTag("Player") && !otherObject.GetComponent<PlayerBrain>().HasBall) return;
			_ball.gameObject.SetActive(false);
			MatchManager.GameOver(transform.position.x > divider.position.x);
			_gotEye = true;
		}

		#endregion

		#region Private Methods

		private void Move(Vector3 targetPos)
		{
			var pos = transform.position;
			var dividerPos = divider.position;
			var targetDir = targetPos - pos;
			var infNorm = Mathf.Max(Mathf.Abs(targetDir.x), Mathf.Abs(targetDir.z));
			var dangerous = infNorm < dangerZoneRadius;
			UpdateAnimator(targetDir, dangerous ? 2 : (infNorm < alarmZoneRadius ? 1 : 0));
			_collider.enabled = targetDir.sqrMagnitude < colliderEnableDistance;
			var xMovement = Mathf.Abs(targetDir.x) > 0.01f ? Mathf.Sign(targetDir.x) * _speed : 0;
			var yMovement = dangerous
				? targetDir.y * _ySpeed
				: Mathf.Min(maxHeight - pos.y, 2 * _ySpeed);
			var zMovement = targetDir.z * _speed;
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

		private void UpdateAnimator(Vector3 direction, int dangerLevel)
		{
			_animator.SetInteger(AnimatorDangerLevel, _gotEye ? 0 : dangerLevel);
			if (direction.x == 0 && direction.z == 0) return;
			direction.y = 0;
			direction = direction.normalized;
			var z = Mathf.Round(direction.z);
			var x = Mathf.Round(direction.x);
			var nonZeroX = direction.x > 0 ? 1 : -1;
			var nonZeroZ = direction.z > 0 ? 1 : -1;
			_animator.SetFloat(AnimatorZ, x == 0 ? nonZeroZ : z);
			_animator.SetFloat(AnimatorSpotX, nonZeroX);
			_animator.SetFloat(AnimatorX, z == 0 ? nonZeroX : x);
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