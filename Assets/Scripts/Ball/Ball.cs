﻿using Collectibles.PowerUp.BallPowerUps;
using UnityEngine;

namespace Ball
{
	public class Ball : MonoBehaviour
	{
		#region Public Properties

		public float Mass => _rigidbody.mass;
		public float Radius => _collider.radius;
		public bool Thrown { get; private set; }

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
		[SerializeField] private float groundSlowFactor = 2;

		#endregion

		#region Private Fields

		private Transform _transform;
		private Rigidbody _rigidbody;
		private SphereCollider _collider;
		private TrailRenderer _trailRenderer;
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;

		private GameObject _thrower;
		private bool _held;
		private float _curGrowStartTime = -1;
		private float _curShrinkStartTime = -1;
		private IBallStrategy _ballStrategy;
		private IBallStrategy _defaultBallStrategy;

		#endregion

		#region Function Events

		private void Awake()
		{
			_transform = transform;
			_rigidbody = GetComponent<Rigidbody>();
			_collider = GetComponent<SphereCollider>();
			_trailRenderer = GetComponent<TrailRenderer>();
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshFilter = GetComponent<MeshFilter>();
			_defaultBallStrategy = new DefaultBallStrategy(_meshFilter.mesh, _meshRenderer.material);
			_ballStrategy = _defaultBallStrategy;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!Thrown) return;
			if (collision.gameObject == _thrower) return;
			if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Player"))
				Landed();
			var catchableWithRoll = _ballStrategy.OnHit();
			collision.gameObject.GetComponent<IHittable>()?.TakeHit(collision.relativeVelocity, catchableWithRoll);
			_ballStrategy = _defaultBallStrategy;
		}

		private void OnCollisionStay(Collision other)
		{
			if (other.gameObject.CompareTag("Floor"))
				_rigidbody.velocity /= groundSlowFactor;
		}

		private void Update()
		{
			ChangeSize(_curGrowStartTime >= 0
				? Mathf.Lerp(MinSize, MaxSize, GrowTimePercent)
				: Mathf.Lerp(MaxSize, MinSize, ShrinkTimePercent));
		}

		private void LateUpdate()
		{
			_ballStrategy.OnLateUpdate();
		}

		#endregion

		#region Public Methods

		public void SetMesh(Mesh mesh)
		{
			if (mesh)
				_meshFilter.mesh = mesh;
		}

		public void SetMaterial(Material material)
		{
			if (material)
				_meshRenderer.material = material;
		}

		public void StartCharging(IBallPowerUp powerUp)
		{
			if (powerUp != null)
				_ballStrategy = powerUp;
			_ballStrategy.OnCharge(this);
			Grow();
			gameObject.SetActive(true);
		}

		public void Pickup(Transform newParent)
		{
			if (_held || Thrown)
				return;
			_held = true;
			_transform.SetParent(newParent);
			_transform.localPosition = Vector3.zero;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			_rigidbody.isKinematic = true;
			_rigidbody.angularVelocity = Vector3.zero;
			_collider.enabled = false;
			_trailRenderer.enabled = false;
			gameObject.SetActive(false);
		}

		public void Throw(Vector3 velocity, Vector3 posChange, GameObject thrower)
		{
			_ballStrategy.OnThrow();
			Release(posChange);
			Shrink();
			_rigidbody.velocity = velocity;
			_rigidbody.AddTorque(velocity);
			Thrown = true;
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
			_trailRenderer.enabled = true;
			_transform.SetParent(null);
			_held = false;
		}

		private void Landed()
		{
			Thrown = false;
			_trailRenderer.enabled = false;
		}

		private void Grow()
		{
			_curGrowStartTime = Time.time;
		}

		private void Shrink()
		{
			_curShrinkStartTime = Time.time;
		}

		private void ChangeSize(float size)
		{
			//TODO: should keep an eye on this transformation - it should work but documentation is worrisome
			_transform.localScale = Vector3.one;
			var lossyScale = _transform.lossyScale;
			_transform.localScale = new Vector3(size / lossyScale.x, size / lossyScale.y,
				size / lossyScale.z);
		}

		#endregion
	}
}