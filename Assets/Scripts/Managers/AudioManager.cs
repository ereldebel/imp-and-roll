namespace Managers
{
	public class AudioManager : ObjectAudio
	{
		private static AudioManager _shared;

		protected override void Awake()
		{
			if (_shared)
			{
				Destroy(this);
				return;
			}

			_shared = this;
			base.Awake();
		}

		public static void StartGameMusic()
		{
			_shared.Audio.Stop();
			_shared.Audio.Play();
		}
	}
}