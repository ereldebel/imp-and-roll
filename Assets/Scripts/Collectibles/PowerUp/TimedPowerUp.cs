namespace Collectibles.PowerUp
{
	public abstract class TimedPowerUp : PowerUp
	{
		private readonly float _duration;

		protected TimedPowerUp(float duration, CollectibleType type):base(type)
		{
			_duration = duration;
		}

		public virtual float StartAndGetDuration()
		{
			base.Start();
			return _duration;
		}
	}
}