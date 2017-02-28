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
	public float[] bpm;//bpm of the songs
	public int songIndex = 0;//index of song to play

	public static bool beatCheck = false;//bool to check for beat
	public static bool beatCheckHalf = false;//"" "" for half beat
	public static bool beatCheckQuarter = false;//"" "" for quarter beat
	public static bool beatCheckEighth = false;//"" "" for eighth beat
	private int beatCounter = 0;//Counter for beats
	private int prevBeatCount = 0;
	private int prevHalfBeatCount = 0;
	private int prevQuarterBeatCount = 0;
	private int prevEighthBeatCount = 0;
	private float timeBetweenBeats = 0.0f;

	private AudioSource aSource;
	private bool audioFade = false;//True while audio is fading in or out, to ensure coroutine isnt called twice
	private bool isPaused = false;
	private float clipLength = 0.0f;
	[SerializeField] private float audioFadeTime = 5.0f;
	[SerializeField] private float sampleFadeTime = 5.0f;


	/*AudioTxtReader*/
	public TextAsset[] audioData;//Text file containing audio band data
	private static List<float> _allAudioSamples  = new List<float>();//extracted data from text asset
	public static float[] _currAudioSamples = new float[8];//current data to read from
	private float[] _prevAudioSamples = new float[8];//if data parse fails use the last known sample, also used to prevent rapid flickering at audio start
	private float newVal = 0.0f;//hold the new sample value

	[SerializeField] private float _startDelay = 3.0f;//delay to start playing audio and reading data
	[SerializeField] private float readIntervalTick = 0.02f;//Interval to read audio data, must be the same as interval written
	private int sampleCounter = 0;//keep track of sample to pass to current samples

	void Awake ()
	{
		aSource = this.GetComponent<AudioSource>();
		audioFade = false;
	}

	void Update ()
	{
		beatCheck = GetBeat ();
		beatCheckHalf = GetHalfBeat ();
		beatCheckQuarter = GetQuarterBeat ();
		beatCheckEighth = GetEighthBeat();

		//Stop reading data once the song is over, the audio won't be playing and pause bool will still be false
		if (!aSource.isPlaying && !isPaused) 
		{
			stopReadingData();
		}

		if(Input.GetKeyDown(KeyCode.Space))
			StartCoroutine(PlayPauseAudio());
	}

	public void startAudioCoroutine(int index)
	{
		StartCoroutine(StartAudio(index));
	}

	public IEnumerator StartAudio (int index)
	{
		//Set song index to selected index, set audio clip for audio source, set clip length for countdown, set the beat counter back to 0 and the time between beats for BPM detection
		songIndex = index;
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
		//Reset line counter, clear arrays, set starting max value to prevent rapid flickering on start
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
					//if parse fails use previous known sample value
					if (!float.TryParse (lineVals[j], out newVal)) 
					{
						newVal = _prevAudioSamples[j];
					} 

					//Keep track of last value and add newVal to sample array for reading
					_prevAudioSamples[j] = newVal;
					_allAudioSamples.Add(newVal);
				}
			}
		}
		yield return null;
	}

	//Reads audio data at a set interval, continuously invoked but only reads when we are unPaused
	void ReadAudioData ()
	{
		if(!isPaused)
		{
			//Traverse line, add the correct line's data by keeping count of total values
			for (int i = 0; i < 8; i++)
			{
				_currAudioSamples[i] = _allAudioSamples[(sampleCounter + i)];
			}
			//Increase counter for next pass
			sampleCounter += 8;
		}
	}

	//BeatCounter is increased at set interval of the bpm
	bool GetBeat ()
	{
		if (beatCounter > prevBeatCount) {
			prevBeatCount = beatCounter;
			return true;
		}
		return false;
	}
	bool GetHalfBeat ()
	{
		if(beatCounter > prevHalfBeatCount)
		{
			prevHalfBeatCount = beatCounter + 1;
			return true;
		}
		return false;
	}
	bool GetQuarterBeat()
	{
		if(beatCounter > prevQuarterBeatCount)
		{
			prevQuarterBeatCount = beatCounter + 3;
			return true;
		}
		return false;
	}
	bool GetEighthBeat()
	{
		if(beatCounter >  prevEighthBeatCount)
		{
			prevEighthBeatCount = beatCounter + 7;
			return true;
		}
		return false;
	}
	void BeatCount ()
	{
		if(!isPaused)
			beatCounter += 1;
	}

	//Cancel invokes when audio clip isnt playing, and the song hasnt been paused (end of audio)
	void stopReadingData ()
	{
		CancelInvoke("ReadAudioData");
		CancelInvoke("BeatCount");
	}

	//switch paused/unpaused bool for reading data and bpm counter, pause/unpause audio clip
	public IEnumerator PlayPauseAudio ()
	{
		if (!audioFade) 
		{
			audioFade = true;

			if (isPaused) //Play Audio
			{
				//Debug.Log ("Un Pause Audio");
				isPaused = false;
				aSource.UnPause ();
				yield return StartCoroutine(FadeAudio(true));
				audioFade = false;
			}
			else //Pause Audio
			{
				//Debug.Log("Pause Audio");
				yield return StartCoroutine(FadeAudio(false));
				isPaused = true;
				aSource.Pause();
				audioFade = false;
			}
		}
		yield return null;
	}

	IEnumerator FadeAudio (bool fadeUp)
	{
		if (fadeUp) 
		{
			while (aSource.volume < 0.9f)
			{
				aSource.volume = Mathf.Lerp (aSource.volume, 1.0f, Time.deltaTime * audioFadeTime);
				yield return null;
			}
			aSource.volume = 1.0f;
		} 
		else 
		{
			StartCoroutine(LerpSampleValueDown());
			while (aSource.volume > 0.1f) 
			{
				aSource.volume = Mathf.Lerp (aSource.volume, 0.0f, Time.deltaTime * audioFadeTime);
				yield return null;
			}
			aSource.volume = 0.0f;
		}
		yield return null;
	}

	IEnumerator LerpSampleValueDown ()
	{
		//traverse current samples
		for (int i = 0; i < 8; i++) 
		{
			//Lerp curr audio sample down to 0.0f
			while(_currAudioSamples[i] > 0.01f)
			{
				_currAudioSamples[i] = Mathf.Lerp(_currAudioSamples[i], 0.0f, Time.deltaTime * sampleFadeTime);
				yield return null;
			}
		}
		yield return null;
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
}
