using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Collectibles;
using Collectibles.PowerUp.GlobalPowerUps;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
	public class MatchManager : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private Ball ball;
		[SerializeField] private GameObject arena;
		[SerializeField] private Transform divisionBorder;
		[SerializeField] private GameObject powerUpPrefab;
		[SerializeField] private float timeBetweenPowerUpSpawns;
		[SerializeField] private float minPowerUpDistFromPlayers;

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
		private Coroutine _spawner;

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
			_spawner = StartCoroutine(SpawnCollectible());
		}

		private void Update()
		{
			foreach (var globalPowerUp in _globalPowerUps.Keys)
				globalPowerUp.OnUpdate();
		}

		private void OnDestroy()
		{
			if (ball)
				Destroy(ball.gameObject);
			foreach (var globalPowerUp in _globalPowerUps)
			{
				StopCoroutine(globalPowerUp.Value);
				globalPowerUp.Key.End();
			}
		}

		#endregion

		#region Public Methods

		public static void GameOver(bool leftWon)
		{
			_shared.StopCoroutine(_shared._spawner);
			var winningPlayerIndex = leftWon ? 1 : 0; // assumes the right player is player 0.
			GameManager.Players[winningPlayerIndex].GetComponent<PlayerBrain>()?.GameOver(true);
			GameManager.Players[1 - winningPlayerIndex].GetComponent<PlayerBrain>()?.GameOver(false);
			MatchEnded?.Invoke();
			var player = leftWon ? "left player" : "right player";
			print($"{player} won!");
			_shared.StartCoroutine(EndMatchWithDelay(leftWon, 3));
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

		#region Private Methods

		private Vector3 RandomXZVector(Vector2 min, Vector2 max, float padding)
		{
			Vector3 output;
			var i = 0;
			do
			{
				var x = Random.Range(min.x + padding, max.x - padding);
				var z = Random.Range(min.y + padding, max.y - padding);
				output = new Vector3(x, 0, z);
				if (i++ < 10) break;
			} while (GameManager.Players.Any(player =>
				         Vector3.Distance(player.transform.position, output) < minPowerUpDistFromPlayers));

			return output;
		}

		#endregion

		#region Private Coroutines

		private static IEnumerator PowerUpLifeSpan(IGlobalPowerUp powerUp)
		{
			yield return new WaitForSeconds(powerUp.StartAndGetDuration());
			powerUp.End();
			_shared._globalPowerUps.Remove(powerUp);
		}

		private static IEnumerator EndMatchWithDelay(bool leftWon, float delay)
		{
			yield return new WaitForSeconds(delay);
			GameManager.Shared.PlayerWon(leftWon);
		}

		private IEnumerator SpawnCollectible()
		{
			var halfOfArenaWidth = ArenaWidth / 2;
			var halfOfArenaLength = ArenaLength / 2;
			var allPowerUps = Enum.GetValues(typeof(CollectibleType));
			var random = new System.Random();
			var numOfSpawns = 0;
			var numOfLeftSpawns = 0;
			var spawnOnRight = random.Next(2) == 0;
			while (enabled)
			{
				yield return new WaitForSeconds(timeBetweenPowerUpSpawns);
				++numOfSpawns;
				float minX, maxX;
				if (spawnOnRight)
				{
					minX = divisionBorder.position.x;
					maxX = halfOfArenaLength;
				}
				else
				{
					minX = -halfOfArenaLength;
					maxX = divisionBorder.position.x;
					++numOfLeftSpawns;
				}

				var minVector = new Vector2(minX, -halfOfArenaWidth);
				var maxVector = new Vector2(maxX, halfOfArenaWidth);
				var spawnPoint = RandomXZVector(minVector, maxVector, 0.1f);
				var newPowerUp = Instantiate(powerUpPrefab, spawnPoint, powerUpPrefab.transform.rotation);
				var next = (CollectibleType) allPowerUps.GetValue(random.Next(allPowerUps.Length));
				newPowerUp.GetComponent<Collectible>().CollectibleType = next;
				print(next);
				spawnOnRight = random.Next(numOfSpawns) < numOfLeftSpawns;
			}
		}

		#endregion
	}
}