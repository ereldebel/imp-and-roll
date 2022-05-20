using System;
using System.Collections;
using System.Collections.Generic;
using Collectibles.GlobalPowerUps;
using UnityEngine;

namespace Managers
{
	public class MatchManager : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private Ball ball;
		[SerializeField] private GameObject arena;
		[SerializeField] private Transform divisionBorder;

		#endregion

		#region Public Properties

		public static Transform BallTransform => _shared.ball ? _shared.ball.transform : _shared.transform;
		public static float ArenaLength => _shared._arenaDimensions[0];
		public static float ArenaWidth => _shared._arenaDimensions[1];
		public static Transform DivisionBorder => _shared.divisionBorder;
		public static IEnumerable<Transform> CollectibleCollection => _shared._collectibleCollection;

		#endregion

		#region Private Fields

		private static MatchManager _shared;

		private const float PlaneWidth = 10;
		private Vector2 _arenaDimensions;

		private readonly List<Transform> _collectibleCollection = new List<Transform>();

		private readonly Dictionary<IGlobalPowerUp, Coroutine> _globalPowerUps =
			new Dictionary<IGlobalPowerUp, Coroutine>();

		#endregion

		#region Public C# Events

		public static event Action MatchEnded;

		#endregion

		#region Function Events

		private void Awake()
		{
			_shared = this;
			var scale = arena.transform.localScale;
			_arenaDimensions = new Vector2(scale.x * PlaneWidth, scale.y * PlaneWidth);
			GameManager.Shared.AwakeAI();
		}

		private void Update()
		{
			foreach (var globalPowerUp in _globalPowerUps.Keys)
				globalPowerUp.OnUpdate();
		}

		private void OnDestroy()
		{
			foreach (var globalPowerUp in _globalPowerUps)
			{
				StopCoroutine(globalPowerUp.Value);
				globalPowerUp.Key.End();
			}
		}

		#endregion

		#region Public Methods

		public static void GameOver(bool rightLost)
		{
			MatchEnded?.Invoke();
			var player = rightLost ? "left player" : "right player";
			print($"{player} won!");
			GameManager.Shared.PlayerWon(rightLost);
		}

		public static void AddGlobalPowerUp(IGlobalPowerUp powerUp)
		{
			_shared._globalPowerUps.Add(powerUp, _shared.StartCoroutine(PowerUpLifeSpan(powerUp)));
		}

		public static void AddToCollectibleCollection(Transform collectibleTransform)
		{
			_shared._collectibleCollection.Add(collectibleTransform);
		}

		public static void RemoveFromCollectibleCollection(Transform collectibleTransform)
		{
			_shared._collectibleCollection.Remove(collectibleTransform);
		}

		#endregion

		#region Private Coroutines

		private static IEnumerator PowerUpLifeSpan(IGlobalPowerUp powerUp)
		{
			yield return new WaitForSeconds(powerUp.StartAndGetDuration());
			powerUp.End();
			_shared._globalPowerUps.Remove(powerUp);
		}

		#endregion
	}
}