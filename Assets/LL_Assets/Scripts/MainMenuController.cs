using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour, IDragHandler {

    public Image[] brighterBars;
    private Color32[] brighterBarsColors;
    private float lerpColorTime = 0;

    public enum MenuState { Intro, SongSelect, BGSelect };
    [SerializeField] MenuState currentState;
    [SerializeField] MenuState lastState;
    [SerializeField] RectTransform menuHolder;

    Vector3 newPos = Vector3.zero;
    Vector3 menuVelocity = Vector3.zero;

    bool moveUI = false;
    bool startGame = false;
    float destXPos;
    float smoothTime = 0.5f;
    float start = 0;
    float end = 0;
    int dir = 0;

	// Use this for initialization
	void Start () {
        // Debug.Log("Hi");
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

        /*int fingerCount = 0;
        foreach (Touch touch in Input.touches) {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }

        ProcessInputs();*/

        for (int i = 0; i < brighterBars.Length; i++)
        {
            brighterBars[i].color = Color32.Lerp(brighterBars[i].color, brighterBarsColors[i], lerpColorTime += Time.deltaTime);
        }

        if (startGame)
        {
            //Debug.Log("StartGame");
            //Debug.Log(brighterBars[2].color.a);

            if (brighterBars[2].color.a >= 1.0f)
            {
                LoadScene();
                //Debug.Log("LoadScene");
                startGame = false;
            }
        }
    }

    void UpdateMenuState(bool moveRight) {
        // Debug.Log("Hit Space");

        lerpColorTime = 0;

        switch (currentState)
        {
            case MenuState.Intro:
                newPos = new Vector3(0, 0, 0);
                //lastState = MenuState.Intro;
                //currentState = MenuState.SongSelect;
                brighterBarsColors[0] = Color.clear;
                brighterBarsColors[1] = Color.clear;
                brighterBarsColors[2] = Color.clear;
                break;
            case MenuState.SongSelect:
                newPos = new Vector3(-(Screen.width * 2.38f), 0, 0);
                //lastState = MenuState.Intro;
                //currentState = MenuState.BGSelect;
                brighterBarsColors[0] = Color.white;
                brighterBarsColors[1] = Color.white;
                brighterBarsColors[2] = Color.clear;
                break;
            case MenuState.BGSelect:
                newPos = new Vector3(-(Screen.width * 1.19f), 0, 0);
                //lastState = MenuState.BGSelect;
                //currentState = MenuState.SongSelect;
                brighterBarsColors[0] = Color.white;
                brighterBarsColors[1] = Color.clear;
                brighterBarsColors[2] = Color.clear;
                break;
        }

        moveUI = true;
    }

    void MoveUI() {
        menuHolder.localPosition = Vector3.SmoothDamp(menuHolder.localPosition, newPos, ref menuVelocity, smoothTime);
    }

    void RotateCamera() {
        bool moveRight = false;

        switch (Input.GetTouch(0).phase) {
            case TouchPhase.Began:
                // Debug.Log("Touch started at" + Input.GetTouch(0).deltaPosition.x);
                start = Input.GetTouch(0).deltaPosition.x;
                break;
            case TouchPhase.Moved:
                // Get the change in direction input
                float xDir = Input.GetTouch(0).deltaPosition.x;
                //Debug.Log("deltaPosition: " + Input.GetTouch(0).deltaPosition)
                // Disregard any unintentional, tiny discrepencies in a swipe
                if (Mathf.Abs(xDir) < 50) {
                    start = 0;
                    end = 0;
                }
                break;
            case TouchPhase.Ended:
                // Debug.Log("Touch Ended at: " + Input.GetTouch(0).deltaPosition.x);
                end = Input.GetTouch(0).deltaPosition.x;
                break;
            }

        if (start > end) {
            Debug.Log("Move Left");
            moveRight = false;
        } else if (start < end) {
            Debug.Log("Move Right");
            moveRight = true;
        }

        UpdateMenuState(moveRight);

        start = 0;
        end = 0;
    }

    void ProcessInputs() {
        if (Input.touchCount == 1) {
            RotateCamera();
        }
    }

    public void OnDrag(PointerEventData baseData) {

        // Convert base data to pointer data to get position changes
        PointerEventData pointerData = baseData;

        RotateCamera(/*pointerData.delta.x*/);
    }

    public void ButtonClick(int val)
    {
        lastState = currentState;

        switch (val)
        {
            case 0:
                {
                    currentState = MenuState.Intro;
                    UpdateMenuState(true);
                }
                break;
            case 1:
                {
                    currentState = MenuState.BGSelect;
                    UpdateMenuState(true);
                }
                break;
            case 2:
                {
                    int bgToSave = -1;

                    if (EventSystem.current.currentSelectedGameObject.name == "Music")
                    {
                        currentState = MenuState.SongSelect;
                    }

                    switch (EventSystem.current.currentSelectedGameObject.name)
                    {
                        case "Forest":
                            {
                                if (EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color.a == 0)
                                {
                                    bgToSave = 2;
                                    currentState = MenuState.SongSelect;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    //Debug.Log(bgToSave);

                    PlayerPrefs.SetInt("bgNumber", bgToSave);
                    PlayerPrefs.Save();

                    UpdateMenuState(true);
                }
                break;
            case 3:
                {
                    lerpColorTime = 0;
                    brighterBarsColors[2] = Color.white;

                    int sceneToSave = -1;

                    switch (EventSystem.current.currentSelectedGameObject.name)
                    {
                        case "Seasons Change":
                            {
                                sceneToSave = 0;
                            }
                            break;
                        case "Get Free":
                            {
                                sceneToSave = 1;
                            }
                            break;
                        case "Dream Giver":
                            {
                                sceneToSave = 2;
                            }
                            break;
                        case "Spirit Speaker":
                            {
                                sceneToSave = 3;
                            }
                            break;
                        default:
                            break;
                    }

                    //Debug.Log(sceneToSave);

                    PlayerPrefs.SetInt("sceneNumber", sceneToSave);
                    PlayerPrefs.Save();

                    if (startGame == false)
                    {
                        startGame = true;
                    }
                }
                break;
            default:
                break;
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
            //Debug.Log("Loading scene");
            SceneManager.LoadScene("Main_Mobile");
        } else
        {
            Debug.Log("You missed a selection!!");

            Debug.Log("bgNumber " + PlayerPrefs.GetInt("bgNumber"));
            Debug.Log("sceneNumber " + PlayerPrefs.GetInt("sceneNumber"));

            startGame = false;
        }
    }
}
