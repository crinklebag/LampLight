using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {

	private AudioSource _audioSource;
<<<<<<< HEAD
	public static float[] _samples = new float[512];
	public static float[] _freqBands = new float[8];
	public static float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];

	private float[] _freqBandHighest = new float[8];
=======
	public static float[] _samples = new float[512];//20,000 hz into 512 samples
	public static float[] _freqBands = new float[8];//512 samples to 8 freq bands
	public static float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];
	[SerializeField] float _buffDecrease = 1.3f;

	private static float[] _freqBandHighest = new float[8];
>>>>>>> feature/Audio
	public static float[] _audioBand = new float[8];
	public static float[] _audioBandBuffer = new float[8];

	public static float _amplitude;
	public static float _amplitudeBuffer;
<<<<<<< HEAD
	private float _amplitudeHighest;
=======
	private static float _amplitudeHighest;
>>>>>>> feature/Audio

	void Start () 
	{
		_audioSource = this.gameObject.GetComponent<AudioSource>();
	}

	void Update () 
	{
		GetSpectrumAudioSource();
		MakeFreqBands();
		BandBuffer();
		CreateAudioBands();
		GetAmplitude();
	}

<<<<<<< HEAD
	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);
	}

	void MakeFreqBands ()
	{
		/*
		freq of song in hz / # of samples = hz per sample
		44100hz / 512 = 86.13hz per sample
=======
	//GetSpectrumData() converts samples from the audio being streamed into frequency data & amplitude
	//_samples holds the samples provided, channel is set to 0, FFT window is an algorithm to calculate the spectrum data & prevents leakages
	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
	}

	//Get spectrum samples and convert into 8 freq bands
	void MakeFreqBands ()
	{
		/*
		Freq of audio in hz / # of samples = hz per sample
>>>>>>> feature/Audio

		//Freq Range
		20 - 60hz
		60 - 250hz
		250 - 500hz
		500 - 2000hz
		2000 - 4000hz
		4000 - 6000hz
		6000 - 20000hz

<<<<<<< HEAD
		//Bands - 44100hz (86hz/sample)
		0 - 1 = 86hz
		1 - 2 = 172hz - 87hz - 258hz
		2 - 4 = 344hz - 259hz - 602hz
 		3 - 8 = 688hz - 603hz - 1290hz
		4 - 16 = 1,376 - 1291hz - 2666hz
		5 - 32 = 2,752 - 2667hz - 5418hz
		6 - 64 = 5,504 - 5419hz - 10922hz
		7 - 128 = 11,008 - 10923hz - 21930hz
			255

		//Bands - 22050hz (43hz/sample)
		0 - 2 = 86hz
		1 - 4 = 172hz - 87hz - 258hz
		2 - 8 = 344hz - 259hz - 602hz
 		3 - 16 = 688hz - 603hz - 1290hz
		4 - 32 = 1,376 - 1291hz - 2666hz
		5 - 64 = 2,752 - 2667hz - 5418hz
		6 - 128 = 5,504 - 5419hz - 10922hz
		7 - 256 = 11,008 - 10923hz - 21930hz
			510
=======
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
>>>>>>> feature/Audio
		*/

		int count = 0;
		for (int i = 0; i < 8; i++) 
		{
			float average = 0.0f;
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

<<<<<<< HEAD
=======
			//add missing samples to the last band
>>>>>>> feature/Audio
			if (i == 7) 
			{
				sampleCount += 2;
			}

<<<<<<< HEAD
=======
			//calculate average amplitude, all samples combined
>>>>>>> feature/Audio
			for (int j = 0; j < sampleCount; j++) 
			{
				average += _samples[count] * (count + 1);
				count++;
			}

<<<<<<< HEAD
			average /= count;

=======
			//set average frequency
			average /= count;
>>>>>>> feature/Audio
			_freqBands[i] = average * 10;
		}
	}

<<<<<<< HEAD
=======
	//if the freq band's value is higher than the band buffer, band buffer becomes the freq band
	//Otherwise, the band buffer should decrease by the buffDecreased
>>>>>>> feature/Audio
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
<<<<<<< HEAD
				_bufferDecrease[i] *= 1.25f;
=======
				_bufferDecrease[i] *= _buffDecrease;
>>>>>>> feature/Audio
			}
		}
	}

<<<<<<< HEAD
=======
	//Get each freq band's highest value
	//create audioband & audiobandbuffer for freq band & bufferband 0 to 1 values
>>>>>>> feature/Audio
	void CreateAudioBands ()
	{
		for (int i = 0; i < 8; i++) 
		{
			if(_freqBands[i] > _freqBandHighest[i])
			{
				_freqBandHighest[i] = _freqBands[i];
			}
<<<<<<< HEAD
=======

>>>>>>> feature/Audio
			_audioBand[i] = (_freqBands[i] / _freqBandHighest[i]);
			_audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
		}
	}

<<<<<<< HEAD
=======
	//Average amplitude for all bands, Average amplitude for all bufferBands
	//Create usable values, 0 to 1 range
>>>>>>> feature/Audio
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
<<<<<<< HEAD
=======

	//Reset each frequency band's highest value, used for changing songs
	//TODO when the next song starts its bands highest freqs will be set to 0, causing a large jump in range
	public static IEnumerator ResetFreqHighest ()
	{
		Debug.Log("Start Reset");
		_amplitudeHighest = 0.0f;

		for(int i = 0; i < 8; i++){
			_freqBandHighest[i] = 0.0f;
		}

		/*for(int i = 0; i < 8; i++){
			Debug.Log("freqBandHighest"+ i + ": " + _freqBandHighest[i]); 
		}*/

		Debug.Log("Band Averages Reset");
		yield return new WaitForEndOfFrame();
	}
>>>>>>> feature/Audio
}