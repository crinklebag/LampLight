using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioLoader : MonoBehaviour {

	public List<AudioClip> allAudioClips;

	public bool audioReady = false;

	[SerializeField] string filePath = "D:\\Music";

	void Start () 
	{
		StartCoroutine(LoadAllSongs());

	}

	IEnumerator LoadAllSongs()
	{
		//Get each audio file's path, store in list
		allAudioClips = new List<AudioClip>();
		string[] audioFilePaths = Directory.GetFiles(filePath, "*.wav");//TODO SERIALIZE PATH & FILE .EXT

		for(int i = 0; i < audioFilePaths.Length; i++){
			//Debug.Log(audioFilePaths[i]);
			//Load file from each path
			WWW diskAudioDir = new WWW("file://" + audioFilePaths[i]);
			//Wait for file to load
			while(!diskAudioDir.isDone){
				yield return null;
			}
			//Get the audio clip, name it, add it to the list of audio clips
			AudioClip clip = diskAudioDir.GetAudioClip(false, false, AudioType.WAV);//(IGNORED BY UNITY 5.X(2D/3D SOUND), NOT STREAMING, FILE FORMAT)
			clip.name = Path.GetFileName(audioFilePaths[i]);
			allAudioClips.Add(clip);
		}
		//Tell the audio manager that all clips have been loaded
		audioReady = true;
	}
}
