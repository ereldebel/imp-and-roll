using UnityEngine;

namespace Player
{
	public class PlayerAudio : ObjectAudio
	{
		[Header("Clips by index:  0: Charge, 1: Throw, 2: Roll, 3: Roll End, 4-6: Hits, 7: Run")] [SerializeField]
		private Header _;

		#region Public methods

		public void Charge() => PlaySingleClipByIndex(0);

		public void Throw() => PlaySingleClipByIndex(1);

		public void Roll()
		{
			PlaySingleClipByIndex(2);
			PlayClipByIndex(3);
		}

		public void Hit() => PlaySingleClipByIndex(Random.Next(4, 7));

		public void Die() => PlaySingleClipByIndex(7);

		public void Running(bool play)
		{
			if (play)
				PlayClipByIndex(8);
			else
				Audio.Stop();
		}

		#endregion
	}
}