using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(LineRenderer))]
	public class ThrowTrajectory : MonoBehaviour
	{
		[SerializeField] private int numOfSteps;
		[SerializeField] private float timeStepInterval;

		private PlayerBrain _brain;
		private LineRenderer _lineRenderer;
		private Ball _ball;
		private Quaternion _rotation;

		private Vector3[] trajectoryPoints;

		private void Awake()
		{
			_brain = GetComponent<PlayerBrain>();
			trajectoryPoints = new Vector3[numOfSteps];
			_rotation = Quaternion.Inverse(transform.rotation);
			_brain.StartedChargingThrow += Enable;
			_brain.BallThrown += Disable;
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = numOfSteps;
		}

		private void OnDestroy()
		{
			_brain.StartedChargingThrow -= Enable;
			_brain.BallThrown -= Disable;
		}

		private void Enable(Ball ball)
		{
			enabled = true;
			_lineRenderer.enabled = true;
			_ball = ball;
			DrawTrajectory();
			_brain.ChangedAimDirection += DrawTrajectory;
		}

		private void Disable()
		{
			_brain.ChangedAimDirection -= DrawTrajectory;
			_lineRenderer.enabled = false;
			enabled = false;
		}

		private void DrawTrajectory()
		{
			var throwVelocity = _brain.ThrowVelocity * (timeStepInterval / _ball.Mass);
			var gravity = 0.5f * Physics.gravity.y;
			for (var timeStep = 0; timeStep < numOfSteps; ++timeStep)
			{
				var posAtTime = _brain.ThrowOrigin + throwVelocity * timeStep;
				posAtTime.y += gravity * Mathf.Pow(timeStep * timeStepInterval, 2);
				trajectoryPoints[timeStep] = _rotation * posAtTime;
			}

			_lineRenderer.SetPositions(trajectoryPoints);
		}
	}
}