using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {

	//private string path = @"D:\Text\test.txt";

	private AudioSource _audioSource;
	public static float[] _samples = new float[512];	//20,000 hz into 512 samples

	public static float[] _freqBands = new float[8];	//512 samples to 8 freq bands
	public static float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];
	[SerializeField] float _buffDecrease = 1.3f;

	private static float[] _freqBandHighest = new float[8];
	public static float[] _audioBand = new float[8];
	public static float[] _audioBandBuffer = new float[8];

	public static float _amplitude;
	public static float _amplitudeBuffer;
	private static float _amplitudeHighest;

	//public static float[] _beatSamples = new float[128];//20,000 hz into 128 samples [0]: 0-172hz :D
														//60-120hz bass kick
														//120-150hz snare
	//[SerializeField] float beatThreshValue = 0.3f;
	//[SerializeField] float beatPostIgnore = 250.0f;		//Time in milliseconds to ignore beat
	//private static float beatLastTime = 0.0f;					//Last time the beat occured

	//private static bool beatDetected = false;

	private static float CurrentTimeInMs = 0.0f;		//Hold the audio clip's current time in milliseconds

	/*
	private static Queue<float> beatTimes;
	private static float lastQueuedTime;						//List of how many milliseconds ago the last beat was
	private  static float beatQueueCutoff = 10000;		//Number of milliseconds of previous beats to keep in the queue
	public static float beatAverage = 0;

	static float msPerBeat;
	public static float EstimateBPM;
	*/

	void Start () 
	{
		_audioSource = this.gameObject.GetComponent<AudioSource>();
		//beatTimes = new Queue<float>();
	}

	void Update () 
	{
		GetSpectrumAudioSource();
		MakeFreqBands();
		BandBuffer();
		CreateAudioBands();
		GetAmplitude();
		//AudioClipTick();
		//DetectBeat();
		//PredictBPM();
	}

	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, FFTWindow.Rectangular);
		//_audioSource.GetSpectrumData(_beatSamples, 0, FFTWindow.BlackmanHarris);
	}

	void MakeFreqBands ()
	{
		/*
		//freq of song in hz / # of samples = hz per sample
		//44100hz / 512 = 86.13hz per sample

		//GetSpectrumData() converts samples from the audio being streamed into frequency data & amplitude
		//_samples holds the samples provided, channel is set to 0, FFT window is an algorithm to calculate the spectrum data & prevents leakages

		//Get spectrum samples and convert into 8 freq bands
		Freq of audio in hz / # of samples = hz per sample
		//Freq Range
		20 - 60hz
		60 - 250hz
		250 - 500hz
		500 - 2000hz
		2000 - 4000hz
		4000 - 6000hz
		6000 - 20000hz

		//Bands - 22050hz (43hz/sample)
		//band# - samples = hz [range]
		0 - 2 = 86hz
		1 - 4 = 172hz    [ 87hz - 258hz ]
		2 - 8 = 344hz    [ 259hz - 602hz ]
 		3 - 16 = 688hz   [ 603hz - 1290hz ]
		4 - 32 = 1,376   [ 1291hz - 2666hz ]
		5 - 64 = 2,752   [ 2667hz - 5418hz ]
		6 - 128 = 5,504  [ 5419hz - 10922hz ]
		7 - 256 = 11,008 [ 10923hz - 21930hz ]
	Total - 510 (missing 2 samples)
		*/

		int count = 0;
		for (int i = 0; i < 8; i++) 
		{
			float average = 0.0f;
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

			//add missing samples to the last band
			if (i == 7) 
			{
				sampleCount += 2;
			}

			//calculate average amplitude, all samples combined
			for (int j = 0; j < sampleCount; j++) 
			{
				average += _samples[count] * (count + 1);
				count++;
			}

			//set average frequency
			average /= count;
			_freqBands[i] = average * 10;
		}
	}

	//if the freq band's value is higher than the band buffer, band buffer becomes the freq band
	//Otherwise, the band buffer should decrease by the buffDecreased
	void BandBuffer ()
	{
		for (int i = 0; i < 8; ++i) 
		{
			if(_freqBands[i] > _bandBuffer[i])
			{
				_bandBuffer[i] = _freqBands[i];
				_bufferDecrease[i] = 0.005f;
			}
			if(_freqBands[i] < _bandBuffer[i])
			{
				_bandBuffer[i] -= _bufferDecrease[i];
				_bufferDecrease[i] *= _buffDecrease;

			}
		}
	}

	//Get each freq band's highest value
	//create audioband & audiobandbuffer for freq band & bufferband 0 to 1 values
	void CreateAudioBands ()
	{
		//path = @"D:\Text\" + _audioSource.clip.name + ".txt";
		for (int i = 0; i < 8; i++) {
			if (_freqBands [i] > _freqBandHighest [i]) {
				_freqBandHighest [i] = _freqBands [i];
			}

			_audioBand [i] = (_freqBands [i] / _freqBandHighest [i]);
			_audioBandBuffer [i] = (_bandBuffer [i] / _freqBandHighest [i]);

			/*
			//Write to txt file
			if (i == 7) {
				System.IO.File.AppendAllText (path, _audioBandBuffer [i].ToString ());
				System.IO.File.AppendAllText (path, "\r\n");
			} else {
				System.IO.File.AppendAllText (path, _audioBandBuffer [i].ToString () + ",");
			}
			*/
		}
	}

	//Average amplitude for all bands, Average amplitude for all bufferBands
	//Create usable values, 0 to 1 range
	void GetAmplitude ()
	{
		float _currentAmplitude = 0;
		float _currentAmplitudeBuffer = 0;

		for (int i = 0; i < 8; i++) 
		{
			_currentAmplitude += _audioBand[i];
			_currentAmplitudeBuffer += _audioBandBuffer[i];

		}
		if(_currentAmplitude > _amplitudeHighest)
		{
			_amplitudeHighest = _currentAmplitude;
		}
		_amplitude = _currentAmplitude / _amplitudeHighest;
		_amplitudeBuffer = _currentAmplitudeBuffer / _amplitudeHighest;
	}

	/*
	//Reset each frequency band's highest value, used for changing songs
	//Reset current audio clip time in milliseconds as well
	//Clear beatTimes Queue..
	//TODO when the next song starts its bands highest freqs will be set to 0, causing a large jump in range
	public static IEnumerator ResetFreqHighest ()
	{
		Debug.Log("Start Reset");
		_amplitudeHighest = 0.0f;

		for(int i = 0; i < 8; i++){
			_freqBandHighest[i] = 0.0f;
		}

		for(int i = 0; i < 8; i++){
			Debug.Log("freqBandHighest"+ i + ": " + _freqBandHighest[i]); 
		}

		CurrentTimeInMs = 0.0f;
		beatLastTime = 0.0f;
		beatTimes.Clear();

		Debug.Log("Band Averages Reset");
		yield return null;
	}


	void DetectBeat ()
	{
		beatDetected = false;
		beatAverage = 0;

		//Get average frequency of first 4 samples
		for(int i = 0; i < 4; i++)
		{
			beatAverage += _samples[i];
		}

		beatAverage /= 4;

		//Beat occurs when average of first 4 samples passes threshold
		if (beatAverage >= beatThreshValue && beatLastTime == 0) {
			beatLastTime = GetAudioClipTime ();
			beatDetected = true;

			//Store timne of detected beat
			beatTimes.Enqueue (beatLastTime);
			lastQueuedTime = beatLastTime;

			//Remove oldest beat time if it is older than the cutoff
			while (GetAudioClipTime () - beatTimes.Peek() > beatQueueCutoff) {
				beatTimes.Dequeue ();
				if (beatTimes.Count == 0) {
					break;
				}
			}

		}

		//Ignore given time in audio clip to avoid false beat
		if (GetAudioClipTime () - beatLastTime >= beatPostIgnore) {
			beatLastTime = 0.0f;
		}

	}

	//Take oldest and newest beat times in the queue and divide them by the size of the queue minus one
		//Gets the average number of milliseconds between each beat
	//Divide msPerBeat into 60000 milliseconds to get BPM estimate
	void PredictBPM ()
	{
		if(beatTimes.Count >= 2)
		{
			msPerBeat = ((lastQueuedTime - beatTimes.Peek()) / (beatTimes.Count - 1));
			EstimateBPM = 60000/msPerBeat;
		} else {
			EstimateBPM = 0.0f;
		}

	}

	//Increase to songs counter
	void AudioClipTick ()
	{
		if (_audioSource.isPlaying) {
			CurrentTimeInMs += Time.deltaTime * 1000;
		}
	}


	//Return the songs counter, for beatDetection
	float GetAudioClipTime ()
	{
		return CurrentTimeInMs;	
	}

	//Call from other scripts to read beat
	public static bool GetBeat ()
	{
		return beatDetected;
	}
	*/
}