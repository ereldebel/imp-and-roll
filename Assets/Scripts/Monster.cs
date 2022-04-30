using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
	[SerializeField] private float sightRadius = 5;

	[SerializeField] private LayerMask playerMask;
	[SerializeField] private float speed = 1;

	private static readonly Collider[] TempColliders = new Collider[2];

	private void Update()
	{
		var playersSeen =
			Physics.OverlapSphereNonAlloc(transform.position, sightRadius, TempColliders, playerMask.value);
		if (playersSeen <= 0) return;
		var pos = transform.position;
		var closestPos = pos;
		var closestDist = Mathf.Infinity;
		for (int i = 0; i < playersSeen; ++i)
		{
			var otherPos = TempColliders[i].transform.position;
			var dist = Vector3.Distance(pos, TempColliders[i].transform.position);
			if (closestDist <= dist) continue;
			closestDist = dist;
			closestPos = otherPos;
		}

		var zRelativeChange = (closestPos - pos).normalized.z;
		var localPos = transform.localPosition;
		localPos.z += Time.deltaTime * speed * zRelativeChange;
		transform.localPosition = localPos;
	}

	private void OnCollisionEnter(Collision collision)
	{
		print($"{collision.gameObject.name} lost");
		SceneManager.LoadScene(0);
	}
}