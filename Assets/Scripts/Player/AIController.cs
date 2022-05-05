using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(PlayerBrain))]
	public class AIController : MonoBehaviour
	{
		[SerializeField] private Transform otherPlayer;
		[SerializeField] private Transform ball;
		[SerializeField] private Transform border;
		[SerializeField] private float throwLoad = 0.8f;

		private PlayerBrain _brain;
		private bool _rightSide;
		private bool _throwing;

		private void Awake()
		{
			_rightSide = transform.position.x > 0;
			_brain = GetComponent<PlayerBrain>();
		}

		private void Update()
		{
			var ballIsOnBorderRight = ball.position.x > border.position.x;
			if (_throwing)
			{
				_brain.AimingStick = DirectionTo(otherPlayer.position);
				if (_brain.ThrowChargeTime > throwLoad)
				{
					_brain.ThrowBall();
					_throwing = false;
				}
			}
			else if ((_rightSide && ballIsOnBorderRight) || (!_rightSide && !ballIsOnBorderRight))
			{
				if (_brain.HasBall)
				{
					_throwing = true;
					_brain.ChargeThrow();
				}
				else
					_brain.MovementStick = DirectionTo(ball.position);
			}
			else
			{
				var midOfPlayerPartX = GameManager.ArenaWidth / 4;
				midOfPlayerPartX = _rightSide ? midOfPlayerPartX : -midOfPlayerPartX;
				midOfPlayerPartX += border.position.x / 2;
				_brain.MovementStick = DirectionTo(new Vector3(midOfPlayerPartX, 0, 0));
			}
		}

		private Vector2 DirectionTo(Vector3 destination)
		{
			var pos = transform.position;
			return new Vector2(destination.x - pos.x, destination.z - pos.z).normalized;
		}
	}
}