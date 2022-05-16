using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StunBar : MonoBehaviour
	{
		[SerializeField] private int playerNumber;
		[SerializeField] private Sprite stunnedFace;
		[SerializeField] private Material bar;
		[SerializeField] private float[] barMaxMin = {0.3f, 0.5f};
		
		private PlayerBrain _playerBrain;
		private Sprite _regularFace;
		private Image _image;
		private static readonly int BarPercentage = Shader.PropertyToID("BorderX");

		private void Awake()
		{
			_playerBrain = GameManager.Players[playerNumber].GetComponent<PlayerBrain>();
			bar.SetFloat(BarPercentage, 1);
			_image = GetComponent<Image>();
			_regularFace = _image.sprite;
			_playerBrain.StunStarted += StunStarted;
			_playerBrain.StunEnded += StunEnded;
		}
		
		private void OnDestroy()
		{
			bar.SetFloat(BarPercentage, 1);
			_playerBrain.StunStarted -= StunStarted;
			_playerBrain.StunEnded -= StunEnded;
		}

		private void StunStarted(float percentage)
		{
			_image.sprite = stunnedFace;
			bar.SetFloat(BarPercentage,  Mathf.Lerp(barMaxMin[0],barMaxMin[1],percentage));
		}

		private void StunEnded()
		{
			_image.sprite = _regularFace;
		}
	}
}