using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps.Effects
{
	public class FireDropper : MonoBehaviour
	{
		[SerializeField] private CollectibleFactory collectibleFactory;

		private float _enableTime;
		private float _effectTime;
		private ParticleSystem _particleSystem;
		private bool _stopped;

		public void Stop()
		{
			_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			_stopped = true;
		}

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

		private void OnEnable()
		{
			_particleSystem.Play();
			_enableTime = Time.time;
			_effectTime = collectibleFactory.fireTrailParticleLifeSpan;
			_stopped = false;
		}

		private void Update()
		{
			if (_stopped && Time.time - _enableTime > _effectTime)
				gameObject.SetActive(false);
		}
	}
}