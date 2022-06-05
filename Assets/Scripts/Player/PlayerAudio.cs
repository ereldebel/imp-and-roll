using UnityEngine;

namespace Player
{
	public class PlayerAudio : ObjectAudio
	{
		[Header(
			"0: Charge, 1: Throw, 2: Roll, 3: Roll End, 4-6: Hits, 7: Die, 8: Run, 9: Haha, 10:Yeah, 11: Nonono, 12: Snap")]
		[SerializeField]
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

		public void Laugh() => PlaySingleClipByIndex(9);
		public void Ready() => PlaySingleClipByIndex(10);
		public void Unready() => PlaySingleClipByIndex(11);
		public void Snap() => PlaySingleClipByIndex(12);

		#endregion
	}
}