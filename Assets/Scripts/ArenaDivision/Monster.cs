using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float sightRadius = 1;
		[SerializeField] private LayerMask targetLayerMask;
		[SerializeField] private float baseSpeed = 1;
		[SerializeField] private float baseYSpeed = 0.5f;
		[SerializeField] private float maxHeight = 5;
		[SerializeField] private float chainRemainder = 0.1f;
		[SerializeField] private float maxXDistFromMonster = 1;
		[SerializeField] private Transform divider;
		[SerializeField] private bool constantSpeeds;
		[SerializeField] private float timeToSpeedUp = 60;
		#endregion;

		#region Private Fields

		private SpriteRenderer _spriteRenderer;
		private LineRenderer _lineRenderer;
		private Collider _collider;

		private Transform _ball;
		private float _halfOfArenaWidth;
		private float _chainLength;
		private float _fixedBaseSpeed;
		private float _fixedBaseYSpeed;
		private float _startTime;
		private float _speed = 1;
		private float _ySpeed = 0.5f;

		private static readonly Collider[] TempColliders = new Collider[3];

		#endregion

		#region Function Events

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_lineRenderer = GetComponent<LineRenderer>();
			_collider = GetComponent<Collider>();
			_lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, Vector3.zero);
			_startTime = Time.time;
			OnValidate();
			enabled = false;
		}

		private void Start()
		{
			_ball = GameManager.BallTransform;
			_halfOfArenaWidth = GameManager.ArenaWidth / 2;
		}

		private void OnValidate()
		{
			_chainLength = maxHeight + chainRemainder;
			_fixedBaseSpeed = baseSpeed * Time.fixedDeltaTime;
			_fixedBaseYSpeed = baseYSpeed * Time.fixedDeltaTime;
		}

		private void OnBecameVisible()
		{
			enabled = true;
		}

		private void Update()
		{
			if (constantSpeeds) return;
			// ReSharper disable once PossibleLossOfFraction
			var speedMultiplier = ((int) ((Time.time - _startTime) / timeToSpeedUp) + 1) * 0.6f;// TODO keep looking at values
			_speed = _fixedBaseSpeed * speedMultiplier;
			_ySpeed = _fixedBaseYSpeed * speedMultiplier;
		}

		private void FixedUpdate()
		{
			Move();
			UpdateSpectralChain();
		}

		private void OnTriggerEnter(Collider other)
		{
			GameManager.GameOver(transform.position.x > divider.position.x);
		}

		#endregion

		#region Private Methods

		private void Move()
		{
			var pos = transform.position;
			var ballPos = _ball.position;
			var dividerPos = divider.position;
			var chasingCloseTarget = GetClosestTarget(pos, ballPos, out var closestTargetDir);
			_collider.enabled = closestTargetDir.sqrMagnitude < 1;
			_spriteRenderer.flipX = closestTargetDir.x > dividerPos.x;
			var ballDist = ballPos.x - pos.x;
			var xMovement = Mathf.Abs(ballDist) > 0.01f ? Mathf.Sign(ballDist) * _speed : 0;
			var yMovement = chasingCloseTarget
				? closestTargetDir.y * _ySpeed
				: Mathf.Min(maxHeight - pos.y, 2 * _ySpeed);
			var zMovement = closestTargetDir.z * _speed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) > 0;
			if (movingTowardsDivider)
				xMovement *= 2;
			pos += new Vector3(xMovement, yMovement, zMovement);
			transform.position = pos;
			if (Mathf.Abs(pos.x - dividerPos.x) < maxXDistFromMonster || movingTowardsDivider) return;
			dividerPos.x += xMovement;
			divider.position = dividerPos;
		}

		private void UpdateSpectralChain()
		{
			var closestDividerPoint = divider.position;
			closestDividerPoint.z = transform.position.z;
			_lineRenderer.SetPosition(1, transform.InverseTransformPoint(closestDividerPoint));
		}

		private bool GetClosestTarget(Vector3 pos, Vector3 defaultObjPos, out Vector3 closestTargetDir)
		{
			var playersSeen =
				Physics.OverlapBoxNonAlloc(Vector3.right * pos.x,
					new Vector3(sightRadius, maxHeight, _halfOfArenaWidth), TempColliders, Quaternion.identity,
					targetLayerMask.value);
			var closestPos = defaultObjPos;
			var closestDist = Vector3.Distance(pos, defaultObjPos);
			for (var i = 0; i < playersSeen; ++i)
			{
				var otherPos = TempColliders[i].transform.position;
				var dist = Vector3.Distance(pos, otherPos);
				if (closestDist <= dist) continue;
				closestPos = otherPos;
				closestDist = dist;
			}

			closestTargetDir = closestPos - pos;
			return playersSeen > 0;
		}

		#endregion
	}
}