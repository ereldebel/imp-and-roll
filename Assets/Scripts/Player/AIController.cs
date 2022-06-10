using Managers;
using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(PlayerBrain))]
	public class AIController : MonoBehaviour
	{
		[SerializeField] private float throwLoad = 0.8f;
		[SerializeField] private bool printStateChanges = true;
		[SerializeField] private AIState state;

		private Transform _otherPlayer;
		private PlayerBrain _brain;
		private bool _rightSide;

		private float DistanceToBorder => Mathf.Abs(MatchManager.DivisionBorder.position.x - transform.position.x);

		private Vector3 PlayerSideCenter
		{
			get
			{
				var midOfPlayerSideX = MatchManager.ArenaLength / 4;
				midOfPlayerSideX = _rightSide ? midOfPlayerSideX : -midOfPlayerSideX;
				midOfPlayerSideX += MatchManager.DivisionBorder.position.x / 2;
				return new Vector3(midOfPlayerSideX, 0, 0);
			}
		}

		private AIState State
		{
			set
			{
				if (printStateChanges && state != value)
					print(value);
				state = value;
			}
		}

		private enum AIState
		{
			Idle,
			Throwing,
			ChasingBall,
			ChasingPowerUp,
			DodgingMonster,
			DodgingBall
		}

		private void Awake()
		{
			_rightSide = transform.position.x > 0;
			_brain = GetComponent<PlayerBrain>();
			foreach (var player in GameManager.Shared.GetOpposingPlayers(gameObject))
			{
				_otherPlayer = player.transform;
				return;
			}
		}

		private void Update()
		{
			if (!MatchManager.BallTransform || !MatchManager.DivisionBorder) return;
			var ballPosition = MatchManager.BallTransform.position;
			if (!_brain.HasBall && ballPosition.y > 0.3 &&
			    Vector3.Distance(transform.position, ballPosition) < 0.4 + (NextStdGaussian() / 10))
			{
				State = AIState.DodgingBall;
				_brain.DodgeRoll();
				return;
			}

			var divisionPosition = MatchManager.DivisionBorder.position;
			var ballIsOnBorderRight = ballPosition.x > divisionPosition.x;
			if (_brain.ThrowChargeTime > 0)
			{
				State = AIState.Throwing;
				_brain.AimingStick = DirectionTo(_otherPlayer.position + Next2DGaussianXZ() / 4);
				if (_brain.ThrowChargeTime > throwLoad + NextStdGaussian() / 10 || DistanceToBorder < 1)
					_brain.ThrowBall();
				else
					MoveInDirection(DirectionTo(_otherPlayer.position));
			}
			else if (_rightSide == ballIsOnBorderRight)
			{
				if (_brain.HasBall)
				{
					State = AIState.Throwing;
					if (DistanceToBorder < 1.5)
					{
						State = AIState.DodgingMonster;
						MoveInDirection(new Vector3(_rightSide ? 2 : -2, 0, -transform.position.z), 2);
					}
					else
						_brain.ChargeThrow();
				}
				else
				{
					State = AIState.ChasingBall;
					MoveInDirection(DirectionTo(ballPosition), 1);
				}
			}
			else
			{
				foreach (var collectible in MatchManager.CollectibleCollection)
				{
					State = AIState.ChasingPowerUp;
					var collectibleIsOnBorderRight = collectible.position.x > divisionPosition.x;
					if (_rightSide != collectibleIsOnBorderRight) continue;
					MoveInDirection(DirectionTo(collectible.position));
					return;
				}

				State = AIState.Idle;
				var movementDirection = DirectionTo(PlayerSideCenter);
				MoveInDirection(movementDirection);
			}
		}

		private void MoveInDirection(Vector2 movementDirection, int stressLevel = 0)
		{
			var sqrMagnitude = movementDirection.sqrMagnitude;
			if (sqrMagnitude > 0.15f)
				_brain.MovementStick = sqrMagnitude > 1 ? movementDirection.normalized : movementDirection;
			if (stressLevel == 2 || (stressLevel == 1 && sqrMagnitude > 50) || movementDirection.sqrMagnitude > 100)
				_brain.DodgeRoll();
		}

		private Vector2 DirectionTo(Vector3 destination)
		{
			var pos = transform.position;
			return new Vector2(destination.x - pos.x, destination.z - pos.z);
		}

		private static Vector3 Next2DGaussianXZ()
		{
			return new Vector3(NextStdGaussian(), 0, NextStdGaussian());
		}

		private static float NextStdGaussian()
		{
			float u, s;
			do
			{
				u = 2f * Random.value - 1f;
				var v = 2f * Random.value - 1f;
				s = u * u + v * v;
			} while (s >= 1);

			return u * Mathf.Sqrt(-2f * Mathf.Log(s) / s);
		}
	}
}