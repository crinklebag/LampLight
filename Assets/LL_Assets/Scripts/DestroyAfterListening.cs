using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterListening : MonoBehaviour {

	[SerializeField]
	private float clipLength;

	[SerializeField]
	private float clipTime;

	private AudioSource myAudio;

	[SerializeField]
	private int timer = 0;

	// Use this for initialization
	void Awake () {
		myAudio = GetComponent<AudioSource>();
		clipLength = myAudio.clip.length;

		StartCoroutine("IfClipTimeFailed");
	}
	
	// Update is called once per frame
	void Update () {
		
		clipTime = myAudio.time;

		if (clipTime >= clipLength - 0.1f)
		{
			Destroy(this.gameObject);
		}
	}

	IEnumerator IfClipTimeFailed()
	{
		yield return new WaitForSecondsRealtime(1);
		timer++;

		if (timer >= clipLength)
		{
			Destroy(this.gameObject);
		}
		else
		{
			StartCoroutine("IfClipTimeFailed");
		}
	}
}
