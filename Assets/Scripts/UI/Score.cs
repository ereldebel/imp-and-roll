using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class Score : MonoBehaviour
	{
		[SerializeField] private EPlayer player;

		[SerializeField] private Sprite[] scoreSprites;
		
		private Image _image; 

		private enum EPlayer
		{
			BluePlayer,
			RedPlayer
		}

		private void Awake()
		{
			MatchManager.MatchEnded += UpdateUI;
			_image = GetComponent<Image>();
			UpdateUI();
		}
		
		private void OnDestroy()
		{
			MatchManager.MatchEnded -= UpdateUI;
		}

		private void UpdateUI()
		{
			var score = player == EPlayer.BluePlayer ? GameManager.BlueScore : GameManager.RedScore; 
			_image.sprite = scoreSprites[score];
		}
	}
}