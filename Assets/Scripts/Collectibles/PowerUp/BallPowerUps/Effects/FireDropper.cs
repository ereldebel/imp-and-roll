using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Collectibles.PowerUp.BallPowerUps.Effects
{
	public class FireDropper : MonoBehaviour
	{
		[SerializeField] private CollectibleFactory collectibleFactory;
		[SerializeField] private float flameKnockBackVelocityMultiplier = 2.5f;

		private float _enableTime;
		private float _effectTime;
		private ParticleSystem _particleSystem;
		private bool _stopped;
		private GameObject _flame;

		private static readonly List<ParticleCollisionEvent> CollisionEvents = new List<ParticleCollisionEvent>();

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
			_particleSystem.Stop();
			_flame = transform.GetChild(0).gameObject;
		}

		private void Update()
		{
			if (!_stopped || !(Time.time - _enableTime > _effectTime)) return;
			gameObject.SetActive(false);
			Destroy(gameObject, 5);
		}

		private void OnParticleCollision(GameObject other)
		{
			var player = other.GetComponent<IHittable>();
			if (player == null) return;
			var otherPos = other.transform.position;
			var numOfEvents = _particleSystem.GetCollisionEvents(other, CollisionEvents);
			var velocity = Vector3.zero;
			for (var i = 0; i < numOfEvents; ++i)
				velocity += CollisionEvents[i].intersection - otherPos;
			player.TakeHit(velocity * flameKnockBackVelocityMultiplier, true, 0.25f);
		}
		
		
		public void Play()
		{
			_particleSystem.Play();
			_enableTime = Time.time;
			_effectTime = collectibleFactory.fireTrailParticleLifeSpan;
			_stopped = false;
		}

		public void Stop()
		{
			_particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			_stopped = true;
			_flame.SetActive(false);
		}
	}
}