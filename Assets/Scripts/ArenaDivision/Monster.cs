using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		[SerializeField] private float sightRadius = 1;
		[SerializeField] private LayerMask targetLayerMask;
		[SerializeField] private float speed = 1;
		[SerializeField] private float ySpeed = 0.5f;
		[SerializeField] private float maxHeight = 5;
		[SerializeField] private float chainRemainder = 0.1f;
		[SerializeField] private Transform divider;

		private Transform _ball;
		private SpriteRenderer _spriteRenderer;
		private LineRenderer _lineRenderer;
		private float _halfOfArenaWidth;
		private float _chainLength;
		private float _fixedSpeed;
		private float _fixedYSpeed;

		private static readonly Collider[] TempColliders = new Collider[3];

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.positionCount = 2;
			_lineRenderer.SetPosition(0, Vector3.zero);
			OnValidate();
		}

		private void Start()
		{
			_ball = GameManager.BallTransform;
			_halfOfArenaWidth = GameManager.ArenaWidth / 2;
		}

		private void OnValidate()
		{
			_chainLength = maxHeight + chainRemainder;
			_fixedSpeed = speed * Time.fixedDeltaTime;
			_fixedYSpeed = ySpeed * Time.fixedDeltaTime;
		}

		private void FixedUpdate()
		{
			Move();
			UpdateSpectralChain();
		}

		private void OnCollisionEnter(Collision collision)
		{
			var other = collision.gameObject;
			if (other.CompareTag("Ball"))
				GameManager.MonsterGotBall();
			else
				GameManager.MonsterGotPlayer(other.name.EndsWith("1"));
		}

		private void Move()
		{
			var pos = transform.position;
			var ballPos = _ball.position;
			var dividerPos = divider.position;
			var chasingCloseTarget = GetClosestTarget(pos, ballPos, out var closestObjDir);
			_spriteRenderer.flipX = closestObjDir.x > pos.x;
			var ballDist = ballPos.x - pos.x;
			var xMovement = Mathf.Abs(ballDist) > 0.01f ? Mathf.Sign(ballDist) * _fixedSpeed : 0;
			var yMovement = chasingCloseTarget
				? closestObjDir.y * _fixedYSpeed
				: Mathf.Min(maxHeight - pos.y, 2 * _fixedYSpeed);
			var zMovement = closestObjDir.z * _fixedSpeed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) > 0;
			if (movingTowardsDivider)
				xMovement *= 2;
			pos += new Vector3(xMovement, yMovement, zMovement);
			transform.position = pos;
			if (Vector3.Distance(pos, dividerPos) < _chainLength || movingTowardsDivider) return;
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
					new Vector3(sightRadius, maxHeight + 1, _halfOfArenaWidth), TempColliders, Quaternion.identity,
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
	}
}