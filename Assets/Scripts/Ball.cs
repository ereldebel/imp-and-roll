using System;
using Player;
using UnityEngine;

public class Ball : MonoBehaviour
{
	#region Public Properties

	public Vector3 Position => transform.position;
	public bool Grounded => Physics.CheckSphere(transform.position, _checkSphereRadius);
	public float Mass => _rigidbody.mass;
	public float Radius { get; private set; }
	public bool Thrown { get; private set; } = false;
	private float _minSize => sizeRange[0];
	private float _maxSize => sizeRange[1];
	private float IncreaseTimePercent => Mathf.Clamp(Time.time-_curIncreaseStartTime ,0 , increaseDur)/increaseDur;
	private float DecreaseTimePercent => Mathf.Clamp(Time.time-_curDecreaseStartTime ,0 , decreaseDur)/decreaseDur;
	#endregion

	#region Serialized Fields

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	[SerializeField] private Vector2 sizeRange = new Vector2(0.5f, 1);
	[SerializeField] private float increaseDur = 0.5f;
	[SerializeField] private float decreaseDur = 1;

	#endregion

	#region Private Fields

	private Rigidbody _rigidbody;
	private Transform _transform;

	private GameObject _thrower = null;
	private float _checkSphereRadius;
	private bool _held = false;
	private ParticleSystem _myParticles;
	private float _curIncreaseStartTime = -1;
	private float _curDecreaseStartTime = -1;

	#endregion

	#region Function Events

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_myParticles = GetComponent<ParticleSystem>();
		
		_transform = transform;
		OnValidate();
	}

	private void OnValidate()
	{
		Radius = GetComponent<SphereCollider>().radius;
		_checkSphereRadius = Radius + groundedDistance;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!Thrown) return;
		if (collision.gameObject == _thrower) return;
		if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player")) Landed();
		collision.gameObject.GetComponent<IHittable>()?.TakeHit(collision.relativeVelocity);
		
	}

	private void FixedUpdate()
	{
		ChangeSize(Mathf.Lerp(_maxSize, _minSize, DecreaseTimePercent));
		// print(_increaseSize
		// 	? Mathf.Lerp(_minSize, _maxSize, IncreaseTimePercent)
		// 	: Mathf.Lerp(_maxSize, _minSize, IncreaseTimePercent));
	}

	#endregion

	#region Public Methods

	public void IncreaseSize(float time)
	{
		_curIncreaseStartTime = time;
	}
	

	public void Pickup(Transform newParent)
	{
		if (_held || Thrown)
			return;
		_held = true;
		_transform.SetParent(newParent);
		_transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
	}

	public void Throw(Vector3 velocity, Vector3 posChange, GameObject thrower)
	{
		Release(posChange);
		DecreaseSize(Time.time);
		_rigidbody.velocity = velocity;
		Thrown = true;
		_myParticles.Play();
		_thrower = thrower;
	}

	#endregion

	#region Private Methods

	private void Release(Vector3 posChange)
	{
		_transform.position += posChange;
		ChangeSize(Mathf.Lerp(_minSize, _maxSize, IncreaseTimePercent));
		gameObject.SetActive(true);
		_transform.SetParent(null);
		_held = false;
	}

	private void Landed()
	{
		Thrown = false;
		_myParticles.Stop();
	}
	private void DecreaseSize(float time)
	{
		_curDecreaseStartTime = time;
	}
	private void ChangeSize(float size)
	{
		_transform.localScale = new Vector3(size, size, size); //Bug potential - if parent isnt (1,1,1) scale, this could work unexpectedly.
	}
	#endregion
}