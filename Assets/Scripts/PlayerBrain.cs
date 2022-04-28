using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
	#region Public Properties

	public Vector2 MovementStick { get; set; }

	public Vector2 AimingStick { get; set; }

	#endregion;

	#region Public Fields

	public bool movementByPush;

	#endregion

	#region Serialized Fields

	[SerializeField] private float speed;
	[SerializeField] private float pickupRadius;
	[SerializeField] private string ballLayer = "Ball";
	
	#endregion

	#region Private Fields

	private Rigidbody _myRigid;
	private int _ballLayer;
	
	private Ball _ball; //if not null than it is held by the player and is a child of the game object.

	#endregion

	#region Function Events

	private void Awake()
	{
		_myRigid = GetComponent<Rigidbody>();
		_ballLayer = LayerMask.GetMask(ballLayer);
	}

	private void FixedUpdate()
	{
		if (movementByPush)
		{
			if (MovementStick.sqrMagnitude > 0.1)
			{
				_myRigid.AddForce(new Vector3(MovementStick.x * speed, 0, MovementStick.y * speed), ForceMode.Impulse);
			}
		}
		else
		{
			if (MovementStick.sqrMagnitude > 0.1)
			{
				_myRigid.velocity = new Vector3(MovementStick.x * speed, 0, MovementStick.y * speed);
			}
			else
			{
				_myRigid.velocity = Vector3.zero;
			}
		}
	}

	#endregion

	#region Public Methods

	public void ShootBall(float power)
	{
		if (_ball == null) return;
		//_ball.Throw(power*AimingStick); // TODO: should convert to vector3 on XZ and add desired angle upwards
		_ball = null;
	}

	public void PickupBall()
	{
		if (_ball == null) return;
		var groundProjection = transform.position;
		groundProjection.y = 0;
		if (!Physics.CapsuleCast(transform.position,groundProjection, pickupRadius, Vector3.down, out var hit,
			    _ballLayer)) return;
		_ball = hit.collider.gameObject.GetComponent<Ball>();
		if (_ball != null)
			_ball.Pickup(transform);
	}

	#endregion
}