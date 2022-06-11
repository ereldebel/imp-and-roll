using Managers;
using UnityEngine;

namespace Reflections
{
	public class BallReflection : MonoBehaviour
	{
		private MeshRenderer _myMeshRenderer, _daddyMeshRenderer;
		private MeshFilter _myMeshFilter, _daddyMeshFilter;
		private Transform _myT, _daddyT;
		private Ball.Ball _ball;

		private void OnEnable()
		{
			_myT = transform;
			_myMeshRenderer = GetComponent<MeshRenderer>();
			_myMeshFilter = GetComponent<MeshFilter>();
			_daddyMeshRenderer = MatchManager.BallTransform.GetComponent<MeshRenderer>();
			_daddyT = _daddyMeshRenderer.transform;
			_ball = _daddyT.GetComponent<Ball.Ball>();
			_daddyMeshFilter = _daddyT.GetComponent<MeshFilter>();
		}

		private void Update()
		{
			if (!_daddyT.gameObject.activeSelf || _ball.Held)
			{
				_myMeshRenderer.enabled = false;
				return;
			}

			_myMeshRenderer.enabled = true;
			var newPos = _daddyT.position;
			newPos.y *= -1;
			_myT.position = newPos;
			_myT.localScale = _daddyT.localScale;
			_myT.rotation = Quaternion.AngleAxis(180, Vector3.forward) * _daddyT.rotation;
			_myMeshFilter.mesh = _daddyMeshFilter.mesh;
			if (_daddyMeshRenderer.material != _myMeshRenderer.material)
				_myMeshRenderer.material = _daddyMeshRenderer.material;
			if (GameManager.CurScene != 2)
				Destroy(gameObject);
		}
	}
}