using Collectibles.PowerUp.BallPowerUps;
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

		[SerializeField] private MeshFilter outlineMeshFilter;

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
		private DefaultBallStrategy _defaultBallStrategy;
		private GameObject _shadow;
		private BallAudio _audio;

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
			_shadow = GetComponentInChildren<Shadow>().gameObject;
			_audio = GetComponent<BallAudio>();
			_defaultBallStrategy = new DefaultBallStrategy(_meshFilter.mesh, _meshRenderer.materials);
			_ballStrategy = _defaultBallStrategy;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!Thrown) return;
			if (collision.gameObject == _thrower || (!collision.gameObject.CompareTag("Floor") &&
			                                         !collision.gameObject.tag.Contains("Player")))
			{
				_audio.SoftBounce();
				return;
			}

			_audio.HardBounce();
			_ballStrategy.OnHit(collision);
			_ballStrategy = _defaultBallStrategy;
			_defaultBallStrategy.Apply(this);
			Landed();
		}

		private void OnCollisionStay(Collision other)
		{
			if (other.gameObject.CompareTag("Floor"))
				_rigidbody.velocity /= groundSlowFactor;
		}

		private void Update()
		{
			if (_curGrowStartTime >= 0)
			{
				ChangeSize(Mathf.Lerp(MinSize, MaxSize, GrowTimePercent));
				if (_curShrinkStartTime >= 0)
					_curShrinkStartTime = -1;
			}
			else if (_curShrinkStartTime >= 0)
			{
				ChangeSize(Mathf.Lerp(MaxSize, MinSize, ShrinkTimePercent));
				if (ShrinkTimePercent == 0)
					_curShrinkStartTime = -1;
			}
		}

		private void LateUpdate()
		{
			if (Thrown)
				_ballStrategy.OnLateUpdate();
		}

		#endregion

		#region Public Methods

		public void SetMesh(Mesh mesh)
		{
			if (!mesh) return;
			_meshFilter.mesh = mesh;
			outlineMeshFilter.mesh = mesh;
		}

		public void SetMaterials(Material[] materials)
		{
			if (materials.Length > 0)
				_meshRenderer.materials = materials;
		}

		public void StartCharging(IBallPowerUp powerUp)
		{
			if (powerUp != null)
				_ballStrategy = powerUp;
			_ballStrategy.OnCharge(this);
			gameObject.SetActive(true);
		}

		public bool Pickup(Transform newParent, bool isRolling)
		{
			if (_held || Thrown || (_ballStrategy.IsUncatchableWithRoll() && isRolling))
				return false;
			_held = true;
			_transform.SetParent(newParent);
			_transform.localPosition = Vector3.zero;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			_rigidbody.isKinematic = true;
			_rigidbody.angularVelocity = Vector3.zero;
			_collider.enabled = false;
			_trailRenderer.enabled = false;
			_shadow.SetActive(false);
			gameObject.SetActive(false);
			return true;
		}

		public void Throw(Vector3 velocity, Vector3 posChange, GameObject thrower)
		{
			_ballStrategy.OnThrow(velocity);
			Release(posChange);
			_rigidbody.velocity = velocity;
			_rigidbody.AddTorque(velocity);
			Thrown = true;
			_trailRenderer.enabled = true;
			_thrower = thrower;
		}


		public void Grow()
		{
			_curGrowStartTime = Time.time;
		}

		public void Shrink()
		{
			_curShrinkStartTime = Time.time;
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
			_shadow.SetActive(true);
			_transform.SetParent(null);
			_held = false;
		}

		private void Landed()
		{
			_thrower = null;
			Thrown = false;
			_trailRenderer.enabled = false;
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