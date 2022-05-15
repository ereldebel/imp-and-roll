using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private Ball ball;
	[SerializeField] private GameObject arena;
	[SerializeField] private Transform playerOne;
	[SerializeField] private Transform divisionBorder;
	#endregion

	#region Public Properties

	public static Transform BallTransform => _shared.ball.transform;
	public static float ArenaLength => _shared._arenaDimensions[0];
	public static float ArenaWidth => _shared._arenaDimensions[1];
	public static Transform PlayerOne => _shared.playerOne;
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
		_shared.playerOne = GameObject.FindGameObjectWithTag("Player 1").transform;
	}

	#endregion

	#region Public Methods

	public static void GameOver(bool rightLost)
	{
		var player = rightLost ? "left player" : "right player";
		print($"{player} won!");
		SceneManager.LoadScene(0);
	}

	#endregion
}