using UnityEngine;

public class FaceCamera : MonoBehaviour
{
	private Transform _camera;

	private void Awake()
	{
		_camera = Camera.main.transform;
	}

	private void Update()
	{
		transform.LookAt(_camera);
	}
}