using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Ball ball;
	[SerializeField] private Material arenaMaterial;
	[SerializeField] private string arenaMaterialBorderXValueName = "BorderX";
	[SerializeField] private float borderWidth;
	[SerializeField] private float borderChangeSpeed = 0.01f;

	private static GameManager _shared;

	private float _borderX = 0.5f;

	private void Awake()
	{
		_shared = this;
	}

	private void OnValidate()
	{
		borderWidth /= 2;
	}

	private void Update()
	{
		UpdateBorder();
	}

	private void UpdateBorder()
	{
		if (!ball.Grounded || Mathf.Abs(ball.XPosition - _borderX) < borderWidth) return;
		var change = Time.deltaTime * borderChangeSpeed;
		_borderX += ball.XPosition < _borderX ? change : -change;
		arenaMaterial.SetFloat(arenaMaterialBorderXValueName, _borderX);
	}
}