using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Image[] topBar;
    // public Image[] brighterBars;
    public Image[] navButtonsBG;
    public Image[] navButtonsSong;
    public Image[] navButtonsPlay;
    [SerializeField]
    // private Color32[] brighterBarsColors;
    private float lerpColorTime = 0;

    public enum MenuState { Intro, SongSelect, BGSelect, PlayGame };
    [SerializeField] MenuState currentState;
    [SerializeField] MenuState lastState;
    [SerializeField] RectTransform menuHolder;

    [SerializeField] GameObject[] bgOptions;

    Vector3 newPos = Vector3.zero;
    Vector3 menuVelocity = Vector3.zero;

    bool moveUI = false;
    bool startGame = false;
    float destXPos;
    float smoothTime = 0.5f;
    float start = 0;
    float end = 0;
    int dir = 0;

    [SerializeField] AudioSFX menuSFX;

	// Use this for initialization
	void Start () {
        currentState = MenuState.Intro;
        lastState = MenuState.Intro;

        PlayerPrefs.SetInt("bgNumber", -1);
        PlayerPrefs.SetInt("sceneNumber", -1);
        PlayerPrefs.Save();
    }

    void Update() {
        MoveUI();
        UpdateTopBar();

        lerpColorTime += (Time.deltaTime * 0.2f);

        
    }


    void UpdateTopBar() {
        switch (currentState)
        {
            case MenuState.Intro:
                // Evrything fades to clear - set as grey
                // Fade in X Button
                for (int i = 0; i < topBar.Length; i++) {
                    // topBar[i].color = Color32.Lerp(topBar[i].color, Color.grey, lerpColorTime);
                    topBar[i].color = Color32.Lerp(topBar[i].color, Color.clear, lerpColorTime);
                }
                // Fade in BG Select to white and others to grey
                for (int j = 0; j < navButtonsBG.Length; j++) {
                    // navButtonsBG[j].color = Color32.Lerp(navButtonsBG[j].color, Color.grey, lerpColorTime);
                    navButtonsBG[j].color = Color32.Lerp(navButtonsBG[j].color, Color.clear, lerpColorTime);
                    // navButtonsSong[j].color = Color32.Lerp(navButtonsSong[j].color, Color.grey, lerpColorTime);
                    navButtonsSong[j].color = Color32.Lerp(navButtonsSong[j].color, Color.clear, lerpColorTime);
                }
                for (int k = 0; k < navButtonsPlay.Length; k++) {
                    // navButtonsPlay[k].color = Color32.Lerp(navButtonsPlay[k].color, Color.grey, lerpColorTime);
                    navButtonsPlay[k].color = Color32.Lerp(navButtonsPlay[k].color, Color.clear, lerpColorTime);
                }
                break;
            case MenuState.SongSelect:
                // Fade in Song Select
                for (int j = 0; j < navButtonsBG.Length; j++) {
                    navButtonsSong[j].color = Color32.Lerp(navButtonsSong[j].color, Color.white, lerpColorTime);
                }
                break;
            case MenuState.BGSelect:
                // Fade in X Button
                for (int i = 0; i < topBar.Length; i++) {
                    topBar[i].color = Color32.Lerp(topBar[i].color, Color.white, lerpColorTime);
                }
                // Fade in BG Select to white and others to grey
                for (int j = 0; j < navButtonsBG.Length; j++) {
                    navButtonsBG[j].color = Color32.Lerp(navButtonsBG[j].color, Color.white, lerpColorTime);
                    navButtonsSong[j].color = Color32.Lerp(navButtonsSong[j].color, Color.grey, lerpColorTime);
                }
                for (int k = 0; k < navButtonsPlay.Length; k++) {
                    navButtonsPlay[k].color = Color32.Lerp(navButtonsPlay[k].color, Color.grey, lerpColorTime);
                }
                break;
            case MenuState.PlayGame:
                // Fade in Play Game to white
                for (int k = 0; k < navButtonsPlay.Length; k++) {
                    navButtonsPlay[k].color = Color32.Lerp(navButtonsPlay[k].color, Color.white, lerpColorTime);
                }
                break;
        }
    }

    public void UpdateMenuState(bool moveRight) {
        // Debug.Log("Hit Space");

        lerpColorTime = 0;

        switch (currentState)
        {
            case MenuState.Intro:
                newPos = new Vector3(0, 0, 0);
                break;
            case MenuState.SongSelect:
                Debug.Log("Move Menu");
                newPos = new Vector3(-3840, 0, 0);
                break;
            case MenuState.BGSelect:
                newPos = new Vector3(-1920, 0, 0);
                break;
        }

        UpdateTopBar();
        moveUI = true;
    }

    void MoveUI() {
        menuHolder.localPosition = Vector3.SmoothDamp(menuHolder.localPosition, newPos, ref menuVelocity, smoothTime);
    }

    public void ButtonClick(int val)
    {
        lastState = currentState;

        menuSFX.playTap();

        switch (val)
        {
            case 0:
                currentState = MenuState.Intro;
                ResetBackgroundSelection();
                UpdateMenuState(true);
                break;
            case 1:
                currentState = MenuState.BGSelect;
                ResetBackgroundSelection();
                UpdateMenuState(true);
                break;
            case 2:
                    currentState = MenuState.SongSelect;
                    break;
            case 3:
                currentState = MenuState.PlayGame;
                lerpColorTime = 0;
                // brighterBarsColors[2] = Color.white;

                string sceneToSave = "";

                switch (EventSystem.current.currentSelectedGameObject.name) {
                    case "Seasons Change":
                        sceneToSave = "Seasons Change";
                        break;
                    case "Get Free":
                        sceneToSave = "Get Free";
                        break;
                    case "Dream Giver":
                        sceneToSave = "Dream Giver";
                        break;
                    case "Spirit Speaker":
                        sceneToSave = "Spirit Speaker";
                        break;
                    default:
                        break;
                }

                Debug.Log(sceneToSave);

                PlayerPrefs.SetString("sceneNumber", sceneToSave);
                PlayerPrefs.Save();
                break;
            default:
                break;
        }
    }

    public void ResetBackgroundSelection() {
        // Loop through the BG Options and reset the one that has been chosen 
        foreach (GameObject bg in bgOptions) {
            if (bg.GetComponent<StretchUIMask>().IsSelected()) {
                // Debug.Log("Looking through bg");
                bg.GetComponent<StretchUIMask>().DeselectLocation();
            }
        }
    }

    public void LoadScene()
    {
        //yield return new WaitUntil(() => brighterBars[2].color.a >= (brighterBarsColors[2].a - 1));

        // need to make scenes with the background and music in it already
        // for now just loads and finds the music/bg and sets it
        // since it is using ints, maybe have the scene named like Main_Mobile_01 for a backyard bg and Get Free music

        //string sceneName = "Main_Mobile_" + PlayerPrefs.GetInt("bgNumber").ToString() + PlayerPrefs.GetInt("sceneNumber").ToString();

        //SceneManager.LoadScene(sceneName);

        if (PlayerPrefs.GetInt("bgNumber") != -1 && PlayerPrefs.GetString("sceneNumber") != "")
        {
            // Debug.Log("Loading scene");
            // SceneManager.LoadScene("Main_Mobile");

            // Just load instructions here 

            //Load selected scene
            switch (PlayerPrefs.GetInt("bgNumber")) {
                case 1:
                    SceneManager.LoadScene("Main_Mobile");
                    //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                    break;
                case 2:
                    SceneManager.LoadScene("Main_Mobile");
                    //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                    break;
                case 3:
                    SceneManager.LoadScene("Main_Mobile_DeepForest");
                    //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                    break;
                case 4:
                    SceneManager.LoadScene("Main_Mobile_DeepForest");
                    //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                    break;
            }
        } else
        {
            Debug.Log("You missed a selection!!");

            Debug.Log("bgNumber " + PlayerPrefs.GetInt("bgNumber"));
            Debug.Log("sceneNumber " + PlayerPrefs.GetInt("sceneNumber"));

            startGame = false;
        }
    }

    public int GetCurrentState() {
        return (int)currentState;
    }
    
}
