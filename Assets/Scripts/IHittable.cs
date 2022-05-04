using UnityEngine;

public interface IHittable
{
	void TakeHit(Vector3 contactPoint, Vector3 velocity);
}