using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    public int[] TimesFireflyWentHere { get { return timesFireflyWentHere; } set { timesFireflyWentHere = value; } }
    public bool[] JarsYSetAlready { get { return jarsYSetAlready; } set { jarsYSetAlready = value; } }
    public bool WinGame { get { return winGame; } set { winGame = value; } }

    public enum IngameMenuStates
    {
        PLAY,
        PAUSE,
        EXIT
    }

    public IngameMenuStates theState;

    public Text scoreText;
    //public Text multiplierText;
    //public Image uiJarMultiplier;
    //public Image fireflyJarMultiplier;
    public Text bugsCaughtFG;
	public Text jarsFilledFG;
	public Text totalScoreMulFG;
    public Text totalScoreFG;
    public Text countdown;

    public Image FGOverlay;
    public Image progressBar;

    public ParticleSystem[] crackedJarFireflies;

    public Light[] pointLights;

    public SpriteRenderer[] glows;
    public SpriteRenderer[] jars;

    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken
    //public Sprite[] jarImagesMultiplier; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    public GameObject[] brokenHalfJars;
    public GameObject am;
    public GameObject exitButtonFG;

    GameController gc;

    [SerializeField] private Color32[] currentColor;

    [SerializeField] Vector3[] beginningJarPos;

    [SerializeField] float[] intensities;
    [SerializeField] float[] fireflyColorConvert;
    [SerializeField] float[] jarsY;
    [SerializeField] float jarDistance;
    [SerializeField] float jarDistanceOffset;
    float lerpColorTime = 0;
    float fireflyColorConvertUI;
    float maxLightIntensity = 0.6f;
    float jarsYLerpTime = 0;

    int maxFireflies = 30;
    int score = 0;
    int totalScore = 0;
    int tempScoreCounter = 0;
    [SerializeField] int[] timesFireflyWentHere;

    [SerializeField] bool hasTouchedAtEnd = false;
    [SerializeField] bool[] jarsYSetAlready;
    [SerializeField] bool[] jarsPulseAlready;
    bool calledCountUpCoroutine = false;
    [SerializeField] bool winGame = false;
    bool startedGame = false;

    void Awake()
    {
        countdown = GameObject.Find("Countdown").GetComponent<Text>();

        currentColor = new Color32[jars.Length];
        jarsY = new float[jars.Length];
        fireflyColorConvert = new float[jars.Length];
        intensities = new float[jars.Length];
        jarsYSetAlready = new bool[jars.Length];
        jarsPulseAlready = new bool[jars.Length];
        beginningJarPos = new Vector3[jars.Length];
        timesFireflyWentHere = new int[jars.Length + 1];

        pointLights = new Light[jars.Length];
        brokenHalfJars = new GameObject[jars.Length];
        crackedJarFireflies = new ParticleSystem[jars.Length];
        glows = new SpriteRenderer[jars.Length];

        for (int i = 0; i < jars.Length; i++)
        {
            pointLights[i] = jars[i].GetComponentInChildren<Light>();

            if (Application.loadedLevelName != "Main_Mobile_Waterfall")
            {
                brokenHalfJars[i] = jars[i].GetComponentInChildren<Rigidbody2D>().gameObject;
                brokenHalfJars[i].gameObject.GetComponent<Rigidbody2D>().simulated = true;
                brokenHalfJars[i].gameObject.SetActive(false);
            }
            
            crackedJarFireflies[i] = jars[i].GetComponentInChildren<ParticleSystem>();
            glows[i] = jars[i].GetComponentsInChildren<SpriteRenderer>()[1];
        }

        if (Application.loadedLevelName != "Main_Mobile_Waterfall")
        {
            for (int i = 0; i < jars.Length; i++)
            {
                currentColor[i] = Color.clear;
                intensities[i] = 0.0f;
                jarsYSetAlready[i] = false;
                jarsPulseAlready[i] = false;

                jars[i].gameObject.transform.position = new Vector3(jars[i].gameObject.transform.position.x, jars[i].gameObject.transform.position.y + (jarDistance - jarDistanceOffset), jars[i].gameObject.transform.position.z);

                beginningJarPos[i] = new Vector3(jars[i].gameObject.transform.position.x, jars[i].gameObject.transform.position.y, jars[i].gameObject.transform.position.z);

                jarsY[i] = beginningJarPos[i].y;
            }
        }
        
        countdown.gameObject.SetActive(false);
        startedGame = false;

        theState = IngameMenuStates.PLAY;
    }

    // Use this for initialization
    void Start ()
    {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        StartCoroutine("Countdown");
    }

    // Update is called once per frame
    void Update ()
    {
        lerpColorTime = (lerpColorTime += Time.deltaTime) / 1f;

        timesFireflyWentHere[5] = gc.FilledJars;

        if (Application.loadedLevelName != "Main_Mobile_Waterfall")
        {
            if (Vector3.Distance(jars[0].gameObject.transform.position, beginningJarPos[0]) >= 0.54f && Vector3.Distance(jars[0].gameObject.transform.position, beginningJarPos[0]) <= 0.65f && jarsYSetAlready[0] == false)
            {
                SetJarYPos(0);
            }
        }

        for (int i = 0; i < glows.Length; i++)
        {
            glows[i].color = Color32.Lerp(glows[i].color, currentColor[i], lerpColorTime);
            pointLights[i].intensity = Mathf.Lerp(pointLights[i].intensity, intensities[i], lerpColorTime);

            if (Application.loadedLevelName != "Main_Mobile_Waterfall")
            {
                float y = Mathf.Lerp(jars[i].gameObject.transform.position.y, jarsY[i], jarsYLerpTime += (Time.deltaTime * 0.01f));

                jars[i].gameObject.transform.position = new Vector3(jars[i].gameObject.transform.position.x, y, jars[i].gameObject.transform.position.z);

                if (jarsYSetAlready[i] && !jarsPulseAlready[i] && jars[i].gameObject.transform.position.y <= jarsY[i] + 0.1f)
                {
                    jars[i].gameObject.GetComponent<JarPulse>().SetPulse(true);
                    jarsPulseAlready[i] = true;
                }
            }
            else
            {
                if (jarsYSetAlready[i] && !jarsPulseAlready[i])
                {
                    jars[i].gameObject.GetComponent<JarPulse>().SetPulse(true);
                    jarsPulseAlready[i] = true;
                }
            }
        }

            // to reset after every 5 jars are all full
            if (timesFireflyWentHere[4] >= maxFireflies && jarsYSetAlready[4])
            {
                jarsYSetAlready[4] = false;
                ResetJars();
            }

            if (winGame && glows[0].color == currentColor[0] && glows[1].color == currentColor[1] && glows[2].color == currentColor[2] && glows[3].color == currentColor[3] && glows[4].color == currentColor[4] )
            {
                FinishGame(gc.FilledJars);
            }

        if (startedGame)
        {
            progressBar.fillAmount = am.GetComponent<AudioSource>().time / am.GetComponent<AudioSource>().clip.length;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            //ResetGlow();

            if (!hasTouchedAtEnd)
            {
                Debug.Log("Touched at end using key");
                hasTouchedAtEnd = true;
            }
        }

        scoreText.text = gc.GetAmountOfBugs().ToString();

        switch (theState)
        {
            case IngameMenuStates.PLAY:
                {

                }
                break;
            case IngameMenuStates.PAUSE:
                {

                }
                break;
            case IngameMenuStates.EXIT:
                {

                }
                break;
            default:
                break;
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSecondsRealtime(2);

        for (int i = 3; i >= 0; i--)
        {
            countdown.gameObject.SetActive(true);

            if (i == 0)
            {
                countdown.text = "GO!";
            }
            else
            {
                countdown.text = i.ToString();
            }
            
            countdown.gameObject.GetComponent<Animator>().Play("CountdownFade", -1, 0.0f);

            yield return new WaitForSecondsRealtime(1);
        }

        countdown.gameObject.SetActive(false);

        if (Application.loadedLevelName == "Main_Mobile_Waterfall")
        {
            SetJarYPos(0);
        }

        startedGame = true;

        gc.StartGameAfterCountdown();
    }

    public void SetJarYPos(int val)
    {
        if (val < jars.Length)
        {
            jarsYLerpTime = 0;
            jarsY[val] = beginningJarPos[val].y - jarDistance;
            jarsYSetAlready[val] = true;
        }
    }

    public void setJarImage(int val)
    {
        for (int i = 0; i < jars.Length; i++)
        {
            jars[i].sprite = jarImages[val];
        }
    }

    public Color GetBugInJarColor(int bugNumber)
    {
        return glows[bugNumber].color;
    }

    public void ResetGlow()
    {
        lerpColorTime = 0;
        //currentColorMultiplier = Color.clear;

        for (int i = 0; i < glows.Length; i++)
        {
            if (jarsYSetAlready[i] == true)
            {
                fireflyColorConvert[i] = 0;
                currentColor[i] = new Color32(255,255,255, (byte)fireflyColorConvert[i]);
                intensities[i] = 0.0f;

                if (Application.loadedLevelName != "Main_Mobile_Waterfall")
                {
                    brokenHalfJars[i].SetActive(true);
                    brokenHalfJars[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), Random.Range(-30, -10)));
                }

                crackedJarFireflies[i].Play();
            }
        }
    }
    
    public void ResetJars()
    {
        for (int i = 0; i < glows.Length; i++)
        {
            fireflyColorConvert[i] = 0;
            currentColor[i] = new Color32(255,255,255, (byte)fireflyColorConvert[i]);
            intensities[i] = 0.0f;
            timesFireflyWentHere[i] = 0;

            if (Application.loadedLevelName != "Main_Mobile_Waterfall")
            {
                jarsY[i] = beginningJarPos[i].y;
            }

            jarsYSetAlready[i] = false;
            jarsPulseAlready[i] = false;
        }

        lerpColorTime = 0;
        jarsYLerpTime = 0;

        if (Application.loadedLevelName == "Main_Mobile_Waterfall")
        {
            SetJarYPos(0);
        }
    }

    public void AddBug(int bugNumber) {
        lerpColorTime = 0;
        score += 10;

        if (fireflyColorConvert[bugNumber] >= 0 && fireflyColorConvert[bugNumber] < 255)
        {
            fireflyColorConvert[bugNumber] += (25.5f / (maxFireflies / 10));
        }

        if (fireflyColorConvert[bugNumber] > 255)
        {
            fireflyColorConvert[bugNumber] = 255;
        }

        if (fireflyColorConvertUI >= 0 && fireflyColorConvertUI < 255)
        {
            fireflyColorConvertUI += 25.5f;
        }

        if (fireflyColorConvertUI > 255)
        {
            fireflyColorConvertUI = 255;
        }

        if (pointLights[bugNumber].intensity < maxLightIntensity)
        {
            intensities[bugNumber] += 0.05f;
        }

        timesFireflyWentHere[bugNumber]++;

        if (timesFireflyWentHere[bugNumber] == maxFireflies)
        {
            gc.FilledJars++;
            SetJarYPos(gc.FilledJars % 5);
        }

        currentColor[bugNumber] = new Color32(255, 255, 255,(byte)fireflyColorConvert[bugNumber]);

        jars[bugNumber].GetComponent<JarPulse>().SetPulse(true);
    }

    public void RemoveBug(int bugNumber)
    {
        lerpColorTime = 0;

        if (fireflyColorConvert[bugNumber] > 0)
        {
            fireflyColorConvert[bugNumber] -= (25.5f / (maxFireflies / 10));
        }

        if (fireflyColorConvert[bugNumber] < 0)
        {
            fireflyColorConvert[bugNumber] = 0;
        }

        if (fireflyColorConvertUI > 0)
        {
            fireflyColorConvertUI -= 25.5f;
        }

        if (fireflyColorConvertUI < 0)
        {
            fireflyColorConvertUI = 0;
        }

        intensities[bugNumber] -= 0.05f;

        if (intensities[bugNumber] < 0.0f)
        {
            intensities[bugNumber] = 0.0f;
        }

        if (timesFireflyWentHere[bugNumber] > 0)
        {
            timesFireflyWentHere[bugNumber]--;
        }

        currentColor[bugNumber] = new Color32(255, 255, 255, (byte)fireflyColorConvert[bugNumber]);
        
    }

    public void FinishGame (int multiplier)
	{
		bugsCaughtFG.text = (score/10).ToString();
		jarsFilledFG.text = multiplier.ToString ();

        //AE - FGOverlay.gameObject.SetActive(true); - want to move this as the timing seems to be off, as in the level will become dark sometimes before the overlay is up

        if (multiplier > 0)
        {
			totalScoreMulFG.text = score.ToString () + " x " + multiplier.ToString ();
			totalScoreFG.text = tempScoreCounter.ToString ();
            totalScore = score * multiplier;
        }
        else
        {
			totalScoreMulFG.text = tempScoreCounter.ToString ();
			totalScoreFG.text = "";
            totalScore = score;
        }

		if (!calledCountUpCoroutine)
		{
			StartCoroutine("CountUpScore");
			calledCountUpCoroutine = true;
		}
    }

    public void SetHasTouchedAtEnd(bool val)
    {
        hasTouchedAtEnd = val;
    }

    IEnumerator CountUpScore()
    {
		yield return new WaitForSecondsRealtime(0.001f);

		while (tempScoreCounter < totalScore)
		{
            if (hasTouchedAtEnd)
            {
                break;
            }

            tempScoreCounter++;

			yield return new WaitForSecondsRealtime(0.001f);
		}

        tempScoreCounter = totalScore;
        exitButtonFG.SetActive(true);

        yield return new WaitForSeconds(3.0f);

		gc.ReturnToMenu();
    }

    public void CallPause()
    {
        theState = IngameMenuStates.PAUSE;
    }

    public void ChangeState()
    {
        switch (EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text)
        {
            case "RESUME":
                {

                }
                break;
            case "RESTART":
                {

                }
                break;
            case "EXIT GAME":
                {

                }
                break;
            default:
                break;
        }
    }

    public void showFGOverlay()
    {
		FGOverlay.gameObject.SetActive(true);
    }
}
