using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(LineRenderer))]
	public class ThrowTrajectory : MonoBehaviour
	{
		[SerializeField] private int numOfSteps;
		[SerializeField] private float timeStepInterval;
		[SerializeField] private LayerMask playerLayerMask;

		private PlayerBrain _brain;
		private LineRenderer _lineRenderer;
		private Ball _ball;
		private Quaternion _rotation;

		private bool _charged;
		private int _maxSteps;
		private Vector3[] _trajectoryPoints;

		private void Awake()
		{
			_brain = GetComponent<PlayerBrain>();
			_lineRenderer = GetComponent<LineRenderer>();
			_brain.StartedChargingThrow += Enable;
			_brain.BallThrown += Disable;

			var maxDist = Mathf.Sqrt(Mathf.Pow(GameManager.ArenaLength, 2) + Mathf.Pow(GameManager.ArenaWidth, 2));
			_maxSteps = Mathf.CeilToInt(maxDist / timeStepInterval);
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
			_charged = false;
			_lineRenderer.enabled = false;
			enabled = false;
		}

		private void DrawTrajectory()
		{
			var throwVelocity = _brain.ThrowVelocity * (timeStepInterval / _ball.Mass);
			var gravity = 0.5f * Physics.gravity;
			var c = new Collider[1];
			var rumbles = false;
			for (var timeStep = 0; timeStep < _maxSteps; ++timeStep)
			{
				var posAtTime = _brain.ThrowOrigin + throwVelocity * timeStep;
				posAtTime += gravity * Mathf.Pow(timeStep * timeStepInterval, 2);
				if (timeStep < numOfSteps)
					_trajectoryPoints[timeStep] = _rotation * posAtTime;
				if (rumbles || 0 >=
				    Physics.OverlapSphereNonAlloc(transform.position + posAtTime, 0.5f, c, playerLayerMask)) continue;
				_brain.Rumble?.AimRumblePulse();
				rumbles = true;
			}

			_lineRenderer.SetPositions(_trajectoryPoints);
		}
	}
}