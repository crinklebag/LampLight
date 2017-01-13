using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioLoader : MonoBehaviour {

	public List<AudioClip> allAudioClips;

	public bool audioReady = false;

	void Start () 
	{
		StartCoroutine(LoadAllSongs());
	}

	IEnumerator LoadAllSongs()
	{
		allAudioClips = new List<AudioClip>();
		string[] audioFilePaths = Directory.GetFiles("D:\\Music", "*.wav");

		for(int i = 0; i < audioFilePaths.Length; i++){
			//Debug.Log(audioFilePaths[i]);
			//Get each audio file's path
			WWW diskAudioDir = new WWW("file://" + audioFilePaths[i]);
			//Wait for file to load
			while(!diskAudioDir.isDone){
				yield return null;
			}
			//Get the audio clip, name it, add it to the list of audio clips
			AudioClip clip = diskAudioDir.GetAudioClip(false, false, AudioType.WAV);
			clip.name = Path.GetFileName(audioFilePaths[i]);
			allAudioClips.Add(clip);
		}
		//Play audio once all audio clips have been loaded
		audioReady = true;
	}
}
