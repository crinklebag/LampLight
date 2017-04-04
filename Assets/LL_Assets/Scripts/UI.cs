using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public Text bugsCaughtFG;
	public Text jarsFilledFG;
	public Text totalScoreMulFG;
    public Text totalScoreFG;
    public Text countdown;

    public Image FGOverlay;
    public Image progressBar;
    [SerializeField] private float progressFlashSpeed = 5.0f;

    [SerializeField] private Sprite[] finalPanelBgs;
    [SerializeField] private GameObject[] finalScoreObjs;

    //Wave panel stuffs
	[SerializeField]
    private GameObject WavePanel;
    [SerializeField]
    private GameObject WaveText;
    [SerializeField]
    private float desiredPanelHeight = -500.0f;
    [SerializeField]
    private float wavePanelMoveSpeed = 10.0f;
    [SerializeField]
    private float panelUpTime = 1.0f;
    private bool isMovingWave = false;
    private int WavePanelMoveCount = 0;
    private int WaveCount = 0;



    public ParticleSystem[] crackedJarFireflies;

    public Light[] pointLights;

    public SpriteRenderer[] glows;
    public SpriteRenderer[] jars;

    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken
    //public Sprite[] jarImagesMultiplier; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    public GameObject[] brokenHalfJars;
    public GameObject am;
    public GameObject exitButtonFG;

    public GameObject jar1Pos;

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

    int maxFireflies = 10;
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

    bool startedProgressBarFlash = false;
    bool startedFinalCountdown = false;

    private int scoreMultiplier = 1;

    [SerializeField] private float FinalScoreFinishWait = 5.0f;
	private float timeRemaining; 

	private bool calledFadeCoroutine = false;

	[SerializeField] Image toMenuFade;
    [SerializeField] float toMenuFadeSpeed;
    bool toMenuIsFading = false;

    void Awake()
    {
        countdown = GameObject.Find("Countdown").GetComponent<Text>();

		toMenuFade.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		toMenuFade.gameObject.SetActive(false);

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

            if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
            {
                brokenHalfJars[i] = jars[i].GetComponentInChildren<Rigidbody2D>().gameObject;
                brokenHalfJars[i].gameObject.GetComponent<Rigidbody2D>().simulated = true;
                brokenHalfJars[i].gameObject.SetActive(false);
            }
            
            crackedJarFireflies[i] = jars[i].GetComponentInChildren<ParticleSystem>();
            glows[i] = jars[i].GetComponentsInChildren<SpriteRenderer>()[1];
        }

        if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
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

        if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
        {
            //Debug.Log(Vector3.Distance(jars[0].gameObject.transform.position, jar1Pos.transform.position));

            //if (Vector3.Distance(jars[0].gameObject.transform.position, beginningJarPos[0]) >= 0.54f && Vector3.Distance(jars[0].gameObject.transform.position, beginningJarPos[0]) <= 0.65f && jarsYSetAlready[0] == false)
            if (Vector3.Distance(jars[0].gameObject.transform.position, jar1Pos.transform.position) < 0.5f && jarsYSetAlready[0] == false)
            {
                SetJarYPos(0);
            }
        }

        for (int i = 0; i < glows.Length; i++)
        {
            glows[i].color = Color32.Lerp(glows[i].color, currentColor[i], lerpColorTime);
            pointLights[i].intensity = Mathf.Lerp(pointLights[i].intensity, intensities[i], lerpColorTime);

            if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
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

        if (winGame)
        {
            //Debug.Log("Won Game??");
            ShowEndUI(gc.FilledJars);
        }

        if (startedGame)
        {
			timeRemaining = am.GetComponent<AudioSource>().clip.length - am.GetComponent<AudioSource>().time;

			wavePanelUpdate();
            progressBarUpdate();
            endGameUpdate();
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

        if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
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

                if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
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

            if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
            {
                jarsY[i] = beginningJarPos[i].y;
            }

            jarsYSetAlready[i] = false;
            jarsPulseAlready[i] = false;
        }

        lerpColorTime = 0;
        jarsYLerpTime = 0;

        if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
        {
            SetJarYPos(0);
        }
    }

    public void AddBug(int bugNumber) {
        lerpColorTime = 0;
        score += 1;

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

    public void ShowEndUI (int multiplier)
	{
		scoreMultiplier = multiplier;
        bugsCaughtFG.text = (score).ToString();
        jarsFilledFG.text = scoreMultiplier.ToString();

        setFinalImage();

        if(!calledFadeCoroutine)
        {
        	calledFadeCoroutine = true;
			StartCoroutine(fadeInScorePanel());
        }

        if (multiplier > 0)
        {
			totalScoreMulFG.text = (score).ToString() + " x " + multiplier.ToString ();
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

            if(tempScoreCounter < totalScore * 0.5f)//under 1/2
            {
				tempScoreCounter += 20;
				Debug.Log("under half");
            }
            else if(tempScoreCounter < totalScore * 0.75f)//under 3/4
            {
            	tempScoreCounter += 10;
            	Debug.Log("under three quarter");
            }
            else
            {
            	Debug.Log("the rest");
            	tempScoreCounter++;
            }

			yield return new WaitForSecondsRealtime(0.001f);
		}

        tempScoreCounter = totalScore;
        exitButtonFG.SetActive(true);

        yield return new WaitForSeconds(FinalScoreFinishWait);

		gc.ReturnToMenu();
    }

    public void CallPause()
    {
        theState = IngameMenuStates.PAUSE;
    }

    //Not needed?
    public void showFGOverlay()
    {
		bugsCaughtFG.text = (score/10).ToString();
		jarsFilledFG.text = scoreMultiplier.ToString ();

		FGOverlay.gameObject.SetActive(true);
    }

    void progressBarUpdate()
    {
		progressBar.fillAmount = am.GetComponent<AudioSource>().time / am.GetComponent<AudioSource>().clip.length;

		//Debug.Log("t: " + timeRemaining);

		if(timeRemaining <= 10.0f && !startedProgressBarFlash)
		{
			startedProgressBarFlash = true;
			StartCoroutine(flashProgressBar());
		}

        if (progressBar.fillAmount >= 0.999f)
        {
            winGame = true;
            gc.FinishGame();
		}
    }

    void endGameUpdate()
    {
		if(timeRemaining <= 5.0f && !startedFinalCountdown)
		{
			startedFinalCountdown = true;
			StartCoroutine(FinishGameCountdown());
		}
    }

    IEnumerator flashProgressBar()
    {
		float temp = progressBar.color.b;

    	while(temp > 0.0f)
    	{
    		temp = Mathf.MoveTowards(temp, 0.0f, Time.deltaTime * progressFlashSpeed);
    		progressBar.color =  new Color(progressBar.color.r, temp, temp);
    		yield return null;
    	}

    	yield return new WaitForSeconds(0.1f);

		while(temp < 1.0f)
    	{
    		temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * progressFlashSpeed);
			progressBar.color =  new Color(progressBar.color.r, temp, temp);
    		yield return null;
    	}

    	startedProgressBarFlash = false;
    	yield return null;
    }

    IEnumerator FinishGameCountdown()
    {
    	countdown.gameObject.SetActive(true);
    	float intervalTime = timeRemaining/3.0f;

    	for(int i = 3; i > 0; i--)
    	{
			countdown.text = i.ToString();

			countdown.gameObject.GetComponent<Animator>().Play("CountdownFade", -1, 0.0f);

			yield return new WaitForSeconds(intervalTime);
    	}

		countdown.gameObject.SetActive(false);
    	yield return null;
    }

    //Wave Panel shit
	public void wavePanelUpdate()
    {
        WaveText.GetComponent<Text>().text = "WAVE " + WaveCount.ToString();

        if (WavePanelMoveCount > 0 && !isMovingWave)
        {
            WavePanelMoveCount--;
            isMovingWave = true;
            StartCoroutine(moveWavePanel());
        }
    }

    IEnumerator moveWavePanel()
    {
        float tempY = WavePanel.transform.localPosition.y;

        while (tempY < (desiredPanelHeight - 0.01f))
        {
            tempY = Mathf.MoveTowards(tempY, desiredPanelHeight, Time.deltaTime * wavePanelMoveSpeed);
            WavePanel.transform.localPosition = new Vector3(WavePanel.transform.localPosition.x, tempY, WavePanel.transform.localPosition.z);
            yield return null;
        }

        yield return new WaitForSeconds(panelUpTime);

        while (tempY > -749.99f)
        {
            tempY = Mathf.MoveTowards(tempY, -750.0f, Time.deltaTime * wavePanelMoveSpeed);
            WavePanel.transform.localPosition = new Vector3(WavePanel.transform.localPosition.x, tempY, WavePanel.transform.localPosition.z);
            yield return null;
        }

        isMovingWave = false;
        yield return null;
    }

    public void incWaveCount()
    {
        WaveCount++;
        WavePanelMoveCount++;
    }

	void setFinalImage()
	{
		switch (PlayerPrefs.GetInt("bgNumber"))
        {
            case 1:
				FGOverlay.sprite = finalPanelBgs[0];
                break;
            case 2:
				FGOverlay.sprite = finalPanelBgs[1];
                break;
            case 3:
				FGOverlay.sprite = finalPanelBgs[2];
                break;
            case 4:
				FGOverlay.sprite = finalPanelBgs[3];
                break;
        }
	}

	IEnumerator fadeInScorePanel()
	{
		for(int i = 0; i < finalScoreObjs.Length; i++)
		{
			finalScoreObjs[i].SetActive(false);
		}

		FGOverlay.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		FGOverlay.gameObject.SetActive(true);

		float tempA = FGOverlay.color.a;

		while(tempA < 0.999f)
		{
			tempA = Mathf.MoveTowards(tempA, 1.0f, Time.deltaTime * 5.0f);

			FGOverlay.color = new Color(1.0f, 1.0f, 1.0f, tempA);

			yield return null;
		}

		for(int i = 0; i < finalScoreObjs.Length; i++)
		{
			finalScoreObjs[i].SetActive(true);
		}

		yield return null;
	}


	public void fadeToMenu()
    {	
    	if(!toMenuIsFading)
    	{
			toMenuFade.gameObject.SetActive(true);
			StartCoroutine(fadeEm());
    	}
    }

    IEnumerator fadeEm()
    {
    	float temp = toMenuFade.color.a;

    	while(temp < 0.999f)
    	{
    		temp = Mathf.MoveTowards(temp, 1.0f, Time.deltaTime * toMenuFadeSpeed);

    		toMenuFade.color = new Color(0.0f, 0.0f, 0.0f, temp);

    		yield return null;
    	}

    	toMenuIsFading = false;
    	this.GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");

    	yield return null;
    }
}
