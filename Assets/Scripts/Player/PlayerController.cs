using Managers;
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
		private Plane _plane = new Plane(Vector3.up, 0);
		public static bool Dirty { get; set; } 

		private void Awake()
		{
			_myBrain = GetComponent<PlayerBrain>();
			_playerInput = GetComponent<PlayerInput>();
			_camera = Camera.main;
		}

		public void OnMovement(InputAction.CallbackContext context)
		{
			var direction = context.ReadValue<Vector2>();
			_myBrain.MovementStick = direction;
			Dirty = Dirty || direction != Vector2.zero;
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			if (_playerInput.currentControlScheme == "Keyboard")
				_myBrain.MousePos = ScreenToWorld2D(context.ReadValue<Vector2>());
			else
			{
				var direction = context.ReadValue<Vector2>();
				_myBrain.AimingStick = direction;
				Dirty = Dirty || direction != Vector2.zero;
			}
		}

		public void OnThrow(InputAction.CallbackContext context)
		{
			if (context.started)
				_myBrain.ChargeThrow();

			if (context.canceled)
				_myBrain.ThrowBall();
			Dirty = true;
		}

		public void OnDodgeRoll(InputAction.CallbackContext context)
		{
			if (context.started)
				_myBrain.DodgeRoll();
			Dirty = true;
		}

		public void OnStart(InputAction.CallbackContext context)
		{
			
			if (context.started)
				_myBrain.PlayerReady();
			Dirty = true;
		}

		public void OnTaunt(InputAction.CallbackContext context)
		{
			if (context.started)
				_myBrain.Taunt();
			Dirty = true;
		}

		public void OnDisconnect()
		{
			GameManager.Shared.Pause(_playerInput);
		}
		
		public void OnReconnect()
		{
			GameManager.Shared.Resume(_playerInput);
		}

		public void OnPause(InputAction.CallbackContext context)
		{
			if (context.started)
				GameManager.Shared.Pause(_playerInput);
		}

		public void OnResume(InputAction.CallbackContext context)
		{
			if (context.started)
				GameManager.Shared.Resume(_playerInput);
		}

		public void OnQuit(InputAction.CallbackContext context)
		{
			if (context.started)
				GameManager.Shared.Quit();
		}

		public void OnRemove(InputAction.CallbackContext context)
		{
			if (context.started)
				GameManager.Shared.RemovePlayer(_playerInput);
		}

		private Vector2 ScreenToWorld2D(Vector2 screenPos)
		{
			if (_camera == null) _camera=Camera.main;
			if (_camera == null) return Vector2.zero;
			var worldPos3d = Vector3.zero;
			var ray = _camera.ScreenPointToRay(screenPos);
			if (_plane.Raycast(ray, out var distance))
				worldPos3d = ray.GetPoint(distance);
			return new Vector2(worldPos3d.x, worldPos3d.z);
		}
	}
}