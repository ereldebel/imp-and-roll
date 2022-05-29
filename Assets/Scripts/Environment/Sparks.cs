using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
	public class Sparks : MonoBehaviour
	{
		[SerializeField] private float minRelativeSpeed;
		[SerializeField] private float maxRelativeSpeed;
		[SerializeField] private float minSparkRate = 3.5f;
		[SerializeField] private float maxSparkRate = 10;

		private ParticleSystem.MainModule _main;
		private ParticleSystem.EmissionModule _emission;

		private bool _change;

		private void Awake()
		{
			var particles = GetComponent<ParticleSystem>();
			_main = particles.main;
			_emission = particles.emission;
		}

		private void OnEnable()
		{
			_change = true;
			StartCoroutine(ChangeSpeed());
			StartCoroutine(ChangeRate());
		}

		private void OnDisable()
		{
			_change = false;
		}

		private IEnumerator ChangeSpeed()
		{
			while (_change)
			{
				yield return new WaitForSeconds(Random.Range(5, 10));
				var nextSpeed = Random.Range(minRelativeSpeed, maxRelativeSpeed);
				while (_change && Math.Abs(_main.simulationSpeed - nextSpeed) > 0.01)
				{
					_main.simulationSpeed = Mathf.Lerp(_main.simulationSpeed, nextSpeed, 0.01f);
					yield return new WaitForFixedUpdate();
				}
			}
		}

		private IEnumerator ChangeRate()
		{
			while (_change)
			{
				yield return new WaitForSeconds(Random.Range(5, 10));
				var nextMultiplier = Random.Range(minSparkRate, maxSparkRate);
				while (_change && Math.Abs(_emission.rateOverTimeMultiplier - nextMultiplier) > 0.01)
				{
					_emission.rateOverTimeMultiplier =
						Mathf.Lerp(_emission.rateOverTimeMultiplier, nextMultiplier, 0.01f);
					yield return new WaitForFixedUpdate();
				}
			}
		}
	}
}