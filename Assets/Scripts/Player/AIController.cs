using Managers;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(PlayerBrain))]
	public class AIController : MonoBehaviour
	{
		[SerializeField] private Transform otherPlayer;
		[SerializeField] private float throwLoad = 0.8f;

		private Transform _border;
		private Transform _ball;
		private PlayerBrain _brain;
		private bool _rightSide;
		private bool _throwing;

		private void Awake()
		{
			_rightSide = transform.position.x > 0;
			_brain = GetComponent<PlayerBrain>();
			if (otherPlayer == null)
				otherPlayer = GameManager.Players[0].transform;
		}

		private void OnEnable()
		{
			_ball = MatchManager.BallTransform;
			_border = MatchManager.DivisionBorder;
		}

		private void Update()
		{
			var ballIsOnBorderRight = _ball.position.x > _border.position.x;
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
					_throwing = _brain.ChargeThrow();
				else
					_brain.MovementStick = DirectionTo(_ball.position).normalized;
			}
			else
			{
				var midOfPlayerPartX = MatchManager.ArenaLength / 4;
				midOfPlayerPartX = _rightSide ? midOfPlayerPartX : -midOfPlayerPartX;
				midOfPlayerPartX += _border.position.x / 2;
				var movementDirection = DirectionTo(new Vector3(midOfPlayerPartX, 0, 0));
				_brain.MovementStick =
					movementDirection.sqrMagnitude > 0.5f ? movementDirection.normalized : Vector2.zero;
				if (movementDirection.sqrMagnitude > 100)
					_brain.DodgeRoll();
			}
		}

		
		private Vector2 DirectionTo(Vector3 destination)
		{
			var pos = transform.position;
			return new Vector2(destination.x - pos.x, destination.z - pos.z);
		}
	}
}