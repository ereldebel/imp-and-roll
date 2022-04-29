using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
	#region Public Properties

	public Vector2 MovementStick { private get; set; }
	public Vector2 AimingStick { private get; set; }

	#endregion;

	#region Private Properties

	private Vector3 ColliderBottom => transform.position + _diffFromColliderCenterToBottom;

	#endregion

	#region Serialized Fields

	[SerializeField] private bool movementByPush;
	[SerializeField] private float speed;
	[SerializeField] private float throwPower, throwYPower;
	[SerializeField] private float pickupDistance;
	[SerializeField] private LayerMask ballMask;

	#endregion

	#region Private Fields

	private Rigidbody _myRigid;
	private float _colliderRadius;
	private float _pickupRadius;
	private Vector3 _diffFromColliderCenterToBottom;

	private Ball _ball; //if not null than it is held by the player and is a child of the game object.

	private static Collider[] TempColliders = new Collider[5];

	#endregion

	#region Function Events

	private void Awake()
	{
		_myRigid = GetComponent<Rigidbody>();
		OnValidate();
	}

	private void OnValidate()
	{
		var t = transform;
		_colliderRadius = transform.localScale.x * GetComponent<CapsuleCollider>().radius;
		_pickupRadius = _colliderRadius + pickupDistance;
		_diffFromColliderCenterToBottom =
			t.rotation * (0.5f * t.localScale.y * GetComponent<CapsuleCollider>().height * Vector3.down);
	}

	private void FixedUpdate()
	{
		if (movementByPush)
		{
			if (MovementStick.sqrMagnitude > 0.1)
				_myRigid.AddForce(new Vector3(MovementStick.x * speed, 0, MovementStick.y * speed), ForceMode.Impulse);
		}
		else
		{
			if (MovementStick.sqrMagnitude > 0.1)
				_myRigid.velocity = new Vector3(MovementStick.x * speed, 0, MovementStick.y * speed);
			else
				_myRigid.velocity = Vector3.zero;
		}
	}

	#endregion

	#region Public Methods

	public void ThrowBall(float power)
	{
		if (_ball == null) return;
		_ball.Throw(throwPower*power*new Vector3(AimingStick.x,throwYPower,AimingStick.y));
		_ball = null;
	}

	public void PickupBall()
	{
		if (_ball != null)
		{
			_ball.Release((transform.position.x > 0 ? Vector3.left : Vector3.right) * _colliderRadius +
			              _diffFromColliderCenterToBottom);
			_ball = null;
			return;
		}

		if (Physics.OverlapCapsuleNonAlloc(transform.position, ColliderBottom, _pickupRadius, TempColliders,
			    ballMask.value) <= 0) return;
		_ball = TempColliders[0].gameObject.GetComponent<Ball>();
		if (_ball == null) return;
		if (_ball.Grounded && _ball.Pickup(transform)) return;
		_ball = null;
	}

	#endregion
}