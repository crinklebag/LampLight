using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSFX : MonoBehaviour {

	[SerializeField] private AudioClip tapClip;
	[SerializeField] private AudioClip dodoClip;
	[SerializeField] private AudioClip jarDropClip;
	[SerializeField] private AudioClip greenTwinkle;

	private AudioSource aSource;

	void Start ()
	{
		aSource = this.GetComponent<AudioSource>();
	}

	public void playTap ()
	{
		aSource.clip = tapClip;
		aSource.Play();
	}

	public void playDodo ()
	{
		aSource.clip = dodoClip;
		aSource.Play();
	}

	public void playGreenDodo()
	{
		aSource.clip = greenTwinkle;
		aSource.Play();
	}

	public void playJarDrop ()
	{
		aSource.clip = jarDropClip;
		aSource.PlayScheduled(AudioSettings.dspTime + 0.75f);
	}
}
