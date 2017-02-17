using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

[RequireComponent(typeof (AudioSource))]
public class AudioManager : MonoBehaviour {

	/*Audio Manager*/
	public List<AudioClip> allAudioClips;//Songs to be included
	public float[] bpm;
	public int songIndex = 0;

	public static bool beatCheck = false;//bool to check for beat
	private int beatCounter = 0;
	private int prevBeatCount = 0;
	private float timeBetweenBeats = 0.0f;

	private AudioSource aSource;

	private bool isPaused = false;
	private float clipLength = 0.0f;

	/*AudioTxtReader*/
	public TextAsset[] audioData;//Text file containing audio band data
	private static List<float> _allAudioSamples  = new List<float>();//extracted data from text asset
	public static float[] _currAudioSamples = new float[8];//current data to read from
	private float[] _prevAudioSamples = new float[8]; 
	private float newVal = 0.0f;

	[SerializeField] private float _startDelay = 3.0f;
	[SerializeField] private float readIntervalTick = 0.02f;//Interval to read audio data, must be the same as interval written
	private int sampleCounter = 0;//keep track of sample to pass to current samples

	void Awake ()
	{
		aSource = this.GetComponent<AudioSource>();
		//StartCoroutine(StartAudio(songIndex));
	}

	void Update ()
	{
		beatCheck = GetBeat();
	}

	public IEnumerator StartAudio (int index)
	{
		//Set song index to selected index, set audio clip for audio source, set clip length for countdown, set the beat counter back to 0 and the time between beats for BPM detection
		songIndex = index;//Random.Range (0, allAudioClips.Count);
		aSource.clip = allAudioClips [songIndex];
		clipLength = aSource.clip.length;
		beatCounter = 0;
		timeBetweenBeats = 60.0f / bpm[songIndex];

		// Needed for making BG scroll to length of song
		//GameObject.Find ("BG").GetComponent<BackgroundScroller> ().Reset (clipLength);

		//Wait until txt file is loaded, play audio, invoke readAudioData and beatCount
		yield return StartCoroutine(LoadTxtFile(songIndex));
		aSource.PlayScheduled(AudioSettings.dspTime + _startDelay);
		InvokeRepeating("ReadAudioData", _startDelay, readIntervalTick);
		InvokeRepeating("BeatCount", _startDelay, timeBetweenBeats);


		yield return null;
	}

	//Reads Audio data from txt file, splits txt file into lines (each line is the 8 bars current value per frame)
	//Splits each line into values which are passed to the all audio samples list
	IEnumerator LoadTxtFile (int fileIndex)
	{
		//Reset line counter, clear arrays
		sampleCounter = 0;
		_allAudioSamples.Clear ();
		System.Array.Clear (_currAudioSamples, 0, _currAudioSamples.Length);
		_prevAudioSamples = new float[8] {0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f};

		string[] lines = audioData [fileIndex].text.Split ("\n" [0]);//split by line breaks

		//Traverse lines
		for (int i = 0; i < lines.Length; i++) {
			if (lines [i] != null) {
				//Split lines into their values and store in lineVals
				string[] lineVals = lines [i].Split (',');

				//Traverse lineVals, convert strings into floats, add to all Audio Samples list
				for (int j = 0; j < lineVals.Length; j++) 
				{
					if (!float.TryParse (lineVals[j], out newVal)) 
					{
						newVal = _prevAudioSamples[j];
					} 

					_prevAudioSamples[j] = newVal;
					_allAudioSamples.Add(newVal);
				}
			}
		}
		yield return null;
	}

	//Reads audio data at a set interval
	void ReadAudioData ()
	{
		//Traverse line, add the correct line's data by keeping count of total values
		for (int i = 0; i < 8; i++)
		{
			_currAudioSamples[i] = _allAudioSamples[(sampleCounter + i)];
		}
		//Increase counter for next pass
		sampleCounter += 8;
	}

	//BeatCounter is increased at set interval of the bpm
	public bool GetBeat ()
	{
		if (beatCounter > prevBeatCount) {
			prevBeatCount = beatCounter;
			return true;
		}
		return false;
	}

	void BeatCount ()
	{
		beatCounter += 1;
	}

	/*
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
	}*/
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
	/*
	public float getCurretClipLength ()
	{
		return aSource.clip.length;
	}
	*/
}
