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
			new Vector3(_aimDirection.x, 0, _aimDirection.y) * (_colliderRadius + _ball.Radius);

		public Vector3 ThrowVelocity =>
			maxThrowForce * ThrowCharge * new Vector3(_aimDirection.x, throwYForce, _aimDirection.y);

		public float ThrowCharge => Mathf.Clamp(Time.time - _chargeStartTime, minThrowChargeTime, maxThrowChargeTime);

		#endregion;

		#region Private Properties

		private Vector3 ColliderBottom => transform.position + _diffFromColliderCenterToBottom;
		private CharacterController _controller;

		#endregion

		#region Serialized Fields

		[SerializeField] private float speed;
		[SerializeField] private float dodgerollSpeed;

		[SerializeField] private float pickupDistance;
		[SerializeField] private LayerMask ballMask;
		[SerializeField] private float maxThrowForce;
		[SerializeField] private float throwYForce;
		[SerializeField] private float minThrowChargeTime = 0.1f;
		[SerializeField] private float maxThrowChargeTime = 1;
		[SerializeField] private float knockBackDuration = 0.5f;
		[SerializeField] private float rollDuration = 0.25f;
		

		[SerializeField] private float knockOutDuration = 1;
		[SerializeField] private float movementRelativeSpeedWhileCharging = 0.5f;

		#endregion

		#region Private Fields

		private SpriteRenderer _spriteRenderer;
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
		
		#region Private Functions

		private Vector3 vector2_to_vector3XZ(Vector2 input)
		{
			return new Vector3(input.x,0, input.y);
		}
		#endregion
		
		#region Function Events

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
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
			Move();
		}

		#endregion

		#region Public Methods

		public void ChargeThrow()
		{
			if (_ball == null) return;
			// _myRigid.velocity = Vector3.zero;
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

		public bool PickupBall()
		{
			if (_ball != null)
			{
				_ball.Release((transform.position.x > 0 ? Vector3.left : Vector3.right) *
				              (_colliderRadius + _ball.Radius) +
				              _diffFromColliderCenterToBottom);
				_ball = null;
				return true;
			}

			if (Physics.OverlapCapsuleNonAlloc(transform.position, ColliderBottom, _pickupRadius, TempColliders,
				    ballMask.value) <= 0) return false;
			_ball = TempColliders[0].gameObject.GetComponent<Ball>();
			if (_ball == null) return false;
			if (_ball.Grounded && _ball.Pickup(transform)) return true;
			_ball = null;
			return false;
		}

		public void TakeHit(Vector3 normal)
		{
			// _myRigid.AddForce(Vector3.Reflect(normal, Vector3.up), ForceMode.Impulse);
			if (knockOutDuration > 0 && !_rolling)
				StartCoroutine(Knockout(Vector3.Reflect(normal, Vector3.up)));
		}

		public void DodgeRoll()
		{
			print(MovementStick);
			StartCoroutine(DodgeRoll(vector2_to_vector3XZ(MovementStick)));
		}
		#endregion

		#region Private Methods and Coroutines

		private void Move()
		{
			if (_knockedOut || _rolling) return;
			Vector3 velocity;
			if (MovementStick.sqrMagnitude > 0.1)
				velocity = new Vector3(MovementStick.x * speed/50, 0, MovementStick.y * speed/50);
			else
				velocity = Vector3.zero;
			_controller.Move(_chargeStartTime >= 0 ? velocity * movementRelativeSpeedWhileCharging : velocity);
		}
		private IEnumerator DodgeRoll(Vector3 rollDir)
		{
			_rolling = true;
			print(rollDir);
			for (int i = 0; i < rollDuration*50; i++)
			{
				_controller.Move(dodgerollSpeed*rollDir*Time.fixedDeltaTime);
				yield return new WaitForFixedUpdate();
			}
			_rolling = false;

		}
		private IEnumerator Knockout(Vector3 knockbackDir)
		{
			var color = _spriteRenderer.color;
			_spriteRenderer.color = Color.gray;
			_knockedOut = true;
			Vector3 temp = knockbackDir;
			temp.y = 0;
			knockbackDir = temp;
			for (int i = 0; i < knockBackDuration*50; i++)
			{
				print(knockbackDir);
				_controller.Move(-knockbackDir*Time.fixedDeltaTime);
				yield return new WaitForFixedUpdate();
			}
			yield return new WaitForSeconds(knockOutDuration - knockBackDuration);
			_knockedOut = false;
			_spriteRenderer.color = color;
		}

		#endregion
	}
}