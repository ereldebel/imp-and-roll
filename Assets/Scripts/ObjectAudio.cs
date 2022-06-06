using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class ObjectAudio : MonoBehaviour
{
	#region Serialized fields

	[SerializeField] protected AudioClipAndVolume[] clips;

	#endregion

	#region Protected fields

	protected AudioSource Audio;
	protected static readonly System.Random Random = new System.Random();
	protected static HashSet<AudioSource> AudioSources;

	#endregion

	#region Private Fields

	private bool _inObjectList;

	#endregion

	#region Function events

	protected virtual void Awake()
	{
		Audio = GetComponent<AudioSource>();

		if (AudioSources == null) return;
		AudioSources.Add(Audio);
		_inObjectList = true;
	}

	protected void OnDestroy()
	{
		if (_inObjectList)
			AudioSources.Remove(Audio);
	}

	#endregion

	#region Protected Methods

	protected void PlaySingleClipByIndex(int clipIndex)
	{
		Audio.Stop();
		PlayClipByIndex(clipIndex);
	}

	protected void PlayClipByIndex(int clipIndex)
	{
		if (clipIndex >= clips.Length || !clips[clipIndex].clip) return;
		var clip = clips[clipIndex];
		Audio.PlayOneShot(clip.clip, clip.volume);
	}

	protected void SwitchClip(AudioClipAndVolume newClip)
	{
		Audio.Stop();
		Audio.clip = newClip.clip;
		Audio.volume = newClip.volume;
		Audio.Play();
	}

	protected static void SetObjectSet(HashSet<AudioSource> newSet) => AudioSources = newSet;

	#endregion

	[Serializable]
	protected class Header
	{
	}
	
	[Serializable]
	protected struct AudioClipAndVolume
	{
		public AudioClip clip;
		[Range(0, 1)] public float volume;
	}
}