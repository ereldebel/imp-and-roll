using System.Collections.Generic;
using UnityEngine;

public class PredictedTrajectory
{
	private readonly Vector3 _origin;
	private readonly Vector3 _throwVelocity;
	private readonly float _gravity;

	private const float TimeStepInterval = 0.1f;
	private const int NumOfSteps = 10;

	public PredictedTrajectory(Vector3 origin,Vector3 throwVelocity, float mass, float gravity)
	{
		_origin = origin;
		_throwVelocity = throwVelocity * TimeStepInterval / (mass * Time.fixedDeltaTime);
		_gravity = gravity;
	}

	private Vector3 TrajectoryPosAtTimeStep(int timeStep)
	{
		var posAtTime =_origin + _throwVelocity * timeStep;
		var gravityVelocityY = 0.5f * _gravity * timeStep * timeStep;
		posAtTime.y += gravityVelocityY;
		return posAtTime;
	}

	public List<Vector3> GetTrajectory()
	{
		var trajectory = new List<Vector3>(NumOfSteps); 
		for (var timeStep = 0; timeStep < NumOfSteps; ++timeStep)
			trajectory.Add(TrajectoryPosAtTimeStep(timeStep));
		return trajectory;
	}
}