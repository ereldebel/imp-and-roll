﻿using System;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class StunBar : MonoBehaviour
	{
		[SerializeField] private int playerNumber;
		[SerializeField] private RotationRange rotationRange;

		private Material _material;
		private PlayerBrain _playerBrain;
		private static readonly int BarRotation = Shader.PropertyToID("_RotationAngle");

		[Serializable]
		private struct RotationRange
		{
			public float min, max;
		}

		private void Awake()
		{
			_playerBrain = GameManager.Players[playerNumber].GetComponent<PlayerBrain>();
			_material = GetComponent<Image>().material;
			_material.SetFloat(BarRotation, rotationRange.max);
			_playerBrain.StunStarted += StunStarted;
			MatchManager.MatchStarted += Reset;
		}

		private void Reset()
		{
			_material.SetFloat(BarRotation, rotationRange.max);
		}

		private void OnDestroy()
		{
			_material.SetFloat(BarRotation, 1);
			_playerBrain.StunStarted -= StunStarted;
			MatchManager.MatchStarted -= Reset;
		}

		private void StunStarted(float percentage)
		{
			_material.SetFloat(BarRotation, Mathf.Lerp(rotationRange.min, rotationRange.max, percentage));
		}
	}
}