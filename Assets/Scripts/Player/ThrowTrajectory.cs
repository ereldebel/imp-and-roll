using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(LineRenderer))]
	public class ThrowTrajectory : MonoBehaviour
	{
		[SerializeField] private int numOfSteps;
		[SerializeField] private float timeStepInterval;
		[SerializeField] private Color[] gradientColors = new Color[] {Color.clear, Color.white};

		private PlayerBrain _brain;
		private LineRenderer _lineRenderer;
		private Ball _ball;
		private Quaternion _rotation;

		private bool _charged;
		private Vector3[] _trajectoryPoints;

		private void Awake()
		{
			_brain = GetComponent<PlayerBrain>();
			_lineRenderer = GetComponent<LineRenderer>();
			_brain.StartedChargingThrow += Enable;
			_brain.BallThrown += Disable;
			OnValidate();
			enabled = false;
		}

		private void OnValidate()
		{
			_rotation = Quaternion.Inverse(transform.rotation);
			_trajectoryPoints = new Vector3[numOfSteps];
			GetComponent<LineRenderer>().positionCount = numOfSteps;
		}

		private void OnDestroy()
		{
			_brain.StartedChargingThrow -= Enable;
			_brain.BallThrown -= Disable;
		}

		private void Update()
		{
			// if (_charged) return;
			// if (_brain.ThrowCharge >= 1)
			// {
			// 	_charged = true;
			// 	_brain.ChangedAimDirection += DrawTrajectory;
			// }

			DrawTrajectory();
		}

		private void Enable(Ball ball)
		{
			enabled = true;
			_lineRenderer.enabled = true;
			_ball = ball;
			DrawTrajectory();
		}

		private void Disable()
		{
			if (_charged)
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
				_trajectoryPoints[timeStep] = _rotation * posAtTime;
			}

			_lineRenderer.SetPositions(_trajectoryPoints);
		}
	}
}