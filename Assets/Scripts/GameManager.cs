using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Ball ball;
	[SerializeField] private GameObject arena;
	[SerializeField] private string arenaMaterialBorderXValueName = "BorderX";
	[SerializeField] private float borderRelativeWidth;
	[SerializeField] private float borderChangeSpeed = 0.01f;

	private static GameManager _shared;

	private float _borderX = 0.5f;
	private Material _arenaMaterial;
	private float _arenaWidth;
	private const float PlaneWidth = 10;
	private int _shaderBorderXVar;

	private void Awake()
	{
		_shared = this;
		_arenaMaterial = arena.GetComponent<Renderer>().material;
		_arenaWidth = arena.transform.localScale.x * PlaneWidth;
		_shaderBorderXVar = Shader.PropertyToID(arenaMaterialBorderXValueName);
	}

	private void OnValidate()
	{
		borderRelativeWidth /= 2;
	}

	private void Update()
	{
		UpdateBorder();
	}

	private void UpdateBorder()
	{
		if (!ball.Grounded) return;
		var normalizedXPosition = ball.XPosition / _arenaWidth + 0.5f;
		if (Mathf.Abs(normalizedXPosition - _borderX) < borderRelativeWidth) return;
		var change = Time.deltaTime * borderChangeSpeed;
		_borderX += normalizedXPosition < _borderX ? -change : change;
		_arenaMaterial.SetFloat(_shaderBorderXVar, _borderX);
	}
}