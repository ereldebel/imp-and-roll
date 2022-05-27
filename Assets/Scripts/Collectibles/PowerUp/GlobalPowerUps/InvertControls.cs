// using System.Transactions;
// using Managers;
// using Player;
// using UnityEngine;
//
// namespace Collectibles.PowerUp.GlobalPowerUps
// {
// 	public class InvertControls : TimedPowerUp, IGlobalPowerUp
// 	{
// 		private PlayerBrain _invertedPlayerBrain;
//
// 		private const CollectibleType PowerUpType = CollectibleType.InvertControls;
//
// 		public InvertControls(float duration) : base(duration, PowerUpType)
// 		{
// 		}
//
// 		public override void Collect(GameObject collector)
// 		{
// 			var targetObject = GameManager.Shared.GetOpposingPlayer(collector);
// 			base.Collect(targetObject);
// 			_invertedPlayerBrain = targetObject.GetComponent<PlayerBrain>();
// 		}
//
// 		public override float StartAndGetDuration()
// 		{
// 			_invertedPlayerBrain.InvertJoysticks = true;
// 			return base.StartAndGetDuration();
// 		}
//
// 		public void OnUpdate()
// 		{
// 		}
//
// 		public void End()
// 		{
// 			_invertedPlayerBrain.InvertJoysticks = false;
// 		}
// 	}
// }