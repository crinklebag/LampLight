using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSFX : MonoBehaviour {

	[SerializeField] private AudioClip tapClip;

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
}
