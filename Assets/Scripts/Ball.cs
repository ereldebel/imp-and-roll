using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
	#region Public Properties

	public float Mass => _rigidbody.mass;
	public float Radius => _collider.radius;
	public bool Thrown { get; private set; } = false;

	#endregion

	#region Private Properties

	private float MinSize => sizeRange[0];
	private float MaxSize => sizeRange[1];
	private float GrowTimePercent => Mathf.Clamp(Time.time - _curGrowStartTime, 0, growDur) / growDur;
	private float ShrinkTimePercent => Mathf.Clamp(Time.time - _curShrinkStartTime, 0, shrinkDur) / shrinkDur;

	#endregion

	#region Serialized Fields

	[SerializeField] private Vector2 sizeRange = new Vector2(0.5f, 1);
	[SerializeField] private float growDur = 0.5f;
	[SerializeField] private float shrinkDur = 1;

	#endregion

	#region Private Fields

	private Transform _transform;
	private Rigidbody _rigidbody;
	private SphereCollider _collider;
	private ParticleSystem _myParticles;
	private TrailRenderer _trailRenderer;

	private GameObject _thrower = null;
	private bool _held = false;
	private float _curGrowStartTime = -1;
	private float _curShrinkStartTime = -1;

	#endregion

	#region Function Events

	private void Awake()
	{
		_transform = transform;
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<SphereCollider>();
		_myParticles = GetComponent<ParticleSystem>();
		_trailRenderer = GetComponent<TrailRenderer>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!Thrown) return;
		if (collision.gameObject == _thrower) return;
		if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
			Landed();
		collision.gameObject.GetComponent<IHittable>()?.TakeHit(collision.relativeVelocity);
	}

	private void Update()
	{
		ChangeSize(_curGrowStartTime >= 0
			? Mathf.Lerp(MinSize, MaxSize, GrowTimePercent)
			: Mathf.Lerp(MaxSize, MinSize, ShrinkTimePercent));
	}
	

	#endregion

	#region Public Methods

	public void Grow(float time)
	{
		_curGrowStartTime = time;
	}

	public void Pickup(Transform newParent)
	{
		if (_held || Thrown)
			return;
		_held = true;
		_transform.SetParent(newParent);
		_transform.localPosition = Vector3.zero;
		// gameObject.SetActive(false);
		_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
		_rigidbody.isKinematic = true;
		_collider.enabled = false;
	}

	public void Throw(Vector3 velocity, Vector3 posChange, GameObject thrower)
	{
		Release(posChange);
		Shrink(Time.time);
		_rigidbody.velocity = velocity;
		Thrown = true;
		_myParticles.Play();
		_trailRenderer.enabled = true;
		_thrower = thrower;
	}

	#endregion

	#region Private Methods

	private void Release(Vector3 posChange)
	{
		_transform.position += posChange;
		_curGrowStartTime = -1;
		_rigidbody.isKinematic = false;
		_rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
		_collider.enabled = true;
		_transform.SetParent(null);
		_held = false;
	}

	private void Landed()
	{
		Thrown = false;
		_myParticles.Stop();
		_trailRenderer.enabled = false;
	}

	private void Shrink(float time)
	{
		_curShrinkStartTime = time;
	}

	private void ChangeSize(float size)
	{
		//TODO: should keep an eye on this transformation - it should work but documentation is worrisome
		_transform.localScale = Vector3.one;
		var lossyScale = transform.lossyScale;
		_transform.localScale = new Vector3(size / lossyScale.x, size / lossyScale.y,
			size / lossyScale.z);
	}
	#endregion
}