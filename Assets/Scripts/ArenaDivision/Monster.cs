using System;
using UnityEngine;

namespace ArenaDivision
{
	public class Monster : MonoBehaviour
	{
		[SerializeField] private float sightRadius = 5;

		[SerializeField] private LayerMask playerMask;
		[SerializeField] private float speed = 1;
		[SerializeField] private float ySpeed = 1;
		[SerializeField] private float maxHeight = 5;
		[SerializeField] private float chainRemainder = 0.1f;
		[SerializeField] private Transform divider;

		private float _halfOfArenaWidth;
		private Transform _ball;
		private float _chainLength;
		private float _fixedSpeed;
		private float _fixedYSpeed;

		private static readonly Collider[] TempColliders = new Collider[3];

		private void Start()
		{
			_ball = GameManager.BallTransform;
			_halfOfArenaWidth = GameManager.ArenaWidth / 2;
			OnValidate();
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

		private void Move()
		{
			var pos = transform.position;
			var ballPos = _ball.position;
			var dividerPos = divider.position;
			var chasingCloseObject = GetClosestObject(pos, ballPos, out var closestObjDir);
			var xMovement = Mathf.Sign(ballPos.x - pos.x) * _fixedSpeed;
			var yMovement = chasingCloseObject ? closestObjDir.y * _fixedYSpeed : Mathf.Min(maxHeight - pos.y, _fixedYSpeed);
			var zMovement = closestObjDir.z * _fixedSpeed;
			var movingTowardsDivider = xMovement * (dividerPos.x - pos.x) >= 0;
			if (movingTowardsDivider)
				xMovement *= 2;
			pos += new Vector3(xMovement, yMovement, zMovement);
			transform.position = pos;
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

		private void OnCollisionEnter(Collision collision)
		{
			var other = collision.gameObject;
			if (other.CompareTag("Ball"))
				GameManager.MonsterGotBall();
			else
				GameManager.MonsterGotPlayer(other.name.EndsWith("1"));
		}
	}
}