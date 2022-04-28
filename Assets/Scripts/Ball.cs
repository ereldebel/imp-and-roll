using UnityEngine;

public class Ball : MonoBehaviour
{
	public bool Grounded => Physics.CheckSphere(_rigidbody.position, _checkSphereRadius);

	public float XPosition => _rigidbody.position.x;

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	private SphereCollider _collider;
	private Rigidbody _rigidbody;
	private Transform _transform;
	private float _checkSphereRadius;

	private void Awake()
	{
		_collider = GetComponent<SphereCollider>();
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;
		_checkSphereRadius = _collider.radius + groundedDistance;
	}

	public void Pickup(Transform newParent)
	{
		_transform.SetParent(newParent);
		_transform.localPosition = Vector3.zero;
	}
	
	public void Release()
	{
		_transform.SetParent(null);
	}
	
	public void Throw(Vector3 velocity)
	{
		_rigidbody.AddForce(velocity);
		_transform.SetParent(null);
	}
}