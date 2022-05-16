using UnityEngine;

public class SetCamera : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Canvas>().worldCamera=Camera.main;
	}
}
