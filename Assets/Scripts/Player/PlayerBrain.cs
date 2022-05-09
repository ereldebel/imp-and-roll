using System;
using System.Collections;
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
				if (_aimDirection.sqrMagnitude < 0.9) return;
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
			new Vector3(_aimDirection.x, 0, _aimDirection.y) *
			(_colliderRadius + _ball.Radius + speed * Time.fixedDeltaTime + throwOriginEpsilon);

		public Vector3 ThrowVelocity =>
			maxThrowForce * ThrowCharge * new Vector3(_aimDirection.x, throwYForce, _aimDirection.y);

		public float ThrowCharge => Mathf.Clamp(Time.time - _chargeStartTime, minThrowChargeTime, maxThrowChargeTime);
		public bool HasBall => _ball != null;

		#endregion;

		#region Private Properties

		private Vector3 ColliderBottom => transform.position + _diffFromColliderCenterToBottom;
		private Vector3 ColliderTop => transform.position - _diffFromColliderCenterToBottom;

		#endregion

		#region Serialized Fields

		[SerializeField] private float speed;
		[SerializeField] private float dodgeRollSpeed;
		[SerializeField] private float rollDuration = 0.25f;
		[SerializeField] private float pickupDistance;
		[SerializeField] private LayerMask ballMask;
		[SerializeField] private float maxThrowForce;
		[SerializeField] private float throwYForce;
		[SerializeField] private float minThrowChargeTime = 0.1f;
		[SerializeField] private float maxThrowChargeTime = 1;
		[SerializeField] private float knockBackDuration = 0.5f;
		[SerializeField] private float knockBackRelativeSpeed = 0.5f;
		[SerializeField] private float stunDuration = 1;
		[SerializeField] private float movementRelativeSpeedWhileCharging = 0.5f;
		[SerializeField] private float throwOriginEpsilon = 0.1f;
		[SerializeField] private float stunBarPercentagePerHit = 0.2f;

		#endregion

		#region Private Fields

		private SpriteRenderer _spriteRenderer;
		private CharacterController _controller;
		private Animator _animator;
		private float _colliderRadius;
		private Vector3 _diffFromColliderCenterToBottom;
		private float _chargeStartTime = -1;
		private Vector2 _aimDirection;
		private bool _knockedOut;
		private bool _rolling;
		private float _stunBar = 1;


		private Ball _ball; //if not null than it is held by the player and is a child of the game object.

		private static readonly Collider[] TempColliders = new Collider[5];
		private static readonly int AnimatorRunning = Animator.StringToHash("Running");
		private static readonly int AnimatorX = Animator.StringToHash("X Direction");
		private static readonly int AnimatorZ = Animator.StringToHash("Z Direction");
		private static readonly int AnimatorDodge = Animator.StringToHash("Dodge");

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
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_controller = GetComponent<CharacterController>();
			_animator = GetComponent<Animator>();
			_spriteRenderer.flipX = transform.position.x < 0;
			OnValidate();
		}

		private void OnValidate()
		{
			var t = transform;
			var scale = t.localScale;
			_colliderRadius = scale.x * GetComponent<CapsuleCollider>().radius;
			_diffFromColliderCenterToBottom =
				t.rotation * (0.5f * scale.y * GetComponent<CapsuleCollider>().height * Vector3.down);
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

		public void ChargeThrow()
		{
			if (_ball == null) return;
			_chargeStartTime = Time.time;
			AimingStick = MovementStick.normalized;
			StartedChargingThrow?.Invoke(_ball);
		}

		public void ThrowBall()
		{
			if (_ball == null) return;
			_chargeStartTime = -1;
			_ball.Throw(ThrowVelocity, ThrowOrigin, gameObject);
			_ball = null;
			BallThrown?.Invoke();
		}

		public void TakeHit(Vector3 velocity)
		{
			if (stunDuration <= 0 || _rolling) return;
			StartCoroutine(Stun(velocity));
		}

		public void DodgeRoll()
		{
			if (MovementStick == Vector2.zero || _chargeStartTime > 0 || _knockedOut || _rolling) return;
			StartCoroutine(DodgeRoll(vector2_to_vector3XZ(MovementStick)));
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
			_spriteRenderer.flipX = velocity.x > 0;
			_controller.SimpleMove(velocity);
		}

		private static Vector3 vector2_to_vector3XZ(Vector2 input)
		{
			return new Vector3(input.x, 0, input.y);
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
			_stunBar = Mathf.Max(_stunBar - stunBarPercentagePerHit, 0);
			var currStunDuration = stunDuration;
			if (_stunBar <= 0)
			{
				_stunBar = 1;
				currStunDuration *= 2;
			}

			StunStarted?.Invoke(_stunBar);
			var color = _spriteRenderer.color;
			_spriteRenderer.color = Color.gray;
			_knockedOut = true;
			knockBackDir.y = 0;
			for (var i = 0; i < knockBackDuration * 50; i++)
			{
				_controller.SimpleMove(-knockBackDir * knockBackRelativeSpeed);
				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds(currStunDuration - knockBackDuration);
			_knockedOut = false;
			_spriteRenderer.color = color;
			StunEnded?.Invoke();
		}

		#endregion
	}
}