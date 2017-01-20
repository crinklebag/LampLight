using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioLoader : MonoBehaviour {

	public List<AudioClip> allAudioClips;
	[SerializeField] string filePathWindows = @"D:\Music"; 
	[SerializeField] string filePathAndroid = @"/storage/emulated/0/Android/media";
	public bool audioReady = false;

	void Start () 
	{
		/*if(Application.platform == RuntimePlatform.WindowsEditor){
			StartCoroutine(LoadAllSongsWindows());
		}*/
		if(Application.platform == RuntimePlatform.Android){
			StartCoroutine(LoadAllSongsAndroid());
		}
	}

	IEnumerator LoadAllSongsWindows()//Will only work while playing from the editor
	{
		//Get each audio file's path, store in list
		allAudioClips = new List<AudioClip>();
		string[] audioFilePaths = Directory.GetFiles(filePathWindows, "*.wav"); 

		for(int i = 0; i < audioFilePaths.Length; i++){
			Debug.Log(audioFilePaths[i]);
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

	IEnumerator LoadAllSongsAndroid ()
	{
		//Get each audio file's path, store in list
		allAudioClips = new List<AudioClip>();
		string[] audioFilePaths = Directory.GetFiles(filePathAndroid, "*.mp3"); 

		for(int i = 0; i < audioFilePaths.Length; i++){
			Debug.Log(audioFilePaths[i]);
			//Load file from each path
			WWW diskAudioDir = new WWW("file://" + audioFilePaths[i]); //"jar:file:// + audioFilePaths[i]"
			//Wait for file to load
			while(!diskAudioDir.isDone){
				yield return null;
			}
			//Get the audio clip, name it, add it to the list of audio clips
			AudioClip clip = diskAudioDir.GetAudioClip(false, false, AudioType.MPEG);//(IGNORED BY UNITY 5.X(2D/3D SOUND), NOT STREAMING, FILE FORMAT)
			clip.name = Path.GetFileName(audioFilePaths[i]);
			allAudioClips.Add(clip);
		}
		//Tell the audio manager that all clips have been loaded
		audioReady = true;
	}
}
