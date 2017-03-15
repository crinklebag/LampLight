using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerMenu : MonoBehaviour {

	private AudioSource aSource;

	// Use this for initialization
	void Start () 
	{
		aSource = this.GetComponent<AudioSource>();
		StartCoroutine(startAudio());
	}
	

	IEnumerator startAudio ()
	{
		aSource.loop = true;
		aSource.clip = Resources.Load<AudioClip>("Audio/Intro");
		aSource.volume = 0.5f;
		aSource.Play();
		yield return null;
	}
}
