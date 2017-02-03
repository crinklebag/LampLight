using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public List<AudioClip> allAudioClips;//Songs to be included
	public float[] bpm;
	public int songIndex = 0;

	private float timePassed;
	public static bool beatCheck = false;
	float timeBetweenBeats = 0.0f;

	//[SerializeField] GameObject aLoader;
	[SerializeField] AudioSource aSource;//Audio Peer

	bool isPaused = false;
	float clipLength = 0.0f;

	void Start ()
	{
		aSource.Pause();
		StartCoroutine(startAudio(songIndex));
	}

	void Update ()
	{
		//bool isReady = aLoader.GetComponent<AudioLoader> ().audioReady;

		//If the audio loader has finished loading all audio files and the startAudio function hasnt already run, start the audio
		/*if (!hasStarted && isReady) {
			hasStarted = true;
			//allAudioClips = aLoader.GetComponent<AudioLoader> ().allAudioClips;
			startAudio();
		}*/

		//Allow user input for play, pause, next song, previous song
		//getUserInput();


		beatCheck = getBeat();
		//Debug.Log(beatCheck);
	}

	IEnumerator startAudio (int index)
	{
		//Set song index to random song, set audio clip for audio source, set clip length for countdown, play audio
		songIndex = index;//Random.Range (0, allAudioClips.Count);
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;
		timeBetweenBeats = 60000.0f / bpm[songIndex];
		//Debug.Log("time between beats = " + timeBetweenBeats);

		// Needed for making BG scroll to length of song
		GameObject.Find ("BG").GetComponent<BackgroundScroller> ().Reset (clipLength);

		///*
		//try to load the values
		if (AudioTxtReader.loadTxtFile (aSource.clip.name)) {
			aSource.Play ();
		} else {
			Debug.Log("Audio data text file not loaded");
		}
		//*/
		//aSource.Play();

		yield return null;
		//StartCoroutine(AutoNextSong());
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
	/*
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

        // Needed for making BG scroll to length of song
        GameObject.Find("BG").GetComponent<BackgroundScroller>().Reset(clipLength);
        aSource.Play ();
	}
	*/
	/*
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

        // Needed for making BG scroll to length of song
        GameObject.Find("BG").GetComponent<BackgroundScroller>().Reset(clipLength);
        aSource.Play ();
	}
	*/
	/*
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
		NextSong();
		StartCoroutine(NextSong());

		StartCoroutine(AutoNextSong());
	}
	*/
	/*
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
		//Forwards Seek
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			StartCoroutine(NextSong());
		}
		//Backwards Seek
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			StartCoroutine(PreviousSong());
		}

	}*/

	private bool getBeat ()
	{
		//Keep track of time in ms since last beat
		timePassed += Time.deltaTime * 1000;
		//Debug.Log("time passed = " + timePassed);

		//Reset the time passed conuter and return true, there is a beat
		if (timePassed >= timeBetweenBeats) {
			timePassed = 0.0f;
			return true;
		}

		return false;
	}

	public float getCurretClipLength ()
	{
		return aSource.clip.length;
	}
}
