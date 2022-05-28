using System;
using System.Collections;
using System.Collections.Generic;
using Collectibles.PowerUp;
using Collectibles.PowerUp.BallPowerUps;
using Collectibles.PowerUp.GlobalPowerUps;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerBrain : MonoBehaviour, IHittable
	{
		#region Public Properties

		public Vector2 MovementStick
		{
			get => _movementDirection;
			set => _movementDirection = InvertJoysticks ? -value : value;
		}

		public Vector2 AimingStick
		{
			set
			{
				var oldSqrMagnitude = AimDirection.sqrMagnitude;
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
				AimDirection = value.normalized;
				ChangedAimDirection?.Invoke();
			}
		}

		public Vector2 MousePos
		{
			set
			{
				var pos = transform.position;
				AimDirection = (value - new Vector2(pos.x, pos.z)).normalized;
				ChangedAimDirection?.Invoke();
			}
		}

		public bool InvertJoysticks { private get; set; }

		public float ThrowChargeTime => _chargeStartTime > 0 ? Time.time - _chargeStartTime : 0;

		public Vector3 ThrowOrigin =>
			new Vector3(AimDirection.x,
				_ballPositionsByDirection[(int) _animator.GetFloat(AnimatorZ) + 1][ballPositionsSide.Length - 1].y,
				AimDirection.y) *
			(_colliderRadius + _ball.Radius + speed * Time.fixedDeltaTime + throwOriginEpsilon);

		public Vector3 ThrowVelocity =>
			maxThrowForce * ThrowCharge * new Vector3(AimDirection.x, throwYForce, AimDirection.y);

		public bool HasBall => _ball != null;
		public Rumble Rumble { get; private set; }
		public bool Flipped => _left == (transform.rotation == _faceRight);

		#endregion;

		#region Private Properties

		private float ThrowCharge => Mathf.Clamp(Time.time - _chargeStartTime, minThrowChargeTime, maxThrowChargeTime);

		private Vector2 AimDirection
		{
			set => _aimDirection = InvertJoysticks ? -value : value;
			get => _aimDirection;
		}

		#endregion

		#region Serialized Fields

		[Header("Player Movement Settings")] [SerializeField]
		private float speed;

		[SerializeField] private float friction;

		[SerializeField] private float dodgeRollSpeed;

		[SerializeField] private float rollDuration = 0.25f;

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

		[SerializeField] private float maxStunDurationIncrease = 1;
		[SerializeField] private float stunBarPercentagePerHit = 0.2f;

		[Header("Throw Animation")] [SerializeField]
		private Vector3[] ballPositionsDown45 = new Vector3[7];

		[SerializeField] private Vector3[] ballPositionsSide = new Vector3[7];
		[SerializeField] private Vector3[] ballPositionsUp45 = new Vector3[7];

		#endregion

		#region Private Fields

		//Components:
		private CharacterController _controller;
		private Animator _animator;

		//Input:
		private Vector2 _movementDirection;
		private Vector2 _aimDirection;

		//Rotation:
		private Quaternion _faceLeft;
		private Quaternion _faceRight;
		private bool _left;

		//State:
		private bool _stunned;
		private float _stunBar = 1;
		private bool _rolling;
		private bool _calledThrow;
		private float _chargeStartTime = -1;
		private float _colliderRadius;

		//Ball and ball Animation:
		private Ball.Ball _ball; //if not null than it is held by the player and is a child of the game object.
		private readonly List<Vector3[]> _ballPositionsByDirection = new List<Vector3[]>();
		private int _ballThrowPositionIndex;

		//PowerUps:
		private IBallPowerUp _ballPowerUp;
		private IPlayerPowerUp _playerPowerUp;
		private Rumble _rumble;
		
		//Movement
		private Vector3 _velocity = Vector3.zero;
		

		#endregion

		#region Private Static Fields

		private static readonly int AnimatorRunning = Animator.StringToHash("Running");
		private static readonly int AnimatorX = Animator.StringToHash("X Direction");
		private static readonly int AnimatorZ = Animator.StringToHash("Z Direction");
		private static readonly int AnimatorDodge = Animator.StringToHash("Dodge");
		private static readonly int AnimatorStunned = Animator.StringToHash("Stunned");
		private static readonly int AnimatorThrowing = Animator.StringToHash("Throwing");
		private static readonly int AnimatorLost = Animator.StringToHash("Lost");
		private static readonly int AnimatorWon = Animator.StringToHash("Won");
		private static readonly int AnimatorHasBall = Animator.StringToHash("Has Ball");
		private static readonly int AnimatorTaunt = Animator.StringToHash("Taunt");

		#endregion

		#region Public C# Events

		public event Action<Ball.Ball> StartedChargingThrow;
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
			_left = transform.position.x > 0;
			transform.rotation = _left ? _faceLeft : _faceRight;
		}

		private void OnValidate()
		{
			var t = transform;
			var scale = t.localScale;
			_colliderRadius = scale.x * GetComponent<CapsuleCollider>().radius;
			_faceLeft = t.rotation;
			_faceRight = Quaternion.AngleAxis(180, t.up) * _faceLeft;
		}

		private void FixedUpdate()
		{
			ProcessMovementInput();
			if (_chargeStartTime < 0) return;
			_animator.SetFloat(AnimatorX, Mathf.Round(Mathf.Abs(AimDirection.x)));
			_animator.SetFloat(AnimatorZ, Mathf.Round(AimDirection.y));
			transform.rotation = AimDirection.x > 0 ? _faceRight : _faceLeft;
		}

		private void Update()
		{
			_playerPowerUp?.OnUpdate();
			var playerLayerMask = 1 << gameObject.layer;
			var aimDir = new Vector3(AimDirection.x, 0, AimDirection.y);
			if (_chargeStartTime < 0 ||
			    !Physics.Raycast(transform.position + aimDir * (_colliderRadius + _ball.Radius), aimDir,
				    MatchManager.MaxDistance, playerLayerMask)) return;

			if (_rumble)
				_rumble.AimRumblePulse();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Teleporter"))
				PlayerReady();
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Teleporter"))
				PlayerReady();
		}

		private void OnCollisionEnter(Collision collision)
		{
			PickupBall(collision);
		}

		private void OnCollisionStay(Collision collision)
		{
			PickupBall(collision);
		}

		public void Reset()
		{
			_rolling = false;
			_stunned = false;
			_calledThrow = false;
			_ball = null;
			_chargeStartTime = -1;
			_stunBar = 1;
			_animator.SetBool(AnimatorWon, false);
			_animator.SetBool(AnimatorHasBall, false);
			_animator.SetBool(AnimatorLost, false);
			_animator.SetBool(AnimatorRunning, false);
			_animator.SetBool(AnimatorDodge, false);
			_animator.SetBool(AnimatorStunned, false);
			_animator.SetBool(AnimatorThrowing, false);
			_animator.SetFloat(AnimatorX, 1);
			_animator.SetFloat(AnimatorZ, -1);
			SetPowerUp(null);
		}

		#endregion

		#region Public Methods

		public void SetPowerUp(PowerUp powerUp)
		{
			switch (powerUp)
			{
				case IPlayerPowerUp playerPowerUp:
					_ballPowerUp = null;
					_playerPowerUp = playerPowerUp;
					break;
				case IBallPowerUp ballPowerUp:
					_playerPowerUp = null;
					_ballPowerUp = ballPowerUp;
					break;
				default:
					_ballPowerUp = null;
					_playerPowerUp = null;
					break;
			}

			if (_chargeStartTime < 0) return;
			_chargeStartTime = -1;
			ChargeThrow();
		}

		public void Taunt()
		{
			_animator.SetTrigger(AnimatorTaunt);
		}

		public void GameOver(bool won)
		{
			_animator.SetBool(won ? AnimatorWon : AnimatorLost, true);
			_chargeStartTime = -1;
			_ball = null;
			SetPowerUp(null);
			GetComponent<PlayerInput>()?.SwitchCurrentActionMap("Player Tutorial Area");
		}

		public bool ChargeThrow()
		{
			if (_stunned || _rolling || _chargeStartTime >= 0 || _calledThrow || !_ball) return false;
			_chargeStartTime = Time.time;
			_animator.SetBool(AnimatorThrowing, true);
			_ball.StartCharging(_ballPowerUp);
			StartedChargingThrow?.Invoke(_ball);
			return true;
		}

		public void ThrowBall()
		{
			if (_stunned || _rolling || _chargeStartTime < 0 || _calledThrow || !_ball) return;
			_animator.SetBool(AnimatorThrowing, false);
			_calledThrow = true;
		}

		public void TakeHit(Vector3 velocity, bool uncatchableWithRoll)
		{
			if (stunDuration <= 0 || (!uncatchableWithRoll && _rolling)) return;
			if (_rumble)
				_rumble.Stun(_stunBar);
			StartCoroutine(Stun(velocity));
		}

		public void ApplyKnockBack(Vector3 velocity)
		{
			StartCoroutine(KnockBack(velocity));
		}

		public void DodgeRoll()
		{
			if (_chargeStartTime >= 0 || _stunned || _rolling) return;
			var movementDir = MovementStick;
			if (MovementStick == Vector2.zero)
				StartCoroutine(DodgeRoll(new Vector3((Flipped ? 1 : -1) * _animator.GetFloat(AnimatorX), 0,
					_animator.GetFloat(AnimatorZ))));
			else
				StartCoroutine(DodgeRoll(new Vector3(movementDir.x, 0, movementDir.y)));
		}

		public void PlayerReady()
		{
			GameManager.Shared.PlayerReady(gameObject);
		}

		#endregion

		#region Private Methods

		private void PickupBall(Collision collision)
		{
			if (_ball != null || _stunned) return;
			var ball = collision.gameObject.GetComponent<Ball.Ball>();
			if (ball == null) return;
			if (ball.Thrown) return;
			_ball = ball;
			_animator.SetBool(AnimatorHasBall, true);
			if (gameObject.activeSelf)
				_ball.Pickup(transform);
		}

		private void ProcessMovementInput()
		{
			if (_rolling || !_controller.enabled) return;
			if (_stunned)
			{
				_controller.SimpleMove(Vector3.zero);
				return;
			}
			
			if (Math.Abs(friction - 1) == 0 &&MovementStick.sqrMagnitude <= 0.1)
			{
				_animator.SetBool(AnimatorRunning, false);
				_controller.SimpleMove(Vector3.zero);
				return;
			}
			

			_animator.SetBool(AnimatorRunning, true);
			_velocity = Vector3.Lerp(_velocity, new Vector3(MovementStick.x, 0, MovementStick.y), friction);
			if (_chargeStartTime >= 0)
				_velocity *= movementRelativeSpeedWhileCharging;
			else
			{
				if (MovementStick.sqrMagnitude <= 0.1)
				{
					_animator.SetBool(AnimatorRunning, false);
				}else{
					_animator.SetFloat(AnimatorX, Mathf.Round(Mathf.Abs(MovementStick.x)));
					_animator.SetFloat(AnimatorZ, Mathf.Round(MovementStick.y));
					transform.rotation = _velocity.x > 0 ? _faceRight : _faceLeft;
				}
			}

			_controller.SimpleMove(speed * _velocity);
		}

		private void AnimatorEndStun() => _stunned = false;

		private void AnimatorThrowBall()
		{
			if (!_ball) return;
			_ballThrowPositionIndex = 0;
			_chargeStartTime = -1;
			_ball.Throw(ThrowVelocity, ThrowOrigin, gameObject);
			_ball = null;
			_animator.SetBool(AnimatorHasBall, false);
			_calledThrow = false;
			BallThrown?.Invoke();
		}

		private void AnimatorChangeBallPosition() => ChangeBallPosition(_ballThrowPositionIndex++);
		private void AnimatorThrowChargeBallPosition() => ChangeBallPosition(2);

		private void ChangeBallPosition(int index)
		{
			if (index >= ballPositionsSide.Length || !_ball) return;
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
				_stunBar = Mathf.Max(_stunBar - stunBarPercentagePerHit, 0);
			var currStunDuration = stunDuration + maxStunDurationIncrease * (1 - _stunBar);
			StunStarted?.Invoke(_stunBar);
			_stunned = true;
			yield return KnockBack(knockBackDir);
			yield return new WaitForSeconds(currStunDuration - knockBackDuration);
			_animator.SetBool(AnimatorStunned, false);
			StunEnded?.Invoke();
		}

		private IEnumerator KnockBack(Vector3 knockBackDir)
		{
			knockBackDir.y = 0;
			var numIterations = knockBackDuration / Time.fixedDeltaTime;
			for (var i = 0; i < numIterations; ++i)
			{
				_controller.SimpleMove(-knockBackDir * knockBackRelativeSpeed);
				yield return new WaitForFixedUpdate();
			}
		}

		#endregion
	}
}