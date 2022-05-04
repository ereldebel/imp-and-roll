using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
	public class Sparks : MonoBehaviour
	{
		private ParticleSystem.MainModule _main;
		private ParticleSystem.EmissionModule _emission;

		private bool _change;

		private void Awake()
		{
			var particleSystem = GetComponent<ParticleSystem>();
			_main = particleSystem.main;
			_emission = particleSystem.emission;
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
				var nextSpeed = 0.75f + Random.value;
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
				var nextMultiplier = Random.Range(3.5f,10);
				while (_change && Math.Abs(_emission.rateOverTimeMultiplier - nextMultiplier) > 0.01)
				{
					_emission.rateOverTimeMultiplier = Mathf.Lerp(_emission.rateOverTimeMultiplier, nextMultiplier, 0.01f);
					yield return new WaitForFixedUpdate();
				}
			}
		}
	}
}