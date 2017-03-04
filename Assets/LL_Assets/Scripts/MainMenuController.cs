using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Image[] topBar;
    public Image[] brighterBars;
    private Color32[] brighterBarsColors;
    private float lerpColorTime = 0;

    public enum MenuState { Intro, SongSelect, BGSelect };
    [SerializeField] MenuState currentState;
    [SerializeField] MenuState lastState;
    [SerializeField] RectTransform menuHolder;

    [SerializeField] GameObject[] bgOptions;

    Vector3 newPos = Vector3.zero;
    Vector3 menuVelocity = Vector3.zero;

    bool moveUI = false;
    bool showTopBar = false;
    bool startGame = false;
    float destXPos;
    float smoothTime = 0.5f;
    float start = 0;
    float end = 0;
    int dir = 0;

	// Use this for initialization
	void Start () {
        currentState = MenuState.Intro;
        lastState = MenuState.Intro;

        brighterBarsColors = new Color32[brighterBars.Length];

        for (int i = 0; i < brighterBarsColors.Length; i++)
        {
            brighterBarsColors[i] = brighterBars[i].color;
        }

        PlayerPrefs.SetInt("bgNumber", -1);
        PlayerPrefs.SetInt("sceneNumber", -1);
        PlayerPrefs.Save();

        //Debug.Log(PlayerPrefs.GetInt("bgNumber"));
        //Debug.Log(PlayerPrefs.GetInt("sceneNumber"));
    }

    void Update() {
        MoveUI();

        lerpColorTime += Time.deltaTime;

        for (int i = 0; i < brighterBars.Length; i++)
        {
            brighterBars[i].color = Color32.Lerp(brighterBars[i].color, brighterBarsColors[i], lerpColorTime);
        }

        if (startGame)
        {
            if (brighterBars[2].color.a >= 0.9f)
            {
                LoadScene();
                startGame = false;
            }
        }

        if (showTopBar)
        {
            foreach (Image thing in topBar)
            {
                thing.color = Color32.Lerp(thing.color, Color.white, lerpColorTime);
            }
        } 
        else
        {
            foreach (Image thing in topBar)
            {
                thing.color = Color32.Lerp(thing.color, Color.clear, lerpColorTime);
            }
        }
    }

    public void UpdateMenuState(bool moveRight) {
        // Debug.Log("Hit Space");

        lerpColorTime = 0;

        switch (currentState)
        {
            case MenuState.Intro:
                newPos = new Vector3(0, 0, 0);
                brighterBarsColors[0] = Color.clear;
                brighterBarsColors[1] = Color.clear;
                brighterBarsColors[2] = Color.clear;
                showTopBar = false;
                break;
            case MenuState.SongSelect:
                Debug.Log("Move Menu");
                newPos = new Vector3(-3840, 0, 0);
                brighterBarsColors[0] = Color.white;
                brighterBarsColors[1] = Color.white;
                brighterBarsColors[2] = Color.clear;
                showTopBar = true;
                break;
            case MenuState.BGSelect:
                newPos = new Vector3(-1920, 0, 0);
                brighterBarsColors[0] = Color.white;
                brighterBarsColors[1] = Color.clear;
                brighterBarsColors[2] = Color.clear;
                showTopBar = true;
                break;
        }

        moveUI = true;
    }

    void MoveUI() {
        menuHolder.localPosition = Vector3.SmoothDamp(menuHolder.localPosition, newPos, ref menuVelocity, smoothTime);
    }

    public void ButtonClick(int val)
    {
        lastState = currentState;

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
                lerpColorTime = 0;
                brighterBarsColors[2] = Color.white;

                int sceneToSave = -1;

                switch (EventSystem.current.currentSelectedGameObject.name) {
                    case "Seasons Change":
                        sceneToSave = 0;
                        break;
                    case "Get Free":
                        sceneToSave = 1;
                        break;
                    case "Dream Giver":
                        sceneToSave = 2;
                        break;
                    case "Spirit Speaker":
                        sceneToSave = 3;
                        break;
                    default:
                        break;
                }

                Debug.Log(sceneToSave);

                PlayerPrefs.SetInt("sceneNumber", sceneToSave);
                PlayerPrefs.Save();

                if (startGame == false)  {
                    startGame = true;
                }
                break;
            default:
                break;
        }
    }

    public void ResetBackgroundSelection() {
        // Loop through the BG Options and reset the one that has been chosen 
        foreach (GameObject bg in bgOptions) {
            if (bg.GetComponent<StretchUIMask>().IsSelected()) {
                // Debug.Log("Lookinf through bg");
                bg.GetComponent<StretchUIMask>().DeselectLocation();
            }
        }
    }

    void LoadScene()
    {
        //yield return new WaitUntil(() => brighterBars[2].color.a >= (brighterBarsColors[2].a - 1));

        // need to make scenes with the background and music in it already
        // for now just loads and finds the music/bg and sets it
        // since it is using ints, maybe have the scene named like Main_Mobile_01 for a backyard bg and Get Free music

        //string sceneName = "Main_Mobile_" + PlayerPrefs.GetInt("bgNumber").ToString() + PlayerPrefs.GetInt("sceneNumber").ToString();

        //SceneManager.LoadScene(sceneName);

        if (PlayerPrefs.GetInt("bgNumber") != -1 && PlayerPrefs.GetInt("sceneNumber") != -1)
        {
            // Debug.Log("Loading scene");
            // SceneManager.LoadScene("Main_Mobile");

            //Load selected scene
            switch (PlayerPrefs.GetInt("bgNumber")) {
                case 1:
                    SceneManager.LoadScene("Main_Mobile");
                    break;
                case 2:
                    SceneManager.LoadScene("Main_Mobile");
                    break;
                case 3:
                    SceneManager.LoadScene("Main_Mobile_DeepForest");
                    break;
                case 4:
                    SceneManager.LoadScene("Main_Mobile_DeepForest");
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
}
