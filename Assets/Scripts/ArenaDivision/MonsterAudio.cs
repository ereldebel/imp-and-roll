using UnityEngine;

namespace ArenaDivision
{
	public class MonsterAudio : ObjectAudio
	{
		[Header("Clips by index:  0: Sneeze, 1: Acceleration roar, 2: Attack")] [SerializeField]
		private Header _;

		#region Public methods

		public void Sneeze() => PlaySingleClipByIndex(0);
		public void Accelerate() => PlaySingleClipByIndex(1);
		public void Attack() => PlaySingleClipByIndex(2);
		public void Stop() => Audio.Stop();

		#endregion

	}
}