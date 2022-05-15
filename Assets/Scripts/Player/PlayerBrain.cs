using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerBrain : MonoBehaviour, IHittable
	{
		#region Public Properties

		public Vector2 MovementStick { get; set; }

		public Vector2 AimingStick
		{
			set
			{
				var oldSqrMagnitude = _aimDirection.sqrMagnitude;
				var newSqrMagnitude = value.sqrMagnitude;
				if (_chargeStartTime >= 0 && newSqrMagnitude < oldSqrMagnitude && newSqrMagnitude < 0.1)
				{
					ThrowBall();
					return;
				}

				if (_chargeStartTime < 0 && newSqrMagnitude > oldSqrMagnitude && newSqrMagnitude > 0.1)
				{
					ChargeThrow();
					return;
				}

				if (newSqrMagnitude < 0.7) return;
				_aimDirection = value.normalized;
				ChangedAimDirection?.Invoke();
			}
		}

		public Vector2 MousePos
		{
			set
			{
				var pos = transform.position;
				_aimDirection = (value - new Vector2(pos.x, pos.z)).normalized;
				ChangedAimDirection?.Invoke();
			}
		}

		public float ThrowChargeTime => _chargeStartTime > 0 ? Time.time - _chargeStartTime : 0;

		public Vector3 ThrowOrigin =>
			new Vector3(_aimDirection.x,
				_ballPositionsByDirection[(int) _animator.GetFloat(AnimatorZ) + 1][ballPositionsSide.Length - 1].y,
				_aimDirection.y) *
			(_colliderRadius + _ball.Radius + speed * Time.fixedDeltaTime + throwOriginEpsilon);

		public Vector3 ThrowVelocity =>
			maxThrowForce * ThrowCharge * new Vector3(_aimDirection.x, throwYForce, _aimDirection.y);

		public float ThrowCharge => Mathf.Clamp(Time.time - _chargeStartTime, minThrowChargeTime, maxThrowChargeTime);
		public bool HasBall => _ball != null;
		public Rumble Rumble { get; private set; }
		public bool Flipped => transform.rotation == _faceRight;

		#endregion;

		#region Private Properties

		private Vector3 ColliderBottom => transform.position + _diffFromColliderCenterToBottom;
		private Vector3 ColliderTop => transform.position - _diffFromColliderCenterToBottom;

		#endregion

		#region Serialized Fields

		[Header("Player Movement Settings")] [SerializeField]
		private float speed;

		[SerializeField] private float dodgeRollSpeed;

		[SerializeField] private float rollDuration = 0.25f;

		// [SerializeField] private float pickupDistance; TODO look at these, they are never used, remove if unneeded?
		// [SerializeField] private LayerMask ballMask;
		[Header("Ball Throw Settings")] [SerializeField]
		private float maxThrowForce;

		[SerializeField] private float throwYForce;
		[SerializeField] private float minThrowChargeTime = 0.1f;
		[SerializeField] private float maxThrowChargeTime = 1;
		[SerializeField] private float movementRelativeSpeedWhileCharging = 0.5f;
		[SerializeField] private float throwOriginEpsilon = 0.1f;

		[Header("Knock Back Settings")] [SerializeField]
		private float knockBackDuration = 0.5f;

		[SerializeField] private float knockBackRelativeSpeed = 0.5f;

		[Header("Stun Settings")] [SerializeField]
		private float stunDuration = 1;

		[SerializeField] private float stunDurationIncrease = 0.2f;
		[SerializeField] private float stunBarPercentagePerHit = 0.2f;

		[Header("Throw Animation")] [SerializeField]
		private Vector3[] ballPositionsDown45 = new Vector3[7];

		[SerializeField] private Vector3[] ballPositionsSide = new Vector3[7];
		[SerializeField] private Vector3[] ballPositionsUp45 = new Vector3[7];

		#endregion

		#region Private Fields

		private CharacterController _controller;
		private Animator _animator;

		private float _colliderRadius;
		private Vector3 _diffFromColliderCenterToBottom;
		private float _chargeStartTime = -1;
		private Vector2 _aimDirection;
		private bool _knockedOut;
		private bool _rolling;
		private bool _calledThrow;
		private float _stunBar = 1;
		private int _timesStunned = 0;
		private Quaternion _faceLeft;
		private Quaternion _faceRight;
		
		private readonly List<Vector3[]> _ballPositionsByDirection = new List<Vector3[]>();
		private int _ballThrowPositionIndex = 0;

		private Ball _ball; //if not null than it is held by the player and is a child of the game object.

		private static readonly int AnimatorRunning = Animator.StringToHash("Running");
		private static readonly int AnimatorX = Animator.StringToHash("X Direction");
		private static readonly int AnimatorZ = Animator.StringToHash("Z Direction");
		private static readonly int AnimatorDodge = Animator.StringToHash("Dodge");
		private static readonly int AnimatorStunned = Animator.StringToHash("Stunned");
		private static readonly int AnimatorThrowing = Animator.StringToHash("Throwing");

		#endregion

		#region Public C# Events

		public event Action<Ball> StartedChargingThrow;
		public event Action ChangedAimDirection;
		public event Action BallThrown;
		public event Action<float> StunStarted;
		public event Action StunEnded;

		#endregion

		#region Function Events

		private void Awake()
		{
			_controller = GetComponent<CharacterController>();
			_animator = GetComponent<Animator>();
			Rumble = GetComponent<Rumble>();
			_ballPositionsByDirection.Add(ballPositionsDown45);
			_ballPositionsByDirection.Add(ballPositionsSide);
			_ballPositionsByDirection.Add(ballPositionsUp45);
			OnValidate();
			transform.rotation = transform.position.x > 0 ? _faceLeft : _faceRight;
		}

		private void OnValidate()
		{
			var t = transform;
			var scale = t.localScale;
			_colliderRadius = scale.x * GetComponent<CapsuleCollider>().radius;
			_diffFromColliderCenterToBottom =
				t.rotation * (0.5f * scale.y * GetComponent<CapsuleCollider>().height * Vector3.down);
			_faceLeft = transform.rotation;
			_faceRight = Quaternion.AngleAxis(180, transform.up) * _faceLeft;
		}

		private void FixedUpdate()
		{
			ProcessMovementInput();
		}


		private void OnCollisionEnter(Collision collision)
		{
			PickupBall(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			PickupBall(collision);
		}

		#endregion

		#region Public Methods

		public bool ChargeThrow()
		{
			if (_knockedOut || _rolling || _chargeStartTime >= 0 || _calledThrow || _ball == null) return false;
			_chargeStartTime = Time.time;
			_animator.SetBool(AnimatorThrowing, true);
			_ball.Grow(_chargeStartTime);
			StartedChargingThrow?.Invoke(_ball);
			return true;
		}

		public void ThrowBall()
		{
			if (_knockedOut || _rolling || _chargeStartTime < 0 || _calledThrow || _ball == null) return;
			_animator.SetBool(AnimatorThrowing, false);
			_calledThrow = true;
		}

		public void TakeHit(Vector3 velocity)
		{
			if (stunDuration <= 0 || _rolling) return;
			Rumble?.Stun();
			StartCoroutine(Stun(velocity));
		}

		public void DodgeRoll()
		{
			if (MovementStick == Vector2.zero || _chargeStartTime >= 0 || _knockedOut || _rolling) return;
			StartCoroutine(DodgeRoll(Vector2ToVector3XZ(MovementStick)));
		}

		public void PlayerReady()
		{
			CrossSceneManager.Shared.PlayerReady(gameObject);
		}
		#endregion

		#region Private Methods

		private void PickupBall(Collision collision)
		{
			if (_ball != null || _knockedOut) return;
			var ball = collision.gameObject.GetComponent<Ball>();
			if (ball == null) return;
			if (ball.Thrown) return;
			_ball = ball;
			if (gameObject.activeSelf)
				_ball.Pickup(transform);
		}

		private void ProcessMovementInput()
		{
			if (_knockedOut || _rolling) return;
			if (MovementStick.sqrMagnitude <= 0.1)
			{
				_animator.SetBool(AnimatorRunning, false);
				return;
			}

			_animator.SetBool(AnimatorRunning, true);
			_animator.SetFloat(AnimatorX, Mathf.Round(Mathf.Abs(MovementStick.x)));
			_animator.SetFloat(AnimatorZ, Mathf.Round(MovementStick.y));
			var velocity = speed * new Vector3(MovementStick.x, 0, MovementStick.y);
			if (_chargeStartTime >= 0)
				velocity *= movementRelativeSpeedWhileCharging;
			transform.rotation = velocity.x > 0 ? _faceRight : _faceLeft;
			_controller.SimpleMove(velocity);
		}

		private static Vector3 Vector2ToVector3XZ(Vector2 input)
		{
			return new Vector3(input.x, 0, input.y);
		}

		private void AnimatorEndStun() => _knockedOut = false;

		private void AnimatorThrowBall()
		{
			_ballThrowPositionIndex = 0;
			_chargeStartTime = -1;
			_ball.Throw(ThrowVelocity, ThrowOrigin, gameObject);
			_ball = null;
			_calledThrow = false;
			BallThrown?.Invoke();
		}

		private void AnimatorChangeBallPosition() => ChangeBallPosition(_ballThrowPositionIndex++);


		private void AnimatorThrowChargeBallPosition() => ChangeBallPosition(2);

		private void ChangeBallPosition(int index)
		{
			var ballPos = _ballPositionsByDirection[(int) _animator.GetFloat(AnimatorZ) + 1][index];
			_ball.transform.localPosition = ballPos;
		}

		#endregion

		#region Private Coroutines

		private IEnumerator DodgeRoll(Vector3 rollDir)
		{
			rollDir = rollDir.normalized;
			_animator.SetTrigger(AnimatorDodge);
			_rolling = true;
			for (var i = 0; i < rollDuration / Time.fixedDeltaTime; i++)
			{
				_controller.SimpleMove(dodgeRollSpeed * rollDir);
				yield return new WaitForFixedUpdate();
			}

			_rolling = false;
		}

		private IEnumerator Stun(Vector3 knockBackDir)
		{
			_animator.SetBool(AnimatorRunning, false);
			_animator.SetBool(AnimatorStunned, true);
			_animator.SetFloat(AnimatorX, 1);
			_animator.SetFloat(AnimatorZ, -1);
			transform.rotation = knockBackDir.x > 0 ? _faceRight : _faceLeft;
			if (_stunBar > 0)
			{
				_stunBar = Mathf.Max(_stunBar - stunBarPercentagePerHit, 0);
				++_timesStunned;
			}

			var currStunDuration = stunDuration + stunDurationIncrease * _timesStunned;
			if (_stunBar <= 0)
				currStunDuration *= 2;
			StunStarted?.Invoke(_stunBar);
			_knockedOut = true;
			knockBackDir.y = 0;
			var numIterations = knockBackDuration / Time.fixedDeltaTime;
			for (var i = 0; i < numIterations; ++i)
			{
				_controller.SimpleMove(-knockBackDir * knockBackRelativeSpeed);
				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds(currStunDuration - knockBackDuration);
			_animator.SetBool(AnimatorStunned, false);
			StunEnded?.Invoke();
		}

		#endregion
	}
}