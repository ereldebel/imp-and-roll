using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class AudioManager : ObjectAudio
	{
		[SerializeField] [Header("0: Pause, 1: Match start, 2: PowerUp spawn, 3: PowerUp pickup, 4: Counter")]
		private Header _;

		[SerializeField] private AudioClipAndVolume tutorialMusic;
		[SerializeField] private AudioClipAndVolume matchMusic;
		[SerializeField] private AudioClipAndVolume winMusic;
		[SerializeField] private AudioClipAndVolume openingScreenMusic;
		[SerializeField] private AudioSource pauseAudioSource;

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
			SetObjectSet(new HashSet<AudioSource> {Audio});
		}

		public static void TutorialScreenMusic() => _shared.SwitchClip(_shared.tutorialMusic);
		public static void MatchMusic() => _shared.SwitchClip(_shared.matchMusic);
		public static void WinMusic() => _shared.SwitchClip(_shared.winMusic);
		public static void OpeningScreenMusic() => _shared.SwitchClip(_shared.openingScreenMusic);

		public static void Pause()
		{
			foreach (var audio in AudioSources)
				audio.Pause();
			var sound = _shared.clips[0];
			_shared.pauseAudioSource.PlayOneShot(sound.clip, sound.volume);
		}

		public static void Resume()
		{
			foreach (var audio in AudioSources)
				audio.UnPause();
		}

		public static void MatchStart()
		{
			_shared.PlayClipByIndex(1);
		}

		public static void PowerUpSpawn()
		{
			_shared.PlayClipByIndex(2);
		}
		
		public static void PowerUpPickUp()
		{
			_shared.PlayClipByIndex(3);
		}
		
		public static void Counter()
		{
			_shared.PlayClipByIndex(4);
		}
		
	}
}