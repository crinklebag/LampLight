using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashSceneController : MonoBehaviour {

	/*
    public enum SplashScreenState { SHOW_HANDSOMEDOG_LOGO, SHOW_HANDSOMEDOG_GLOW, HIDE_HANDSOMEDOG_GLOW, HIDE_HANDSOMEDOG_LOGO,
                                    SHOW_ISTHESOUL_LOGO, SHOW_ISTHESOUL_GLOW, HIDE_ISTHESOUL_LOGO, HIDE_ISTHESOUL_GLOW };
    SplashScreenState currentState;
    */

    [SerializeField] Image handsomeDogLogo;
    [SerializeField] Image handsomeDogGlow;
    [SerializeField] Image isthesoulDogLogo;
    [SerializeField] Image isthesoulDogGlow;

    [SerializeField] float speed;
    /*
    bool canFade = false;

    float startTime;
    float journeyLength;
    Image currentUIPiece;
    */

    void Start()
    {
    	StartCoroutine(fadeRoutine());
    }

    IEnumerator fadeRoutine()
    {
    	for(int i = 0; i < 2; i++)
    	{
			yield return StartCoroutine(fadeIn(i));
			yield return StartCoroutine(fadeOut(i));
    	}

		this.GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");

    	yield return null;
    }


    IEnumerator fadeIn(int index)
    {
    	float tempLogo;
    	float tempGlow;
    	if(index == 0)
    	{
    		tempLogo = handsomeDogLogo.color.a;
    		tempGlow = handsomeDogGlow.color.a;
    		while(tempLogo < 0.999f)
    		{
    			tempLogo = Mathf.MoveTowards(tempLogo, 1.0f, Time.deltaTime * speed);
    			handsomeDogLogo.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			yield return null;
    		}
			while(tempGlow < 0.999f)
    		{
    			tempGlow = Mathf.MoveTowards(tempGlow, 1.0f, Time.deltaTime * speed);
    			handsomeDogGlow.color = new Color(1.0f, 1.0f, 1.0f, tempGlow);
    			yield return null;
    		}
    	}
    	else
    	{
			tempLogo = isthesoulDogLogo.color.a;
    		tempGlow = isthesoulDogGlow.color.a;
    		while(tempLogo < 0.999f)
    		{
    			tempLogo = Mathf.MoveTowards(tempLogo, 1.0f, Time.deltaTime * speed);
    			isthesoulDogLogo.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			yield return null;
    		}
			while(tempGlow < 0.999f)
    		{
    			tempGlow = Mathf.MoveTowards(tempGlow, 1.0f, Time.deltaTime * speed);
    			isthesoulDogGlow.color = new Color(1.0f, 1.0f, 1.0f, tempGlow);
    			yield return null;
    		}
    	}

    	yield return null;
    }

    IEnumerator fadeOut(int index)
    {
    	Debug.Log("fade out");

		float tempLogo;
    	float tempGlow;
    	if(index == 0)
    	{
    		tempLogo = handsomeDogLogo.color.a;
    		tempGlow = handsomeDogGlow.color.a;
    		while(tempLogo > 0.001f)
    		{
    			tempLogo = Mathf.MoveTowards(tempLogo, 0.0f, Time.deltaTime * speed);
    			handsomeDogLogo.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			handsomeDogGlow.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			yield return null;
    		}
    	}
    	else
    	{
			tempLogo = isthesoulDogLogo.color.a;
    		tempGlow = isthesoulDogGlow.color.a;
    		while(tempLogo > 0.001f)
    		{
    			tempLogo = Mathf.MoveTowards(tempLogo, 0.0f, Time.deltaTime * speed);
    			isthesoulDogLogo.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			isthesoulDogGlow.color = new Color(1.0f, 1.0f, 1.0f, tempLogo);
    			yield return null;
    		}
    	}

    	yield return null;
    }

    // Use this for initialization
    /*
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
                //Debug.Log("Load Level");
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
    */
}
