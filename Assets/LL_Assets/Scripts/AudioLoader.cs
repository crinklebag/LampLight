using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioLoader : MonoBehaviour {
	//TODO ADD READ EXTERNAL STORAGE PERMISSION FOR ANDROID, not necessary
		//now, but will be necessary in future android releases


	public List<AudioClip> allAudioClips;

	public bool audioReady = false;

	[SerializeField] string filePath = "D:\\Music"; //"/mnt/sdcard/"?

	void Start () 
	{
		StartCoroutine(LoadAllSongs());

	}

	IEnumerator LoadAllSongs()
	{
		//Get each audio file's path, store in list
		allAudioClips = new List<AudioClip>();
		string[] audioFilePaths = Directory.GetFiles(filePath, "*.wav"); 

		for(int i = 0; i < audioFilePaths.Length; i++){
			Debug.Log(audioFilePaths[i]);
			//Load file from each path
			WWW diskAudioDir = new WWW("file://" + audioFilePaths[i]); //"jar:file:// + audioFilePaths[i]"
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
