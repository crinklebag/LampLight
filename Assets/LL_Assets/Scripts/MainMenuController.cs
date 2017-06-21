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
    [SerializeField] GameObject buttons;

    float opaqueJourneyLength = Vector3.Distance(Vector3.one, Vector3.zero);  // Vector3.one, Vector3.zero
    float transparentJourneyLength = Vector3.Distance(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f)); // Vectore3.zero, Vector3(0.5f, 0.5f, 0.5f)
    float startTime;
    float speed = 1;

    // Use for lerping the float values
	[SerializeField] float topBarFillMin = 0;
	[SerializeField] float topBarFillMax = 0;
	[SerializeField] float topBarPreviousMax = 0;
	[SerializeField] float bottomBarFillMin = 0;
	[SerializeField] float bottomBarFillMax = 0;

    float topBarT = 0;
    float bottomBarT = 0;
    float smoothTime = 0.5f;

    bool startTopBarUpdate = false;
    bool startBottomBarUpdate = false;
	[SerializeField] bool canFadeIn = false;
    bool moveUI = false;
    bool startGame = false;
    bool updateTopBar = false;
    bool updateBottomBar = false;
	bool canGoBack = false;

    // Turn this into a player pref eventually.
    int unlockedLevels = 3;

    Vector3 newPos = Vector3.zero;
    Vector3 menuVelocity = Vector3.zero;
    [SerializeField] RectTransform menuHolder;
    [SerializeField] GameObject[] bgOptions;
    [SerializeField] GameObject xReplace;

    public enum MenuState { Intro = 0, BGSelect, SongSelect, PlayGame, Achievements, Scores, Count};
    [SerializeField] MenuState currentState;
	[SerializeField] MenuState previousState;
    [SerializeField] AudioSFX menuSFX;

    [SerializeField] Image creditsFade;
    [SerializeField] float creditsFadeSpeed;
    bool isFading = false;

	public RectTransform achievementsBoard; 
	public RectTransform scoreBoard;
	public RectTransform mainMenuBoard;

	// Use this for initialization
	void Start () {
        currentState = MenuState.Intro;
        xReplace.SetActive(false);
		creditsFade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		creditsFade.gameObject.SetActive(false);
        PlayerPrefs.SetInt("bgNumber", -1);
        PlayerPrefs.SetInt("sceneNumber", -1);
        PlayerPrefs.Save();

		achievementsBoard.rect.Set (mainMenuBoard.rect.position.x, -(mainMenuBoard.rect.position.y * 2), mainMenuBoard.rect.width, mainMenuBoard.rect.height);
		scoreBoard.rect.Set (mainMenuBoard.rect.position.x, (mainMenuBoard.rect.position.y * 2), mainMenuBoard.rect.width, mainMenuBoard.rect.height);

		achievementsBoard.anchoredPosition = new Vector2 (0, mainMenuBoard.rect.height);
		scoreBoard.anchoredPosition = new Vector2 (0, -mainMenuBoard.rect.height);
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
                    if (canFadeIn)
					{
						FadeInTopBar ();
					}
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
					FadeOutDots (3);
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
				topBarPreviousMax = topBarFillMax;
                topBarFillMax = 0;
                if (bottomBarOpaque.color.a < 0.1f) { updateBottomBar = true; }
                FadeOutDots(3);
                break;
            // When going to BG Select: Update Nav Bar - Use the enum to pull from the position array
			case MenuState.BGSelect:
				updateTopBar = true;
				updateBottomBar = true;
				topBarPreviousMax = topBarFillMax;
                topBarFillMax = topBarFillTargets[1];
                topBarFillMin = topBarFillTargets[0];
				
				if ((int)previousState > 1)
				{
					startTopBarUpdate = true;
				}

                break;
            // When at Song Select - Update the nav bar - fade out bottom bar
            case MenuState.SongSelect:
                // Fade in X Button
                updateTopBar = true;
				topBarPreviousMax = topBarFillMax;
                topBarFillMax = topBarFillTargets[2];
                topBarFillMin = topBarFillTargets[1];
                FadeOutDots(3);
                break;
            // Fill the rest of the top bar - then start the game
            case MenuState.PlayGame:
                updateTopBar = true;
				topBarPreviousMax = topBarFillMax;
                topBarFillMax = topBarFillTargets[3];
                topBarFillMin = topBarFillTargets[2];
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
                // Debug.Log("Move Menu");
                newPos = new Vector3(-3840, 0, 0);
                break;
            case MenuState.BGSelect:
                newPos = new Vector3(-1920, 0, 0);
                break;
		case MenuState.Achievements:
			newPos = new Vector3 (0, -mainMenuBoard.rect.height, 0);
                break;
            case MenuState.Scores:
			newPos = new Vector3(0, mainMenuBoard.rect.height, 0);
                break;
        }
        
        moveUI = true;
    }

    void MoveUI() {
        // Debug.Log("Moving UI to " + newPos);
        menuHolder.localPosition = Vector3.SmoothDamp(menuHolder.localPosition, newPos, ref menuVelocity, smoothTime);
    }

    void FadeInTopBar() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        topBarOpaque.color = Color.Lerp(Color.clear, Color.white, fracJourney);
        topBarTransparent.color = Color.Lerp(Color.clear, transparentBarTarget, fracJourney);
        //xReplace.GetComponent<Image>().color = Color.Lerp(Color.clear, transparentBarTarget, 10.0f);
        //StartCoroutine(fadeDot());
        xReplace.SetActive(true);

        // When the lerp is completed
		if (fracJourney >= 1) {
			topBarOpaque.color = Color.white;
			topBarTransparent.color = transparentBarTarget;
			canFadeIn = false;
			// Debug.Log("Setting Can Fade to true");
			if (currentState == MenuState.BGSelect && !startTopBarUpdate) {
				startTopBarUpdate = true;
			}
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
                FadeInDot(3);
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

		if (topBarOpaque.fillAmount == topBarFillMax && !startTopBarUpdate) {
			return;
		}

		if ((int)previousState > (int)currentState)
		{
			topBarOpaque.fillAmount = Mathf.Lerp(topBarPreviousMax, topBarFillMax, topBarT);

			topBarT += speed * Time.deltaTime;

			if (topBarOpaque.fillAmount == topBarFillMax) {
				startTopBarUpdate = false;
				//updateTopBar = false;
				topBarT = 0;

				if (currentState == MenuState.PlayGame) {
					StartCoroutine(LoadScene());
				}

				if (currentState == MenuState.BGSelect) {
					buttons.SetActive (true);
				}
				else
				{
					buttons.SetActive(false);
				}
			}

		} else
		{
			topBarOpaque.fillAmount = Mathf.Lerp(topBarFillMin, topBarFillMax, topBarT);

			topBarT += speed * Time.deltaTime;

			if (topBarOpaque.fillAmount == topBarFillMax) {
				startTopBarUpdate = false;
				//updateTopBar = false;
				topBarT = 0;

				if (currentState == MenuState.PlayGame) {
					StartCoroutine(LoadScene());
				}

				if (currentState == MenuState.BGSelect) {
					buttons.SetActive(true);
				}
				else
				{
					buttons.SetActive(false);
				}
			}
		}
    }

    void ResetTopBar() {
        // Reset the fill value
        topBarOpaque.fillAmount = 0;
    }

    void ResetBottomBar() {
        bottomBarOpaque.fillAmount = 0;
		bottomBarT = 0;
    }

    void FadeInDot(int dotIndex) {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        bottomBarDots[0].color = Color.Lerp(Color.clear, Color.white, fracJourney);
        bottomBarDots[1].color = Color.Lerp(Color.clear, Color.white, fracJourney);
        bottomBarDots[2].color = Color.Lerp(Color.clear, Color.white, fracJourney);
    }

    void FadeOutDots(int dotIndex) {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / opaqueJourneyLength;

        bottomBarDots[0].color = Color.Lerp(Color.white, Color.clear, fracJourney);
        bottomBarDots[1].color = Color.Lerp(Color.white, Color.clear, fracJourney);
        bottomBarDots[2].color = Color.Lerp(Color.white, Color.clear, fracJourney);
    }

    public void ButtonClick(int val)
    {
        menuSFX.playTap();

		previousState = (MenuState)val - 1;

		if ((int)previousState < 0) {
			previousState = MenuState.Intro;
		}

        switch (val)
        {
            case 0:
                currentState = MenuState.Intro;
                //Debug.Log("X");
                ResetBackgroundSelection();
				buttons.SetActive(false);
                break;
            case 1:
				if (currentState == MenuState.Intro)
				{
					canFadeIn = true;
				}

                currentState = MenuState.BGSelect;
                ResetBackgroundSelection();
				buttons.SetActive(false);
                break;
            case 2:
                currentState = MenuState.SongSelect;
                // ChooseSong();
                break;
            case 3:
                currentState = MenuState.PlayGame;
                // Load Game
				buttons.SetActive(false);
                break;
            case 4:
                // Show Achievements
                currentState = MenuState.Achievements;
				buttons.SetActive(false);
                break;
            case 5:
                // Show Score Board
                currentState = MenuState.Scores;
				buttons.SetActive(false);
                break;
            default:
                break;
        }
        
        // Debug.Log("Clicked Button to state: " + currentState);
        UpdateMenuState();
        RegisterUpdate();
    }

	public void RunAwayAndNeverReturn()
	{
		if (currentState == MenuState.PlayGame) {
			return;
		}

		int cs = (int)currentState;

		if (cs > 0)
		{
			previousState = (MenuState)cs;

			cs -= 1;

			ButtonClick(cs);
		}
	}

    public void ChooseSong(string songTitle) {
        PlayerPrefs.SetString("sceneNumber", songTitle);
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
 
    IEnumerator LoadScene()
    {

        // Debug.Log("BG: " + PlayerPrefs.GetInt("bgNumber"));
        // Debug.Log("Song: " + PlayerPrefs.GetString("sceneNumber"));
        yield return new WaitForSeconds(1);
        if (PlayerPrefs.GetInt("bgNumber") != -1 && PlayerPrefs.GetString("sceneNumber") != "")
        {
            //Debug.Log(PlayerPrefs.GetInt("bgNumber"));
            //Debug.Log(PlayerPrefs.GetString("sceneNumber"));

            SceneManager.LoadScene("LoadingFacts");
        } else {

            startGame = false;
        }
    }

    public int GetCurrentState() {
        return (int)currentState;
    }


    IEnumerator fadeDot()
    {
    	float temp = 0.0f;
    	while(temp < 0.085f)
    	{
    		temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * 0.075f);
    		xReplace.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, temp);
    		yield return null;
    	}

		while(temp < 0.999f)
    	{
    		temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * 4.0f);
    		xReplace.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, temp);
    		yield return null;
    	}


    	yield return null;
    }

    public void fadeToCredits()
    {	
    	if(!isFading)
    	{
			creditsFade.gameObject.SetActive(true);
			StartCoroutine(fadeEm());
    	}
    }

    IEnumerator fadeEm()
    {
    	float temp = creditsFade.color.a;

    	while(temp < 0.999f)
    	{
    		temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * creditsFadeSpeed);

    		creditsFade.color = new Color(0.0f, 0.0f, 0.0f, temp);

    		yield return null;
    	}

    	isFading = false;
    	this.GetComponent<SceneLoad>().LoadScene("NewCredits");

    	yield return null;
    }
}
