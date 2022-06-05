using UnityEngine;

namespace Ball
{
	public class BallAudio : ObjectAudio
	{
		[Header("Clips by index:  0: Soft Bounce, 1: Hard Bounce")] [SerializeField]
		private Header _;

		#region Public methods

		public void SoftBounce() => PlayClipByIndex(0);
		public void HardBounce() => PlayClipByIndex(1);

		#endregion
	}
}