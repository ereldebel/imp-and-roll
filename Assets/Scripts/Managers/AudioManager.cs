using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class AudioManager : ObjectAudio
	{
		[SerializeField] private AudioClip startMusic;
		[SerializeField] private AudioClip matchMusic;
		[SerializeField] private AudioClip winMusic;

		private static AudioManager _shared;

		protected override void Awake()
		{
			if (_shared)
			{
				if (SceneManager.GetActiveScene().buildIndex != 0)
				{
					Destroy(this);
					return;
				}

				if (_shared)
					Destroy(_shared);
			}

			_shared = this;
			DontDestroyOnLoad(_shared.gameObject);
			base.Awake();
		}

		public static void StartScreenMusic() => _shared.SwitchClip(_shared.startMusic);
		public static void MatchMusic() => _shared.SwitchClip(_shared.matchMusic);
		public static void WinMusic() => _shared.SwitchClip(_shared.winMusic);
	}
}