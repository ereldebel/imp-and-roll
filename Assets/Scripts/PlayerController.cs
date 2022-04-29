using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerBrain))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float maxLoadingShotTime = 1;
	private PlayerBrain _myBrain;
	private float _holdShootTimer = 0;

	private void Awake()
	{
		_myBrain = GetComponent<PlayerBrain>();
	}

	public void OnMovement(InputAction.CallbackContext context)
	{
		_myBrain.MovementStick = context.ReadValue<Vector2>();
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		_myBrain.AimingStick = context.ReadValue<Vector2>();
	}

	public void OnThrow(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			_holdShootTimer = 0;
		}

		if (context.canceled)
		{
			_myBrain.ThrowBall(_holdShootTimer < maxLoadingShotTime
				? _holdShootTimer
				: maxLoadingShotTime); //If player held the button for "too long" give max instead
			_holdShootTimer = 0;
		}
	}
	
	public void OnPickup(InputAction.CallbackContext context)
	{
		if (context.started)
			_myBrain.PickupBall();
	}

	private void Update()
	{
		_holdShootTimer += Time.deltaTime;
	}
}