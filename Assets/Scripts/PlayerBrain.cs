using System;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
	public Vector2 MovementStick { get; set; }

	public Vector2 AimingStick { get; set; }
	public bool movementByPush;
	
	[SerializeField] private float speed;
	
	private Rigidbody _myRigid;
	private GameObject _ball; //if not null than it is held by the player and is a child of the game object.

	private void Start()
	{
		_myRigid = GetComponent<Rigidbody>();
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
	
	public void ShootBall(float power) //Since Ball will be used by both of us, added a "template" of the function, we should decide how we want the player and ball behaviour to be.
	{
		//Ball.rigidbody.addforce(power*AimingStick);
	}

	public void PickupBall()
	{
		throw new NotImplementedException();
	}
}