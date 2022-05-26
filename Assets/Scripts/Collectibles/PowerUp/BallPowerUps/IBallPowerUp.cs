﻿namespace Collectibles.PowerUp.BallPowerUps
{
	public interface IBallPowerUp: IPowerUp
	{
		void OnThrow(Ball ball);
		void OnLateUpdate();
		bool OnHit();
		void End();
	}
}