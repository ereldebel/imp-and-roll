﻿using UnityEngine;

public class Ball : MonoBehaviour
{
	#region Public Properties

	public Vector3 Position => transform.position;
	public bool Grounded => Physics.CheckSphere(transform.position, _checkSphereRadius);
	public float Mass => _rigidbody.mass;
	public float Radius { get; private set; }
	public bool Thrown { get; private set; } = false;

	#endregion

	#region Serialized Fields

	[Tooltip("The distance of the ball from the ground that the ball is already considered grounded.")] [SerializeField]
	private float groundedDistance = 0.1f;

	#endregion

	#region Private Fields

	private Rigidbody _rigidbody;
	private Transform _transform;

	private GameObject _thrower = null;
	private float _checkSphereRadius;
	private bool _held = false;

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
		Radius = GetComponent<SphereCollider>().radius;
		_checkSphereRadius = Radius + groundedDistance;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!Thrown) return;
		if (collision.gameObject == _thrower) return;
		collision.gameObject.GetComponent<IHittable>()?.TakeHit(collision.relativeVelocity);
		Thrown = false;
	}

	#endregion

	#region Public Methods

	public void Pickup(Transform newParent)
	{
		if (_held || Thrown)
			return;
		_held = true;
		_transform.localPosition = Vector3.zero;
		gameObject.SetActive(false);
		if (transform.parent.gameObject.activeSelf)
			_transform.SetParent(newParent);
	}

	public void Throw(Vector3 velocity, Vector3 posChange, GameObject thrower)
	{
		Release(posChange);
		_rigidbody.AddForce(velocity, ForceMode.Impulse);
		Thrown = true;
		_thrower = thrower;
	}

	#endregion

	#region Private Methods

	private void Release(Vector3 posChange)
	{
		_transform.position += posChange;
		gameObject.SetActive(true);
		_held = false;
		if (transform.parent.gameObject.activeSelf)
			_transform.SetParent(null);
	}

	#endregion
}