using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

	[SerializeField] AudioManager am;

	[SerializeField] AudioSource subAudioSource;

	[SerializeField] GameObject player;

	[SerializeField] GameObject pausePanel;

	private bool isPaused = false;

	[SerializeField] float pauseFadeTime = 5.0f;


	public void ChangePauseState()
	{
		//Change whether this is paused or unpaused when this gets called
		if(isPaused)
		{
			isPaused = false;
		}
		else
		{
			isPaused = true;
		}

		playPauseAudio();

		player.SetActive(!isPaused); //If were paused, isPaused returns true, jar set active bool should be the opposite (off when the game is paused, on when the game is unpaused)
		pausePanel.SetActive(isPaused);//Pause panel is set active when we are paused, and not active when were unpaused

		am.StartCoroutine("PlayPauseAudio");//fades audio up/down also pauses audio and lerps current audio samples down to 0
	}

	private void playPauseAudio()
	{
		if(isPaused)
		{
			StartCoroutine(am.FadeSubAudio(true, pauseFadeTime));
		}
		else
		{
			StartCoroutine(am.FadeSubAudio(false, pauseFadeTime));
		}
	}

	public void RestartLevel()
	{
		SceneManager.LoadScene("Loading");
	}


	public void LoadMenu()
	{
		SceneManager.LoadScene("MainMenu_Mobile");
	}
}
