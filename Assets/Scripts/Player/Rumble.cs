using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class Rumble : MonoBehaviour
	{
		[Header("Rumbles Settings")] [Tooltip("LowFreq, HighFreq, RumbleDuration")] [SerializeField]
		private RumbleValues stunRumble = new RumbleValues(0.3f, 0.5f, 0.3f);

		[SerializeField] private float maxStunLowFreqRumbleAddition = 0.3f;
		[SerializeField] private float maxStunRumbleDurationAddition = 0.5f;

		[Tooltip("LowFreq, HighFreq")] [SerializeField]
		private RumbleValues aimRumble = new RumbleValues(0.05f, 0.1f, 0.1f);

		private Gamepad _myGamepad;

		// private List<RumbleCall> _rumbleCalls = new List<RumbleCall>();
		private float _aimEndTime = -1;

		private void Awake()
		{
			var playerInput = GetComponent<PlayerInput>();
			if (playerInput == null)
				Destroy(this);
			_myGamepad = Gamepad.all.FirstOrDefault(g => playerInput.devices.Any(d => d.deviceId == g.deviceId));
		}

		private void Update()
		{
			if (_aimEndTime < 0 || _aimEndTime >= Time.time) return;
			_aimEndTime = -1;
			StopRumble();
		}

		public void AimRumblePulse()
		{
			if (_aimEndTime < 0)
				StartRumble(aimRumble.lowFreq, aimRumble.highFreq);
			_aimEndTime = Time.time + aimRumble.duration;
		}

		public void Stun(float stunBar)
		{
			RumblePulse(stunRumble.lowFreq + (1 - stunBar) * maxStunLowFreqRumbleAddition, stunRumble.highFreq,
				stunRumble.duration + (1 - stunBar) * maxStunRumbleDurationAddition);
		}

		private void RumblePulse(float lowFreq, float highFreq, float rumbleDur)
		{
			StartRumble(lowFreq, highFreq);
			Invoke(nameof(StopRumble), rumbleDur);
		}

		private void StartRumble(float lowFreq, float highFreq)
		{
			_myGamepad?.SetMotorSpeeds(lowFreq, highFreq);
		}

		private void StopRumble()
		{
			_myGamepad?.SetMotorSpeeds(0, 0);
		}

		[Serializable]
		private struct RumbleValues
		{
			public float lowFreq, highFreq, duration;

			public RumbleValues(float lowFreq, float highFreq, float duration)
			{
				this.lowFreq = lowFreq;
				this.highFreq = highFreq;
				this.duration = duration;
			}
		}
	}
}