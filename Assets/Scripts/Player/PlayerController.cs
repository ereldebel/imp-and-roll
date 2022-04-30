using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	[RequireComponent(typeof(PlayerBrain))]
	[RequireComponent(typeof(PlayerInput))]
	public class PlayerController : MonoBehaviour
	{
		private PlayerBrain _myBrain;
		private PlayerInput _playerInput;
		private Camera _camera;
		Plane plane = new Plane(Vector3.up, 0);
		
		private void Awake()
		{
			_myBrain = GetComponent<PlayerBrain>();
			_playerInput = GetComponent<PlayerInput>();
			_camera = Camera.main;
		}

		public void OnMovement(InputAction.CallbackContext context)
		{
			_myBrain.MovementStick = context.ReadValue<Vector2>();
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			if (_playerInput.currentControlScheme == "Keyboard")
				_myBrain.MousePos = ScreenToWorld2D(context.ReadValue<Vector2>());
			else
				_myBrain.AimingStick = context.ReadValue<Vector2>();
		}

		public void OnThrow(InputAction.CallbackContext context)
		{
			if (context.started)
				_myBrain.LoadThrow();

			if (context.canceled)
				_myBrain.ThrowBall();
		}

		public void OnPickup(InputAction.CallbackContext context)
		{
			if (context.started)
				_myBrain.PickupBall();
		}

		private Vector2 ScreenToWorld2D(Vector2 screenPos)
		{
			if (_camera == null) return Vector2.zero;
			var worldPos3d = Vector3.zero;
			var ray = _camera.ScreenPointToRay(screenPos);
			if (plane.Raycast(ray, out var distance))
				worldPos3d = ray.GetPoint(distance);
			return new Vector2(worldPos3d.x, worldPos3d.z);
		}
	}
}