using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class AudioTxtReader : MonoBehaviour {

	[SerializeField] AudioSource aPeer;

	//the file's path
	private static string path = @"D:\Text\test.txt";

	//keep track of sample to pass to current samples
	private static int sampleCounter = 0;

	//Songs entire set of _audioBand values, Array to store the values at the current frame
	public static List<float> _allAudioSamples  = new List<float>();
	public static float[] _currAudioSamples = new float[8];

	private static float currentTime = 0.0f;
	private static float writeIntervalCounter = 0.0f;		//Time interval in ms at which audio data is written (start 1 second in?)
	private static float writeIntervalInc = 0.2f;


	void FixedUpdate ()
	{
		if (aPeer.isPlaying) {
			currentTime = (float)AudioSettings.dspTime;
			Debug.Log(currentTime);
			passAudioData ();

			/*for (int i = 0; i < _currAudioSamples.Length; i++) {
				Debug.Log(i + " " + _currAudioSamples[i]);
			}*/

		}
	}

	void passAudioData ()
	{	
		if (currentTime >= writeIntervalCounter) 
		{
			//Debug.Log("interval: " + writeIntervalCounter);

			for (int i = 0; i < 8; i++) 
			{
				_currAudioSamples[i] = _allAudioSamples[(sampleCounter + i)];
			}

			sampleCounter += 8;
			writeIntervalCounter += writeIntervalInc;
		}

		//Pass the audio sample into the current audio sample, to be read from

	}

	//Public method to call when changing songs
	//Reads Audio data from txt file, splits txt file into lines (each line is the 8 bars current value per frame)
	//Stores each lines value into a list
	public static bool loadTxtFile (string fName)
	{

		//Reset line counter, clear arrays
		sampleCounter = 0;
		_allAudioSamples.Clear();
		System.Array.Clear(_currAudioSamples, 0, _currAudioSamples.Length);
		//Debug.Log("sample count: " +_allAudioSamples.Count);

		try 
		{
			TextAsset audioData = Resources.Load(fName) as TextAsset;

			string line;
			path  = Application.dataPath + "/LL_Assets/Resources/"  + fName + ".txt";
			StreamReader reader = new StreamReader(path);

			using(reader)
			{
				do //While theres lines remaining do this
				{
					line = reader.ReadLine();//get the line

					if(line != null)
					{
						//split line into values based off comma delimeters
						string[] lineVals = line.Split(new char[]{','});

						//Traverse line, convert strings into floats, add to all Audio Samples list
						for(int i = 0; i < lineVals.Length; i++)
						{
							float newVal = float.Parse(lineVals[i]);
							_allAudioSamples.Add(newVal);
						}
					}
				}
				while(line != null);
				//Done reading, close reader, return true
				reader.Close();
				Debug.Log("txt file loaded");
				return true;
			}
		} 
		catch (UnityException excep)
		{
			Debug.LogError(excep.Message);
			return false;
		}
	}

	public static void setDspCounter ()
	{
		writeIntervalCounter = (float)AudioSettings.dspTime;
	}
}
