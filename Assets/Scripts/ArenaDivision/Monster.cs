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
		[SerializeField] private float timeToSpeedUp = 60;
		[SerializeField] private float speedUpMultiplier = 3;
		[SerializeField] private float colliderEnableDistance = 1;
		[SerializeField] private float turnAroundSpeed = 0.5f;

		#endregion;

		#region Private Fields

		private LineRenderer _lineRenderer;
		private Collider _collider;
		private Animator _animator;
		private MonsterAudio _audio;

		private Transform _ball;
		private Transform _dividerChild;

		private Vector3 _originalPosition;

		private float _fixedBaseSpeed;
		private float _fixedBaseYSpeed;
		private float _speed = 1;
		private float _ySpeed = 0.5f;

		private float _startTime;
		private int _timeStep = 1;

		private bool _gotEye;
		private bool _dangerous;
		private int _dangerLevel;
		private bool _spinningLeft = true;
		private bool _static = true;

		private static readonly int AnimatorZ = Animator.StringToHash("Z");
		private static readonly int AnimatorX = Animator.StringToHash("X");
		private static readonly int AnimatorSpotX = Animator.StringToHash("Spot X");
		private static readonly int AnimatorAttackX = Animator.StringToHash("Attack X");
		private static readonly int AnimatorDangerLevel = Animator.StringToHash("Danger Level");
		private static readonly int AnimatorAccelerate = Animator.StringToHash("Accelerate");

		#endregion

		#region Function Events

		private void Awake()
		{
			_lineRenderer = GetComponent<LineRenderer>();
			_collider = GetComponent<Collider>();
			_animator = GetComponent<Animator>();
			_audio = GetComponent<MonsterAudio>();
			_lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, Vector3.zero);
			_startTime = Time.time;
			_originalPosition = transform.position;
			_dividerChild = divider.GetChild(0);
			OnValidate();
			UpdateSpectralChain();
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
			var speedMultiplier = Mathf.Log10(_timeStep) * speedUpMultiplier + 1;
			_speed = _fixedBaseSpeed * speedMultiplier;
			_ySpeed = _fixedBaseYSpeed * speedMultiplier;
		}

		private void OnBecameVisible()
		{
			enabled = true;
			_audio.Sneeze();
			Invoke(nameof(Continue), 2);
		}

		private void Update()
		{
			if (_gotEye && Vector3.Distance(transform.position, _originalPosition) < 0.1f)
				enabled = false;
			if (_dangerous || Time.time - _startTime < timeToSpeedUp * _timeStep) return;
			_static = true;
			_audio.Accelerate();
			_animator.SetTrigger(AnimatorAccelerate);
			++_timeStep;
			var speedMultiplier = Mathf.Log10(_timeStep) * speedUpMultiplier + 1;
			_speed = _fixedBaseSpeed * speedMultiplier;
			_ySpeed = _fixedBaseYSpeed * speedMultiplier;
		}

		private void FixedUpdate()
		{
			if (_static) return;
			if (_ball.position.y >= transform.position.y && !_gotEye)
				return;
			Move(_gotEye ? _originalPosition : _ball.position);
			UpdateSpectralChain();
		}

		private void OnTriggerEnter(Collider other)
		{
			var otherObject = other.gameObject;
			if (otherObject.transform.position.y > 1.5f) return;
			if (other.gameObject != _ball.gameObject && !otherObject.GetComponent<PlayerBrain>().TakeHitFromMonster())
				return;
			_ball.gameObject.SetActive(false);
			MatchManager.GameOver(transform.position.x > divider.position.x);
			_gotEye = true;
		}

		#endregion

		#region Private Methods

		private void Continue()
		{
			_static = false;
		}

		private void Move(Vector3 targetPos)
		{
			var pos = transform.position;
			var dividerPos = divider.position;
			var targetDir = targetPos - pos;
			var infNorm = Mathf.Max(Mathf.Abs(targetDir.x), Mathf.Abs(targetDir.z));
			_dangerous = infNorm < dangerZoneRadius;
			UpdateAnimator(targetDir, _dangerous ? 2 : (infNorm < alarmZoneRadius ? 1 : 0));
			_collider.enabled = targetDir.sqrMagnitude < colliderEnableDistance;
			var xMovement = Mathf.Abs(targetDir.x) > 0.01f ? Mathf.Sign(targetDir.x) * _speed : 0;
			var yMovement = _dangerous ? targetDir.y * _ySpeed : Mathf.Min(maxHeight - pos.y, 2 * _ySpeed);
			var zMovement = targetDir.z * _speed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) > 0;
			if (movingTowardsDivider)
				xMovement *= turnAroundSpeed;
			var movement = new Vector3(xMovement, yMovement, zMovement);
			pos += movement;
			transform.position = pos;
			if (Mathf.Abs(pos.x - dividerPos.x) < maxXDistFromMonster || movingTowardsDivider) return;
			// If the barrier is now pulled to the other direction
			if ((_spinningLeft && pos.x > dividerPos.x) || (!_spinningLeft && pos.x < dividerPos.x))
				SpinDivider();
			dividerPos.x += xMovement;
			divider.position = dividerPos;
		}

		private void SpinDivider()
		{
			var dividerChildLocalScale = _dividerChild.localScale;
			dividerChildLocalScale.x = -dividerChildLocalScale.x;
			_dividerChild.localScale = dividerChildLocalScale;
			_spinningLeft = !_spinningLeft;
		}

		private void UpdateAnimator(Vector3 direction, int dangerLevel)
		{
			if (_dangerLevel != dangerLevel && !_gotEye)
			{
				if (_dangerLevel == 0)
					_audio.Attack();
				else if (dangerLevel == 0)
					_audio.Stop();
			}

			_dangerLevel = dangerLevel;
			_animator.SetInteger(AnimatorDangerLevel, _gotEye ? 0 : dangerLevel);
			if (direction.x == 0 && direction.z == 0) return;
			direction.y = 0;
			direction = direction.normalized;
			var z = Mathf.Round(direction.z);
			var x = Mathf.Round(direction.x);
			var nonZeroX = direction.x > 0 ? 1 : -1;
			var nonZeroZ = direction.z > 0 ? 1 : -1;
			_animator.SetFloat(AnimatorZ, x == 0 ? nonZeroZ : z);
			_animator.SetFloat(AnimatorX, z == 0 ? nonZeroX : x);
			_animator.SetFloat(AnimatorSpotX, nonZeroX);
			_animator.SetFloat(AnimatorAttackX, z >= 0 ? nonZeroX : x);
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