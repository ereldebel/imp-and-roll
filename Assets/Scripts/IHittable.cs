using UnityEngine;

public interface IHittable
{
	bool TakeHit(Vector3 velocity, bool uncatchableWithRoll = false, float damage = 1);
}