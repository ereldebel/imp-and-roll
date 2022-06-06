using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
	public class AudioManager : ObjectAudio
	{
		[SerializeField] [Header("0: Pause, 1: Match start, 2: PowerUp spawn")]
		private Header _;

		[SerializeField] private AudioClipAndVolume startMusic;
		[SerializeField] private AudioClipAndVolume matchMusic;
		[SerializeField] private AudioClipAndVolume winMusic;
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

		public static void StartScreenMusic() => _shared.SwitchClip(_shared.startMusic);
		public static void MatchMusic() => _shared.SwitchClip(_shared.matchMusic);
		public static void WinMusic() => _shared.SwitchClip(_shared.winMusic);

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
	}
}