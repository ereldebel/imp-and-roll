using UnityEngine;

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

	#region Private Fields

	private static GameManager _shared;

	private float _borderX = 0.5f;
	private Material _arenaMaterial;
	private float _arenaWidth;
	private const float PlaneWidth = 10;
	private int _shaderBorderXVar;

	#endregion

	#region Function Events

	private void Awake()
	{
		_shared = this;
		_arenaMaterial = arena.GetComponent<Renderer>().material;
		_arenaWidth = arena.transform.localScale.x * PlaneWidth;
		_shaderBorderXVar = Shader.PropertyToID(arenaMaterialBorderXValueName);
		_arenaMaterial.SetFloat(_shaderBorderXVar, _borderX);
	}

	private void OnValidate()
	{
		borderRelativeWidth /= 2;
	}

	private void Update()
	{
		UpdateBorder();
	}

	#endregion

	#region Private Methods

	private void UpdateBorder()
	{
		if (!ball.Grounded) return;
		var normalizedXPosition = ball.XPosition / _arenaWidth + 0.5f;
		if (Mathf.Abs(normalizedXPosition - _borderX) < borderRelativeWidth) return;
		var change = Time.deltaTime * borderChangeSpeed;
		_borderX += normalizedXPosition < _borderX ? -change : change;
		_arenaMaterial.SetFloat(_shaderBorderXVar, _borderX);
		border.position = new Vector3(_arenaWidth * (_borderX - 0.5f), 0.5f, 0);
	}

	#endregion
}