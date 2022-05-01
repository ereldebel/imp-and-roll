using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	#region Serialized Fields

	[SerializeField] private Ball ball;
	[SerializeField] private GameObject arena;
	[SerializeField] private Transform border;
	[SerializeField] private string arenaMaterialBorderXValueName = "BorderX";
	[SerializeField] private float borderRelativeWidth;
	[SerializeField] private float borderChangeSpeed = 0.01f;

	#endregion

	#region Public Properties

	public static float ArenaWidth => _shared._arenaWidth;

	#endregion

	#region Private Fields

	private static GameManager _shared;

	private float _borderX = 0.5f;
	private Material _arenaMaterial;
	private const float PlaneWidth = 10;
	private float _arenaWidth;
	private int _shaderBorderXVar;
	private float _borderRelativeWidth;
	private bool _ballOnRight;

	#endregion

	#region Function Events

	private void Awake()
	{
		_shared = this;
		_arenaMaterial = arena.GetComponent<Renderer>().material;
		_arenaWidth = arena.transform.localScale.x * PlaneWidth;
		_shaderBorderXVar = Shader.PropertyToID(arenaMaterialBorderXValueName);
		_arenaMaterial.SetFloat(_shaderBorderXVar, _borderX);
		OnValidate();
	}

	private void OnValidate()
	{
		_borderRelativeWidth = borderRelativeWidth / 2;
	}

	private void Update()
	{
		UpdateBorder();
	}

	#endregion

	#region Public Methods

	public static void MonsterGotBall()
	{
		GameOver(!_shared._ballOnRight);
	}
	
	public static void MonsterGotPlayer(bool rightLost)
	{
		GameOver(!rightLost);
	}

	#endregion

	#region Private Methods

	private static void GameOver(bool rightWon)
	{
		var player = rightWon ? "right player" : "left player";
		print($"{player} won!");
		SceneManager.LoadScene(0);
	}
	
	private void UpdateBorder()
	{
		if (!ball.Grounded) return;
		var normalizedXPosition = ball.XPosition / _arenaWidth + 0.5f;
		if (Mathf.Abs(normalizedXPosition - _borderX) < _borderRelativeWidth) return;
		var change = Time.deltaTime * borderChangeSpeed;
		_ballOnRight = normalizedXPosition > _borderX;
		_borderX += _ballOnRight ? change : -change;
		_arenaMaterial.SetFloat(_shaderBorderXVar, _borderX);
		border.position = new Vector3(_arenaWidth * (_borderX - 0.5f), 0.5f, 0);
	}

	#endregion
}