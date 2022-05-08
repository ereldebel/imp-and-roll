using UnityEngine;

[ExecuteInEditMode]
public class CameraDistance : MonoBehaviour
{
	[SerializeField] private float dist;

	private void OnValidate()
	{
		if (dist == 0) return;
		transform.position = transform.position.normalized * dist;
	}
}