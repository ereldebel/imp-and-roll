﻿using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class ObjectAudio : MonoBehaviour
{
	#region Serialized fields

	[SerializeField]
	protected AudioClip[] clips;

	#endregion

	#region Private fields

	protected AudioSource Audio;
	protected static readonly System.Random Random = new System.Random();

	#endregion

	#region Function events

	protected void Awake()
	{
		Audio = GetComponent<AudioSource>();
	}

	#endregion

	#region Protected Methods

	protected void PlaySingleClipByIndex(int clipIndex)
	{
		Audio.Stop();
		if (clipIndex < clips.Length && clips[clipIndex])
			Audio.PlayOneShot(clips[clipIndex]);
	}

	protected void PlayClipByIndex(int clipIndex)
	{
		if (clipIndex < clips.Length && clips[clipIndex])
			Audio.PlayOneShot(clips[clipIndex]);
	}

	#endregion
	
	[Serializable]
	public class Header{}
}