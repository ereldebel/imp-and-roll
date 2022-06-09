using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Collectibles;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
	public class MatchManager : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private Ball.Ball ball;
		[SerializeField] private GameObject arena;
		[SerializeField] private Transform divisionBorder;
		[SerializeField] private GameObject powerUpPrefab;
		[SerializeField] private float timeBetweenPowerUpSpawns;
		[SerializeField] private float minPowerUpDistFromPlayers;

		[Tooltip("Duplicates mean that the powerUp has a higher chance of being picked.")] [SerializeField]
		private List<CollectibleType> powerUpsInMatch;

		#endregion

		#region Public Properties

		public static Transform BallTransform =>
			_shared ? (_shared.ball ? _shared.ball.transform : _shared.transform) : null;

		public static float ArenaLength => _shared ? _shared._arenaDimensions[0] : 100;
		public static float ArenaWidth => _shared ? _shared._arenaDimensions[1] : 100;
		public static float MaxDistance => _shared ? (_shared != null ? _shared._diagonal : 50) : 100;
		public static Transform DivisionBorder => _shared ? _shared.divisionBorder : null;
		public static IEnumerable<Transform> CollectibleCollection => _shared ? _shared._collectibleCollection : null;

		#endregion

		#region Private Fields

		private static MatchManager _shared;

		private const float PlaneWidth = 10;
		private Vector2 _arenaDimensions;
		private Coroutine _spawner;
		private float _diagonal;

		private readonly List<Transform> _collectibleCollection = new List<Transform>();

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
			_diagonal = Mathf.Sqrt(Mathf.Pow(_arenaDimensions.x, 2) + Mathf.Pow(_arenaDimensions.y, 2));
		}

		private void Start()
		{
			StartCoroutine(InitializeBallWithDelay(1.5f));
		}

		private void OnDestroy()
		{
			if (ball)
				Destroy(ball.gameObject);
		}

		#endregion

		#region Public Methods

		public static void GameOver(bool leftWon)
		{
			_shared.StopCoroutine(_shared._spawner);
			var winningPlayerIndex = leftWon ? 1 : 0; // assumes the right player is player 0.
			GameManager.Players[winningPlayerIndex].GetComponent<PlayerBrain>()?.GameOver(true);
			GameManager.Players[1 - winningPlayerIndex].GetComponent<PlayerBrain>()?.GameOver(false);
			GameManager.Shared.PlayerWon(leftWon);
			MatchEnded?.Invoke();
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
				if (i++ < 30) break;
			} while (GameManager.Players.Any(player =>
				         Vector3.Distance(player.transform.position, output) < minPowerUpDistFromPlayers));

			return output;
		}

		#endregion

		#region Private Coroutines

		private IEnumerator InitializeBallWithDelay(float delay)
		{
			ball.FreezeBall();
			yield return new WaitForSeconds(delay);
			var blueRedDiff = GameManager.BlueScore - GameManager.RedScore;
			var x = Random.Range(2, 4);
			var z = Random.Range(-2, 2);
			if (blueRedDiff < 0 || (blueRedDiff == 0 && Random.value > 0.5f))
				x = -x;
			ball.UnfreezeAndInitializeBall(new Vector3(x, 2, z));
		}

		private IEnumerator SpawnCollectible()
		{
			var halfOfArenaWidth = ArenaWidth / 2;
			var halfOfArenaLength = ArenaLength / 2;
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
				spawnPoint.y = powerUpPrefab.transform.position.y;
				var newPowerUp = Instantiate(powerUpPrefab, spawnPoint, powerUpPrefab.transform.rotation);
				var next = powerUpsInMatch[random.Next(powerUpsInMatch.Count)];
				newPowerUp.GetComponent<Collectible>().CollectibleType = next;
				spawnOnRight = random.Next(numOfSpawns) < numOfLeftSpawns;
				AudioManager.PowerUpSpawn();
			}
		}

		#endregion
	}
}