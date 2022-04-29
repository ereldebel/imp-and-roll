using UnityEngine;

public class Ball : MonoBehaviour
{
	#region Public Properties

	public float XPosition => transform.position.x;
	public bool Grounded => Physics.CheckSphere(transform.position, _checkSphereRadius);
	public bool Held { get; private set; }

	#endregion

	#region Serialized Fields

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	#endregion

	#region Private Fields

	private Rigidbody _rigidbody;
	private Transform _transform;
	private float _checkSphereRadius;

	#endregion

	#region Function Events

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;
		OnValidate();
	}

	private void OnValidate()
	{
		_checkSphereRadius = GetComponent<SphereCollider>().radius + groundedDistance;
	}

	#endregion

	#region Public Methods

	public bool Pickup(Transform newParent)
	{
		if (Held)
			return false;
		Held = true;
		_transform.SetParent(newParent);
		_transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
		return true;
	}
	
	public void Throw(Vector3 velocity)
	{
		Release();
		_rigidbody.AddForce(velocity);
	}
	
	public void Release(Vector3 posChange)
	{
		_transform.position += posChange;
		Release();
	}

	#endregion

	#region Private Methods

	private void Release()
	{
		gameObject.SetActive(true);
		_transform.SetParent(null);
		Held = false;
	}

	#endregion
}