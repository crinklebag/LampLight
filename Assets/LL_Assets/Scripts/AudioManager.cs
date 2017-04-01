using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

[RequireComponent(typeof (AudioSource))]
public class AudioManager : MonoBehaviour {

	[SerializeField] private AudioSource subASource;

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

	[SerializeField] private float endGameFadeSpeed = 5.0f;
	[SerializeField] private float pauseGameFadeSpeed = 5.0f;

	[SerializeField] private float _startDelay = 3.0f;//delay to start playing audio and reading data
	[SerializeField] private float readIntervalTick = 0.02f;//Interval to read audio data, must be the same as interval written
	private int sampleCounter = 0;//keep track of sample to pass to current samples

	[SerializeField] AudioClip endGameAudio;

	void Awake ()
	{
		aSource = this.GetComponent<AudioSource>();
		audioFade = false;
		setAudioClip();
	}

	void Update ()
	{
		beatCheck = GetBeat ();
		beatCheckHalf = GetHalfBeat ();
		beatCheckQuarter = GetQuarterBeat ();
		beatCheckEighth = GetEighthBeat();
	}

	public void setAudioClip()
	{
		//Set song index to selected index, set audio clip for audio source,
		aSource.clip = Resources.Load<AudioClip> ("Audio/OGG_1Min/" + PlayerPrefs.GetString ("sceneNumber"));
		clipLength = aSource.clip.length;
	}

	public void startAudioCoroutine(int index)
	{
		return;
	}

	public IEnumerator StartAudio ()
	{
		 //set clip length for countdown, set the beat counter back to 0 and the time between beats for BPM detection

		beatCounter = 0;

		//songIndex now only used to determine which bpm the audioManager need to use
		switch (PlayerPrefs.GetString ("sceneNumber")) 
		{
			case "Seasons Change":
				songIndex = 0;
				break;
			case "Get Free":
				songIndex = 1;
				break;
			case "Dream Giver":
				songIndex = 2;
				break;
			case "Spirit Speaker":
				songIndex = 3;
				break;
			default:
				break;
		}

		timeBetweenBeats = 60.0f / bpm[songIndex];

		aSource.PlayScheduled(AudioSettings.dspTime + _startDelay);

		InvokeRepeating("BeatCount", _startDelay, timeBetweenBeats);

		yield return null;
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
		{
			beatCounter += 1;
		}
	}

	//Cancel invokes when audio clip isnt playing, and the song hasnt been paused (end of audio)
	void stopReadingData ()
	{
		//CancelInvoke("ReadAudioData");
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
				yield return StartCoroutine(FadeAudio(true, pauseGameFadeSpeed));
				audioFade = false;
			}
			else //Pause Audio
			{
				//Debug.Log("Pause Audio");
				yield return StartCoroutine(FadeAudio(false, pauseGameFadeSpeed));
				isPaused = true;
				aSource.Pause();
				audioFade = false;
			}
		}
		yield return null;
	}

	//Fade the audio up or down
	//On fade down, current audio sample values are lerped down to stop firefly glowing
	public IEnumerator FadeAudio (bool fadeUp, float fadeTime)
	{
		if (fadeUp) 
		{
			while (aSource.volume < 0.95f)
			{
				aSource.volume = Mathf.MoveTowards (aSource.volume, 1.0f, Time.deltaTime * fadeTime);
				yield return null;
			}
			aSource.volume = 1.0f;
		} 
		else 
		{
			StartCoroutine(LerpSampleValueDown(fadeTime));
			while (aSource.volume > 0.05f) 
			{
				aSource.volume = Mathf.MoveTowards (aSource.volume, 0.0f, Time.deltaTime * fadeTime);
				yield return null;
			}
			aSource.volume = 0.0f;
		}

		yield return null;
	}

	//Lerp current audio sample down
	IEnumerator LerpSampleValueDown (float sampleFadeTime)
	{
		//traverse current samples
		for (int i = 0; i < 8; i++) 
		{
			//Lerp curr audio sample down to 0.0f
			while(AudioPeer._audioBandBuffer[i] > 0.01f)
			{
				AudioPeer._audioBandBuffer[i] = Mathf.MoveTowards(AudioPeer._audioBandBuffer[i], 0.0f, Time.deltaTime * sampleFadeTime);
				yield return null;
			}
		}
		yield return null;
	}

	//Perform end game audio shiz
	//fade audio down
	//change audio to short loop
	//fade audio back in
	public IEnumerator EndGame()
	{
		yield return StartCoroutine(FadeAudio(false, endGameFadeSpeed));

		aSource.clip = endGameAudio;

		//yield return new WaitForSeconds(0.5f);

		aSource.PlayScheduled(AudioSettings.dspTime);

		yield return StartCoroutine(FadeAudio(true, endGameFadeSpeed));

		yield return null;
	}


	//Fade the audio up or down
	//On fade down, current audio sample values are lerped down to stop firefly glowing
	public IEnumerator FadeSubAudio (bool fadeUp, float fadeTime)
	{
		if (fadeUp) 
		{
			subASource.clip = endGameAudio;
			subASource.PlayScheduled(AudioSettings.dspTime);

			while (subASource.volume < 0.95f)
			{
				subASource.volume = Mathf.MoveTowards (subASource.volume, 1.0f, Time.deltaTime * fadeTime);
				yield return null;
			}
			subASource.volume = 1.0f;
		} 
		else 
		{
			while (subASource.volume > 0.05f) 
			{
				subASource.volume = Mathf.MoveTowards (subASource.volume, 0.0f, Time.deltaTime * fadeTime);
				yield return null;
			}
			subASource.volume = 0.0f;
			subASource.Stop();
		}

		yield return null;
	}
}
