using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    [SerializeField] PlayButtonController playButton;

    [SerializeField] float[] topBarFillTargets;
    [SerializeField] float[] bottomBarFillTargets;
    [SerializeField] Color transparentBarTarget;
    [SerializeField] Image topBarTransparent; 
    [SerializeField] Image topBarOpaque;
    [SerializeField] Image bottomBarTransparent;
    [SerializeField] Image bottomBarOpaque;
    [SerializeField] Image[] bottomBarDots;

    float opaqueJourneyLength = Vector3.Distance(Vector3.one, Vector3.zero);  // Vector3.one, Vector3.zero
    float transparentJourneyLength = Vector3.Distance(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f)); // Vectore3.zero, Vector3(0.5f, 0.5f, 0.5f)
    float startTime;
    float speed = 1;

    // Use for lerping the float values
    float topBarFillMin = 0;
    float topBarFillMax = 0;
    float bottomBarFillMin = 0;
    float bottomBarFillMax = 0;
    float topBarT = 0;
    float bottomBarT = 0;

    bool startTopBarUpdate = false;
    bool startBottomBarUpdate = false;
    bool canFadeIn = false;

    // Turn this into a player pref eventually.
    int unlockedLevels = 3;



    public Image[] topBar;
    public Image[] navButtonsBG;
    public Image[] navButtonsSong;
    public Image[] navButtonsPlay;
    private float lerpColorTime = 0;

    public enum MenuState { Intro = 0, BGSelect, SongSelect, PlayGame, Count };
    [SerializeField] MenuState currentState;
    [SerializeField] MenuState lastState;
    [SerializeField] RectTransform menuHolder;

    [SerializeField] GameObject[] bgOptions;

    Vector3 newPos = Vector3.zero;
    Vector3 menuVelocity = Vector3.zero;

    bool moveUI = false;
    bool startGame = false;
    bool updateTopBar = false;
    bool updateBottomBar = false;

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
        if (moveUI) { MoveUI(); }
        // UpdateTopBar();

        if (updateTopBar) {
            switch (currentState) {
                case MenuState.Intro:
                    FadeOutTopBar();
                    ResetTopBar();
                    break;
                case MenuState.BGSelect:
                    if (canFadeIn) FadeInTopBar();
                    if (startTopBarUpdate) {
                        UpdateTopBar();
                    }
                    // Update when fade is finished
                    break;
                case MenuState.SongSelect:
                    UpdateTopBar();
                    break;
                case MenuState.PlayGame:
                    UpdateTopBar();
                    break;
            }
        }

        if (updateBottomBar) {
            switch (currentState) {
                case MenuState.Intro:
                    FadeOutBottomBar();
                    ResetBottomBar();
                    break;
                case MenuState.BGSelect:
                    FadeInBottomBar();
                    if(startBottomBarUpdate)UpdateBottomBar();
                    break;
                case MenuState.SongSelect:
                    FadeOutBottomBar();
                    ResetBottomBar();
                    break;
            }
        }
    }


    void RegisterUpdate() {
        switch (currentState)
        {
            // If menu state is intro - Fade both the top and bottom(only if on) bar to clear - reset all fill values
            case MenuState.Intro:
                updateTopBar = true;
                topBarFillMax = 0;
                if (bottomBarOpaque.color.a < 0.1f) { updateBottomBar = true; }
                break;

            // When going to BG Select: Update Nav Bar - Use the enum to pull from the position array
            case MenuState.BGSelect:
                updateTopBar = true;
                updateBottomBar = true;
                topBarFillMax = topBarFillTargets[1];
                break;

            // When at Song Select - Update the nav bar - fade out bottom bar
            case MenuState.SongSelect:
                // Fade in X Button
                updateTopBar = true;
                topBarFillMax = topBarFillTargets[2];
                break;
            // Fill the rest of the top bar - then start the game
            case MenuState.PlayGame:
                updateTopBar = true;
                topBarFillMax = topBarFillTargets[3];
                break;
        }

        // Set the start time here - this function is only called once
        startTime = Time.time;
        moveUI = true;
    }

    public void UpdateMenuState() {
        // Debug.Log("Hit Space");
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
        
        moveUI = true;
    }

    void MoveUI() {
        Debug.Log("Moving UI to " + newPos);
        menuHolder.localPosition = Vector3.SmoothDamp(menuHolder.localPosition, newPos, ref menuVelocity, smoothTime);
    }

    void FadeInTopBar() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        topBarOpaque.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        topBarTransparent.color = Color.Lerp(Color.clear, transparentBarTarget, fracJourney);

        // When the lerp is completed
        if (fracJourney >= 1) {
            canFadeIn = false;
            Debug.Log("Setting Can Fade to true");
            if (currentState == MenuState.BGSelect && !startTopBarUpdate) { startTopBarUpdate = true; }
        }
    }

    void FadeInBottomBar() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        bottomBarOpaque.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        bottomBarTransparent.color = Color.Lerp(Color.clear, transparentBarTarget, fracJourney);

        // When the lerp is completed
        if (fracJourney >= 1) {
            if (currentState == MenuState.BGSelect && !startBottomBarUpdate) { startBottomBarUpdate = true; }
        }
    }

    void FadeOutTopBar() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        topBarOpaque.color = Color.Lerp(Color.white, Color.clear, fracJourney);
        topBarTransparent.color = Color.Lerp( transparentBarTarget, Color.clear, fracJourney);
        
    }

    void FadeOutBottomBar() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        bottomBarOpaque.color = Color.Lerp(Color.white, Color.clear, fracJourney);
        bottomBarTransparent.color = Color.Lerp(transparentBarTarget, Color.clear, fracJourney);
        
    }

    void UpdateBottomBar() {
        // Figure out the bottom bar based off how many levels are unlocked - just do the first three for now
        switch (unlockedLevels) {
            case 1:
                bottomBarFillMax = bottomBarFillTargets[0];
                bottomBarDots[0].color = Color.white;
                break;
            case 2:
                bottomBarFillMax = bottomBarFillTargets[1];
                bottomBarDots[1].color = Color.white;
                break;
            case 3:
                bottomBarFillMax = bottomBarFillTargets[2];
                bottomBarDots[0].color = Color.white;
                bottomBarDots[1].color = Color.white;
                bottomBarDots[2].color = Color.white;
                break;
            case 4:
                bottomBarFillMax = bottomBarFillTargets[3];
                bottomBarDots[3].color = Color.white;
                break;
        }

        bottomBarOpaque.fillAmount = Mathf.Lerp(bottomBarFillMin, bottomBarFillMax, bottomBarT);
        bottomBarT += speed * Time.deltaTime;

        if (bottomBarOpaque.fillAmount == bottomBarFillMax) {
            startBottomBarUpdate = false;
        }
    }

    void UpdateTopBar() {
        topBarOpaque.fillAmount = Mathf.Lerp(topBarFillMin, topBarFillMax, topBarT);
        topBarT += speed * Time.deltaTime;

        if (topBarOpaque.fillAmount == topBarFillMax) {
            startTopBarUpdate = false;
        }
        
    }

    void ResetTopBar() {
        // Reset the fill value
        topBarOpaque.fillAmount = 0;
    }

    void ResetBottomBar() {
        bottomBarOpaque.fillAmount = 0;
    }

    void FadeInDot(int dotIndex) {

    }

    void FadeOutDots(int dotIndex) {

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
                break;
            case 1:
                canFadeIn = true;
                currentState = MenuState.BGSelect;
                ResetBackgroundSelection();
                break;
            case 2:
                currentState = MenuState.SongSelect;
                ChooseSong();
                break;
            case 3:
                currentState = MenuState.PlayGame;
                // Load Game
                break;
            default:
                break;
        }

        Debug.Log("Clicked Button to state: " + currentState);
        UpdateMenuState();
        RegisterUpdate();
    }

    public void ChooseSong() {
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

        // Debug.Log(sceneToSave);

        PlayerPrefs.SetString("sceneNumber", sceneToSave);
        PlayerPrefs.Save();
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
        //

        if (PlayerPrefs.GetInt("bgNumber") != -1 && PlayerPrefs.GetString("sceneNumber") != "")
        {
            SceneManager.LoadScene("Loading");
        } else {

            startGame = false;
        }
    }

    public int GetCurrentState() {
        return (int)currentState;
    }
    
}
