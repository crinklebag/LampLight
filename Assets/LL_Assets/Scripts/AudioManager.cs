using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private List<AudioClip> allAudioClips;
	public int songIndex = 0;

	[SerializeField] GameObject aLoader;
	[SerializeField] AudioSource aSource;

	bool hasStarted = false;
	bool isPaused = false;
	float clipLength = 0.0f;

	void Start ()
	{
		
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
		//TODO check, once started, enable user input
		//Check for play
		if (Input.GetKeyDown (KeyCode.U)) {
			PlayAudio ();
		}
		//Pause
		if (Input.GetKeyDown (KeyCode.P)) {
			PauseAudio();
		}
		//Forward Seek
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			NextSong();
		}
		//Backwards Seek
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			PreviousSong();
		}
	}

	void startAudio ()
	{
		/*//Loop through the audio clips, give the audio source the first clip, set clipLength to the current clip length for counting down, play the audio
		for (int i = 0; i < allAudioClips.Count; i++) {
			aSource.clip = allAudioClips [i];
			clipLength = aSource.clip.length;
			aSource.Play ();

			//while the song has not ended
			//if audio is playing, decrease the time count, wait for audio to finish before starting the next
			while (clipLength > 0.0f) {
				if (!isPaused) {
					clipLength -= Time.deltaTime;
					Debug.Log(clipLength);
				}
				yield return null;
			}
		}*/

		//Set song index to random song, set audio clip, play audio
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

	//TODO wrap around audio clip array
	public void NextSong ()
	{
		Debug.Log("Next Song");
		aSource.Stop();
		songIndex = songIndex+1;
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;
		aSource.Play ();
	}
	public void PreviousSong()
	{
		Debug.Log("Previous Song");
		aSource.Stop();
		songIndex = songIndex-1;
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;
		aSource.Play ();
	}

	IEnumerator AutoNextSong ()
	{
		//Auto Next Song
		//While the song has not ended
		//If audio is playing, decrease the time count, wait for audio to finish before starting the next
		while (clipLength > 0.0f) {
			if (!isPaused) {
				clipLength -= Time.deltaTime;
				Debug.Log(clipLength);
			}
			yield return null;
		}
		NextSong();
		StartCoroutine(AutoNextSong());
	}
}
