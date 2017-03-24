using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashSceneController : MonoBehaviour {

    public enum SplashScreenState { SHOW_HANDSOMEDOG_LOGO, SHOW_HANDSOMEDOG_GLOW, HIDE_HANDSOMEDOG_GLOW, HIDE_HANDSOMEDOG_LOGO,
                                    SHOW_ISTHESOUL_LOGO, SHOW_ISTHESOUL_GLOW, HIDE_ISTHESOUL_LOGO, HIDE_ISTHESOUL_GLOW };
    SplashScreenState currentState;

    [SerializeField] Image handsomeDogLogo;
    [SerializeField] Image handsomeDogGlow;
    [SerializeField] Image isthesoulDogLogo;
    [SerializeField] Image isthesoulDogGlow;

    [SerializeField] float speed;

    bool canFade = false;

    float startTime;
    float journeyLength;
    Image currentUIPiece;

    // Use this for initialization
	void Start () {
        canFade = false;
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        if (canFade) { UpdateMenuState(); }
	}

    void Initialize() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(Vector3.zero, Vector3.one);

        handsomeDogGlow.color = Color.clear;
        handsomeDogLogo.color = Color.clear;
        isthesoulDogGlow.color = Color.clear;
        isthesoulDogLogo.color = Color.clear;

        currentState = SplashScreenState.SHOW_HANDSOMEDOG_LOGO;
        canFade = true;
    }

    void UpdateMenuState() {
        if (currentState == SplashScreenState.HIDE_HANDSOMEDOG_GLOW) {
            currentUIPiece = handsomeDogGlow;
            FadeOut();
        } else if (currentState == SplashScreenState.SHOW_HANDSOMEDOG_GLOW) {
            currentUIPiece = handsomeDogGlow;
            FadeIn();
        }
        else if (currentState == SplashScreenState.HIDE_HANDSOMEDOG_LOGO) {
            currentUIPiece = handsomeDogLogo;
            FadeOut();
        } else if (currentState == SplashScreenState.SHOW_HANDSOMEDOG_LOGO) {
            currentUIPiece = handsomeDogLogo;
            FadeIn();
        }
        else if (currentState == SplashScreenState.HIDE_ISTHESOUL_GLOW) {
            currentUIPiece = isthesoulDogGlow;
            FadeOut();
        }
        else if (currentState == SplashScreenState.SHOW_ISTHESOUL_GLOW) {
            currentUIPiece = isthesoulDogGlow;
            FadeIn();
        }
        else if (currentState == SplashScreenState.HIDE_ISTHESOUL_LOGO) {
            currentUIPiece = isthesoulDogLogo;
            FadeOut();
        } else if (currentState == SplashScreenState.SHOW_ISTHESOUL_LOGO) {
            currentUIPiece = isthesoulDogLogo;
            FadeIn();
        }
    }

    void FadeIn() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        currentUIPiece.color = Color.Lerp(Color.clear, Color.white, fracJourney);

        if (fracJourney >= 1) {
            if (currentState == SplashScreenState.SHOW_HANDSOMEDOG_LOGO) {
                journeyLength = Vector3.Distance(Vector3.zero, Vector3.one);
                startTime = Time.time;
                currentState = SplashScreenState.SHOW_HANDSOMEDOG_GLOW;
            }
            else if (currentState == SplashScreenState.SHOW_HANDSOMEDOG_GLOW) {
                StartCoroutine(FadeDelay());
            }
            else if (currentState == SplashScreenState.SHOW_ISTHESOUL_LOGO) {
                journeyLength = Vector3.Distance(Vector3.zero, Vector3.one);
                startTime = Time.time;
                currentState = SplashScreenState.SHOW_ISTHESOUL_GLOW;
            }
            else if (currentState == SplashScreenState.SHOW_ISTHESOUL_GLOW) {
                StartCoroutine(FadeDelay());
            }
        }
    }

    void FadeOut()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        currentUIPiece.color = Color.Lerp(Color.white, Color.clear, fracJourney);

        if (fracJourney >= 1) {
            if (currentState == SplashScreenState.HIDE_HANDSOMEDOG_GLOW) {
                currentState = SplashScreenState.HIDE_HANDSOMEDOG_LOGO;
                startTime = Time.time;
            }
            else if (currentState == SplashScreenState.HIDE_HANDSOMEDOG_LOGO) {
                StartCoroutine(FadeDelay());
            }
            else if (currentState == SplashScreenState.HIDE_ISTHESOUL_GLOW) {
                currentState = SplashScreenState.HIDE_ISTHESOUL_LOGO;
                startTime = Time.time;
            } else if (currentState == SplashScreenState.HIDE_ISTHESOUL_LOGO) {
                Debug.Log("Load Level");
                GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");
            }
        }
    }

    IEnumerator FadeDelay() {
        canFade = false;

        yield return new WaitForSeconds(1);

        canFade = true;
        startTime = Time.time;

        if (currentState == SplashScreenState.SHOW_HANDSOMEDOG_GLOW) {
            currentState = SplashScreenState.HIDE_HANDSOMEDOG_GLOW;
        } else if (currentState == SplashScreenState.HIDE_HANDSOMEDOG_LOGO) {
            currentState = SplashScreenState.SHOW_ISTHESOUL_LOGO;
        } else if (currentState == SplashScreenState.SHOW_ISTHESOUL_GLOW) {
            currentState = SplashScreenState.HIDE_ISTHESOUL_GLOW;
        }
    }
}
