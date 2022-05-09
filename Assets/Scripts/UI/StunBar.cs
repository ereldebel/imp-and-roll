using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StunBar : MonoBehaviour
	{
		[SerializeField] private PlayerBrain playerBrain;
		[SerializeField] private Sprite stunnedFace;
		[SerializeField] private Material bar;

		private Sprite _regularFace;
		private Image _image;
		private static readonly int BarPercentage = Shader.PropertyToID("BorderX");

		private void Awake()
		{
			if (!playerBrain.gameObject.activeSelf)
				gameObject.SetActive(false);
			bar.SetFloat(BarPercentage, 1);
			_image = GetComponent<Image>();
			_regularFace = _image.sprite;
			playerBrain.StunStarted += StunStarted;
			playerBrain.StunEnded += StunEnded;
		}

		private void OnDestroy()
		{
			playerBrain.StunStarted -= StunStarted;
			playerBrain.StunEnded -= StunEnded;
		}

		private void StunStarted(float percentage)
		{
			_image.sprite = stunnedFace;
			bar.SetFloat(BarPercentage, percentage);
		}

		private void StunEnded()
		{
			_image.sprite = _regularFace;
		}
	}
}