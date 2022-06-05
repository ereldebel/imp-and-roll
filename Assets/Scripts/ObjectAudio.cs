using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class ObjectAudio : MonoBehaviour
{
	#region Serialized fields

	[SerializeField]
	protected AudioClip[] clips;

	#endregion

	#region Protected fields

	protected AudioSource Audio;
	protected static readonly System.Random Random = new System.Random();

	#endregion

	#region Function events

	protected virtual void Awake()
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
	
	protected void SwitchClip(AudioClip newClip)
	{
		Audio.Stop();
		Audio.clip = newClip;
		Audio.Play();
	}

	#endregion
	
	[Serializable]
	protected class Header{}
}