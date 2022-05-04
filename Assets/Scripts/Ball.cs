using UnityEngine;

public class Ball : MonoBehaviour
{
	#region Public Properties

	public float XPosition => transform.position.x;
	public bool Grounded => Physics.CheckSphere(transform.position, _checkSphereRadius);
	public float Mass => _rigidbody.mass;
	public bool Held { get; private set; }
	public float Radius { get; private set; }

	#endregion

	#region Serialized Fields

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	[SerializeField] private float dragWhileOnFloor = -1;

	#endregion

	#region Private Fields

	private Rigidbody _rigidbody;
	private Transform _transform;
	private float _checkSphereRadius;
	private bool _thrown = false;
	private float _defaultBallDrag;

	#endregion

	#region Function Events

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_transform = transform;
		_defaultBallDrag = _rigidbody.drag;
		OnValidate();
	}

	private void OnValidate()
	{
		if (dragWhileOnFloor < 0)
			dragWhileOnFloor = _defaultBallDrag;
		Radius = GetComponent<SphereCollider>().radius;
		_checkSphereRadius = Radius + groundedDistance;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!_thrown) return;
		foreach (var contact in collision.contacts)
			contact.otherCollider.GetComponent<IHittable>()?.TakeHit(contact.normal);
		_rigidbody.drag = dragWhileOnFloor;
		_thrown = false;
	}

	#endregion

	#region Public Methods

	public bool Pickup(Transform newParent)
	{
		if (Held || _thrown)
			return false;
		Held = true;
		_transform.SetParent(newParent);
		_transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
		return true;
	}

	public void Throw(Vector3 velocity, Vector3 posChange)
	{
		Release(posChange);
		_rigidbody.AddForce(velocity, ForceMode.Impulse);
		_thrown = true;
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
		_rigidbody.drag = _defaultBallDrag;
	}

	#endregion
}