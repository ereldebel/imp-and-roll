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
			new Vector3(_aimDirection.x, 0, _aimDirection.y) * (_colliderRadius + _ball.Radius+throwOriginEpsilon);

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
		[SerializeField] private float knockOutDuration = 1;
		[SerializeField] private float movementRelativeSpeedWhileCharging = 0.5f;
		[SerializeField] private float throwOriginEpsilon = 0.1f;

		#endregion

		#region Private Fields

		private SpriteRenderer _spriteRenderer;
		private Collider _collider;
		private CharacterController _controller;
		private float _colliderRadius;
		private float _pickupRadius;
		private Vector3 _diffFromColliderCenterToBottom;
		private float _chargeStartTime = -1;
		private Vector2 _aimDirection;
		private bool _knockedOut;
		private bool _rolling;


		private Ball _ball; //if not null than it is held by the player and is a child of the game object.

		private static readonly Collider[] TempColliders = new Collider[5];

		#endregion

		#region Public C# Events

		public event Action<Ball> StartedChargingThrow;
		public event Action ChangedAimDirection;
		public event Action BallThrown;

		#endregion

		#region Function Events

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_collider = GetComponent<Collider>();
			_controller = GetComponent<CharacterController>();
			OnValidate();
		}

		private void OnValidate()
		{
			var t = transform;
			var scale = t.localScale;
			_colliderRadius = scale.x * GetComponent<CapsuleCollider>().radius;
			_pickupRadius = _colliderRadius + pickupDistance;
			_diffFromColliderCenterToBottom =
				t.rotation * (0.5f * scale.y * GetComponent<CapsuleCollider>().height * Vector3.down);
		}

		private void FixedUpdate()
		{
			ProcessMovementInput();
			PickupBall();
		}

		#endregion

		#region Public Methods

		public void ChargeThrow()
		{
			if (_ball == null) return;
			_chargeStartTime = Time.time;
			StartedChargingThrow?.Invoke(_ball);
		}

		public bool ThrowBall()
		{
			if (_ball == null) return false;
			_chargeStartTime = -1;
			_ball.Throw(ThrowVelocity, ThrowOrigin);
			_ball = null;
			BallThrown?.Invoke();
			return true;
		}

		public void TakeHit(Vector3 contactPoint, Vector3 velocity)
		{
			if (!(knockOutDuration > 0) || _rolling) return;
			StartCoroutine(Knockout(velocity));
		}

		public void DodgeRoll()
		{
			print(MovementStick);
			StartCoroutine(DodgeRoll(vector2_to_vector3XZ(MovementStick)));
		}

		#endregion

		#region Private Methods and Coroutines
		
		private bool PickupBall()
		{
			if (_ball != null ||
			    Physics.OverlapCapsuleNonAlloc(transform.position, ColliderBottom, _pickupRadius, TempColliders,
				    ballMask.value) <= 0) return false;
			_ball = TempColliders[0].gameObject.GetComponent<Ball>();
			if (_ball == null) return false;
			if (_ball.Grounded && _ball.Pickup(transform)) return true;
			_ball = null;
			return false;
		}

		private void ProcessMovementInput()
		{
			if (_knockedOut || _rolling) return;
			if (MovementStick.sqrMagnitude <= 0.1) return;
			var velocity = speed * new Vector3(MovementStick.x, 0, MovementStick.y);
			if (_chargeStartTime >= 0)
				velocity *= movementRelativeSpeedWhileCharging;
			_controller.SimpleMove(velocity);
		}

		private static Vector3 vector2_to_vector3XZ(Vector2 input)
		{
			return new Vector3(input.x, 0, input.y);
		}

		private IEnumerator DodgeRoll(Vector3 rollDir)
		{
			_rolling = true;
			for (var i = 0; i < rollDuration / Time.fixedDeltaTime; i++)
			{
				_controller.Move(dodgeRollSpeed * Time.fixedDeltaTime * rollDir);
				yield return new WaitForFixedUpdate();
			}

			_rolling = false;
		}

		private IEnumerator Knockout(Vector3 knockBackDir)
		{
			var color = _spriteRenderer.color;
			_spriteRenderer.color = Color.gray;
			_knockedOut = true;
			Vector3 temp = knockBackDir;
			temp.y = 0;
			knockBackDir = temp;
			for (int i = 0; i < knockBackDuration * 50; i++)
			{
				print(knockBackDir);
				_controller.Move(-knockBackDir * Time.fixedDeltaTime);
				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds(knockOutDuration - knockBackDuration);
			_knockedOut = false;
			_spriteRenderer.color = color;
		}

		#endregion
	}
}