using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		[SerializeField] private float sightRadius = 1;
		[SerializeField] private LayerMask playerMask;
		[SerializeField] private float speed = 1;
		[SerializeField] private float ySpeed = 0.5f;
		[SerializeField] private float maxHeight = 5;
		[SerializeField] private float chainRemainder = 0.1f;
		[SerializeField] private Rigidbody divider;
		[SerializeField] private GameObject chainLinkPrefab;

		private Rigidbody _rigidBody;
		private float _halfOfArenaWidth;
		private Transform _ball;
		private float _chainLength;
		private float _fixedSpeed;
		private float _fixedYSpeed;

		private static readonly Collider[] TempColliders = new Collider[3];

		private void Awake()
		{
			_rigidBody = GetComponent<Rigidbody>();
			OnValidate();
			CreateChain();
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
		}

		private void OnCollisionEnter(Collision collision)
		{
			var other = collision.gameObject;
			if (other.CompareTag("Ball"))
				GameManager.MonsterGotBall();
			else
				GameManager.MonsterGotPlayer(other.name.EndsWith("1"));
		}

		private void CreateChain()
		{
			Quaternion chainLinkRotation = chainLinkPrefab.transform.rotation;
			var prev = divider.GetComponent<HingeJoint>();
			for (float y = 0; y < _chainLength; y += 0.2f)
			{
				var curr = Instantiate(chainLinkPrefab, Vector3.up * y, chainLinkRotation);
				prev.connectedBody = curr.GetComponent<Rigidbody>();
				prev = curr.GetComponent<HingeJoint>();
			}

			prev.connectedBody = _rigidBody;
		}

		private void Move()
		{
			var pos = _rigidBody.position;
			var ballPos = _ball.position;
			var dividerPos = divider.position;
			var chasingCloseObject = GetClosestObject(pos, ballPos, out var closestObjDir);
			var ballDist = ballPos.x - pos.x;
			var xMovement = Mathf.Abs(ballDist) > 0.01f ? Mathf.Sign(ballDist) * _fixedSpeed : 0;
			var yMovement = chasingCloseObject
				? closestObjDir.y * _fixedYSpeed
				: Mathf.Min(maxHeight - pos.y, _fixedYSpeed);
			var zMovement = closestObjDir.z * _fixedSpeed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) > 0;
			if (movingTowardsDivider)
				xMovement *= 2;
			pos += new Vector3(xMovement, yMovement, zMovement);
			_rigidBody.position = pos;
			if (Vector3.Distance(pos, dividerPos) < _chainLength || movingTowardsDivider) return;
			dividerPos.x += xMovement;
			divider.position = dividerPos;
		}

		private bool GetClosestObject(Vector3 pos, Vector3 defaultObjPos, out Vector3 closestObjDir)
		{
			var playersSeen =
				Physics.OverlapBoxNonAlloc(Vector3.right * pos.x,
					new Vector3(sightRadius, maxHeight, _halfOfArenaWidth), TempColliders, Quaternion.identity,
					playerMask.value);
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

			closestObjDir = closestPos - pos;
			return playersSeen > 0;
		}
	}
}