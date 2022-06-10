using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps.Effects
{
	public class Flame : MonoBehaviour
	{
		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = transform.parent.GetComponentInParent<Rigidbody>();
		}

		private void Update()
		{
			var xVelocity = _rigidbody.velocity.x;
			var angle = xVelocity == 0 ? 0 : Mathf.Lerp(Mathf.Sign(xVelocity) * 90, 0, 1 / Mathf.Sqrt(xVelocity));
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}
}