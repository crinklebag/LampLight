using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {

	private AudioSource _audioSource;
	public static float[] _samples = new float[512];
	public static float[] _freqBands = new float[8];
	public static float[] _bandBuffer = new float[8];
	private float[] _bufferDecrease = new float[8];

	private float[] _freqBandHighest = new float[8];
	public static float[] _audioBand = new float[8];
	public static float[] _audioBandBuffer = new float[8];

	public static float _amplitude;
	public static float _amplitudeBuffer;
	private float _amplitudeHighest;

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

	void GetSpectrumAudioSource()
	{
		_audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);
	}

	void MakeFreqBands ()
	{
		/*
		freq of song in hz / # of samples = hz per sample
		44100hz / 512 = 86.13hz per sample

		//Freq Range
		20 - 60hz
		60 - 250hz
		250 - 500hz
		500 - 2000hz
		2000 - 4000hz
		4000 - 6000hz
		6000 - 20000hz

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
		*/

		int count = 0;
		for (int i = 0; i < 8; i++) 
		{
			float average = 0.0f;
			int sampleCount = (int)Mathf.Pow (2, i) * 2;

			if (i == 7) 
			{
				sampleCount += 2;
			}

			for (int j = 0; j < sampleCount; j++) 
			{
				average += _samples[count] * (count + 1);
				count++;
			}

			average /= count;

			_freqBands[i] = average * 10;
		}
	}

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
				_bufferDecrease[i] *= 1.25f;
			}
		}
	}

	void CreateAudioBands ()
	{
		for (int i = 0; i < 8; i++) 
		{
			if(_freqBands[i] > _freqBandHighest[i])
			{
				_freqBandHighest[i] = _freqBands[i];
			}
			_audioBand[i] = (_freqBands[i] / _freqBandHighest[i]);
			_audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
		}
	}

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
}