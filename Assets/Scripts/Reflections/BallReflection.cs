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
		private int _texturePropertyID, _colorPropertyID;
		private void OnEnable()
		{
			_myT = transform;
			_myMeshRenderer = GetComponent<MeshRenderer>();
			_myMeshFilter = GetComponent<MeshFilter>();
			_daddyMeshRenderer = MatchManager.BallTransform.GetComponent<MeshRenderer>();
			_daddyT = _daddyMeshRenderer.transform;
			_ball = _daddyT.GetComponent<Ball.Ball>();
			_daddyMeshFilter = _daddyT.GetComponent<MeshFilter>();
			_texturePropertyID = _myMeshRenderer.material.shader.FindPropertyIndex("_MainTex");
			_texturePropertyID = _myMeshRenderer.material.shader.GetPropertyNameId(_texturePropertyID);
			_colorPropertyID = _myMeshRenderer.material.shader.FindPropertyIndex("_Color");
			_colorPropertyID = _myMeshRenderer.material.shader.GetPropertyNameId(_colorPropertyID);

		}

		private void Update()
		{
			if (!_daddyT.gameObject.activeSelf || !_ball.gameObject.activeSelf)
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
			if (_daddyMeshRenderer.materials.Length > 1)
			{
				for (int i = 0; i < _daddyMeshRenderer.materials.Length; i++)
				{
					_myMeshRenderer.materials[i].SetTexture(_texturePropertyID,_daddyMeshRenderer.materials[i].mainTexture);
					_myMeshRenderer.materials[i].SetColor(_colorPropertyID,_daddyMeshRenderer.materials[i].color);
				}
			}
			else
			{
				_myMeshRenderer.material.SetTexture(_texturePropertyID,_daddyMeshRenderer.material.mainTexture);
				_myMeshRenderer.material.SetColor(_colorPropertyID,_daddyMeshRenderer.material.color);
				for (int i = 1; i < _myMeshRenderer.materials.Length; i++)
				{
					_myMeshRenderer.materials[i].SetTexture(_texturePropertyID,Texture2D.blackTexture);
					_myMeshRenderer.materials[i].SetColor(_colorPropertyID,Color.white);
				}
			}


			if (GameManager.CurScene != 2)
				Destroy(gameObject);
		}
	}
}