using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private List<AudioClip> allAudioClips;
	public int songIndex = 0;

	[SerializeField] GameObject aLoader;
	[SerializeField] AudioSource aSource;//Audio Peer

	bool hasStarted = false;
	bool isPaused = false;
	float clipLength = 0.0f;

	void Start ()
	{
		aSource.Pause();
	}

	void Update ()
	{
		bool isReady = aLoader.GetComponent<AudioLoader> ().audioReady;

		//If the audio loader has finished loading all audio files and the startAudio function hasnt already run, start the audio
		if (!hasStarted && isReady) {
			hasStarted = true;
			allAudioClips = aLoader.GetComponent<AudioLoader> ().allAudioClips;
			startAudio ();
		}
		//Allow user input for play, pause, next song, previous song
		if(hasStarted)
			getUserInput();
	}

	void startAudio ()
	{
		//Set song index to random song, set audio clip for audio source, set clip length for countdown, play audio
		songIndex = Random.Range(0, allAudioClips.Count);
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;
		aSource.Play ();
		StartCoroutine(AutoNextSong());
	}

	//Un-Pause
	public void PlayAudio ()
	{
		if (!aSource.isPlaying) {
			Debug.Log ("Un Pause");
			isPaused = false;
			aSource.UnPause ();
		}
	}

	public void PauseAudio ()
	{
		if (aSource.isPlaying) {
			Debug.Log ("Pause");
			isPaused = true;
			aSource.Pause ();
		}
	}

	public IEnumerator NextSong ()
	{
		Debug.Log("Next Song");
		aSource.Stop();
		songIndex = songIndex+1;

		//Prevent index from going out of bounds
		if(songIndex > (allAudioClips.Count -1))
			songIndex = 0;

		//Reset band highest average in the audio peer class for the next song
		//aSource.GetComponent<AudioPeer>().ResetBandAverage();
		yield return StartCoroutine(AudioPeer.ResetFreqHighest());

		Debug.Log("Song Index: " + songIndex);
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;

		aSource.Play ();
	}
	public IEnumerator PreviousSong()
	{
		Debug.Log("Previous Song");
		aSource.Stop();
		songIndex = songIndex-1;

		//Prevent index from going out of bounds
		if(songIndex < 0)
			songIndex = allAudioClips.Count -1;

		//Reset band highest average in the audio peer class for the next song
		//aSource.GetComponent<AudioPeer>().ResetBandAverage();
		yield return StartCoroutine(AudioPeer.ResetFreqHighest());

		Debug.Log("Song Index: " + songIndex);
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;

		aSource.Play ();
	}

	IEnumerator AutoNextSong ()
	{
		//While the song has not ended & if audio is playing, decrease clipLength, when clipLength <= 0.0 the next song is started
		while (clipLength > 0.0f) {
			if (!isPaused) {
				clipLength -= Time.deltaTime;
				//Debug.Log("Clip Length Remaining: " + clipLength);
			}
			yield return null;
		}
		StartCoroutine(NextSong());
		StartCoroutine(AutoNextSong());
	}

	void getUserInput ()
	{
		//Play
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			PlayAudio ();
		}
		//Pause
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			PauseAudio();
		}
		//Forward Seek
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			StartCoroutine(NextSong());
		}
		//Backwards Seek
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			StartCoroutine(PreviousSong());
		}
	}

	public float getCurretClipLength ()
	{
		return aSource.clip.length;
	}
}
