using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Ball ball;
	private static GameManager _shared;

	private void Awake()
	{
		_shared = this;
	}
}