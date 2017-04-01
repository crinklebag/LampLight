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
	[SerializeField] Image pausePanelImage;

	public bool isPaused = false;

	public bool isPanelActive = false;

	[SerializeField] float panelFadeSpeed = 5.0f;

	[SerializeField] float pauseFadeTime = 5.0f;

	[SerializeField] Sprite[] PausePanelBgs;
	[SerializeField] GameObject[] TextObjs;



	public void ChangePauseState()
	{
		//Change whether this is paused or unpaused when this gets called
		if(isPaused)
		{
			isPaused = false;
			StartCoroutine(turnPanelOff());
		}
		else
		{
			isPaused = true;
			setPausePanel();
		}

		playPauseAudio();

		player.SetActive(!isPaused); //If were paused, isPaused returns true, jar set active bool should be the opposite (off when the game is paused, on when the game is unpaused)
		//pausePanel.SetActive(isPaused);//Pause panel is set active when we are paused, and not active when were unpaused

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
		switch (PlayerPrefs.GetInt("bgNumber"))
        {
            case 1:
				SceneManager.LoadScene("Main_Mobile_DeepForest");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                break;
            case 2:
                SceneManager.LoadScene("Main_Mobile_DeepForest");
                // SceneManager.LoadScene("Main_Mobile");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                break;
            case 3:
                SceneManager.LoadScene("Main_Mobile_Waterfall");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                break;
            case 4:
                SceneManager.LoadScene("Main_Mobile_Beach");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                break;
        }
	}

	void setPausePanel()
	{
		pausePanel.SetActive(true);//Pause panel is set active when we are paused, and not active when were unpaused

		if(!isPanelActive)
		{
			isPanelActive = true;
			StartCoroutine(fadePanelIn());
		}

		switch (PlayerPrefs.GetInt("bgNumber"))
        {
            case 1:
				pausePanelImage.sprite = PausePanelBgs[0];
                break;
            case 2:
				pausePanelImage.sprite = PausePanelBgs[1];
                break;
            case 3:
				pausePanelImage.sprite = PausePanelBgs[2];
                break;
            case 4:
				pausePanelImage.sprite = PausePanelBgs[3];
                break;
        }
	}

	IEnumerator fadePanelIn()
	{
		pausePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		pausePanelImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		float tempA = pausePanelImage.color.a;

		while(tempA < 0.999f)
		{
			tempA = Mathf.MoveTowards(tempA, 1.0f, Time.deltaTime * panelFadeSpeed);
			pausePanelImage.color =  new Color(1.0f, 1.0f, 1.0f, tempA);
			pausePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, tempA);

			yield return null;
		}

		for(int i = 0; i < TextObjs.Length; i++)
		{
			TextObjs[i].SetActive(true);
		}

		isPanelActive = false;
		yield return null;
	}

	IEnumerator turnPanelOff()
	{
		for(int i = 0; i < TextObjs.Length; i++)
		{
			TextObjs[i].SetActive(false);
		}

		pausePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		pausePanelImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		float tempA = pausePanelImage.color.a;

		while(tempA > 0.001f)
		{
			tempA = Mathf.MoveTowards(tempA, 0.0f, Time.deltaTime * panelFadeSpeed);
			pausePanelImage.color =  new Color(1.0f, 1.0f, 1.0f, tempA);
			pausePanel.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, tempA);

			yield return null;
		}




		pausePanel.SetActive(false);
		yield return null;
	}

	public void LoadMenu()
	{
		SceneManager.LoadScene("MainMenu_Mobile");
	}
}
