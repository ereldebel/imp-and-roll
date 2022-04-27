using UnityEngine;

public class Ball : MonoBehaviour
{
	public bool Grounded => Physics.CheckSphere(_transform.position, _checkSphereRadius);
	
	public float XPosition => _transform.position.x;

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	private SphereCollider _collider;
	private Transform _transform;
	private float _checkSphereRadius;
	
	private void Awake()
	{
		_collider = GetComponent<SphereCollider>();
		_transform = transform;
		_checkSphereRadius = _collider.radius + groundedDistance;
	}

	private void OnValidate()
	{
	}
}