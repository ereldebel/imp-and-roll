using UnityEngine;

public interface IHittable
{
	void TakeHit(Vector3 velocity, bool ignoreRoll);
}