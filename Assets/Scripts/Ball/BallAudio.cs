using UnityEngine;

namespace Ball
{
	public class BallAudio : ObjectAudio
	{
		[Header("Clips by index:  0-3: Soft Bounce, 4: Hard Bounce, 5: Explosion")] [SerializeField]
		private Header _;

		#region Public methods

		public void SoftBounce() => PlayClipByIndex(Random.Next(0,4));
		public void HardBounce() => PlayClipByIndex(4);
		public void Explosion() => PlayClipByIndex(5);

		#endregion
	}
}