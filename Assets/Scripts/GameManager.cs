using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private Ball ball;
	[SerializeField] private GameObject arena;
	[SerializeField] private Transform divisionBorder;
	[SerializeField] private GameObject hands;

	#endregion

	#region Public Properties

	public static Transform BallTransform => _shared.ball ? _shared.ball.transform : _shared.transform;
	public static float ArenaLength => _shared._arenaDimensions[0];
	public static float ArenaWidth => _shared._arenaDimensions[1];
	public static Transform DivisionBorder => _shared.divisionBorder;

	#endregion

	#region Private Fields

	private static GameManager _shared;

	private const float PlaneWidth = 10;
	private Vector2 _arenaDimensions;

	#endregion

	#region Function Events

	private void Awake()
	{
		_shared = this;
		var scale = arena.transform.localScale;
		_arenaDimensions = new Vector2(scale.x * PlaneWidth, scale.y * PlaneWidth);
	}

	#endregion

	#region Public Methods

	public static void GameStarted()
	{
		_shared.hands.SetActive(true);
	}
	
	public static void GameOver(bool rightLost)
	{
		var player = rightLost ? "left player" : "right player";
		print($"{player} won!");
		CrossSceneManager.Shared.PlayerWon(rightLost);
	}

	#endregion
}