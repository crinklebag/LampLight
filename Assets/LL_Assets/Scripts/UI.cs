using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour {

    // GETS/SETS ----------------------------------------------------------------------------------------------------------------------------------------------------

    // Used to check how many fireflies are in what jar
    public int[] TimesFireflyWentHere { get { return timesFireflyWentHere; } set { timesFireflyWentHere = value; } }
    
    // Used to check if a jar has been lowered already
    public bool[] JarsYSetAlready { get { return jarsYSetAlready; } set { jarsYSetAlready = value; } }

    // Used to set the gameover
    public bool WinGame { get { return winGame; } set { winGame = value; } }

    // INGAME MENU STATES -------------------------------------------------------------------------------------------------------------------------------------------

    public enum IngameMenuStates
    {
        PLAY,
        PAUSE,
        EXIT
    }

    [Header("Current Menu State")]
    [Tooltip("This is used to let the game call and put away the pause menu.")]
    public IngameMenuStates theState;

    // GAMECONTROLLER -----------------------------------------------------------------------------------------------------------------------------------------------

    [Header("GameController")]
    [Tooltip("A reference to the GameController in the scene.")]
    GameController gc;

    // PARTICLE SYSTEM ----------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Jar Particle System")]
    [Tooltip("These are the particle system references from the jars in the scene.")]
    public ParticleSystem[] crackedJarFireflies;

    // LIGHT --------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Jar Lights")]
    [Tooltip("These are the light references from the jars in the scene.")]
    public Light[] pointLights;

    // COLOR --------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Jar Colors")]
    [Tooltip("These are the colors used for the firefly sprites in the jars.")]
    [SerializeField] private Color32[] currentColor;

    // VECTOR -------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Hanging Jar Beginning Positions")]
    [Tooltip("These are used to put the jars back up where they were when the jars are reset.")]
    [SerializeField] Vector3[] beginningJarPos;

    // GAMEOBJECT ----------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Broken Halves of the Jars")]
    [Tooltip("These are references to the cracked lower half of the jar which will fly off when the player loses.")]
    public GameObject[] brokenHalfJars;
    [Header("Audio Manager GameObject")]
    [Tooltip("Reference to the audio manager.")]
    public GameObject am;
    [Header("Exit Button (Score Screen)")]
    [Tooltip("The button the player can press to get out of the score screen (in the canvas it's under FinishGame)")]
    public GameObject exitButtonFG;
    [Header("Jar 1 Position (if scene is forest)")]
    [Tooltip("This jar reference is used to compare distances between the first jar in the jars array and the empty gameobject. If it is around the same position, the jar will lower.")]
    public GameObject jar1Pos;
    [Header("Score Screen GameObjects")]
    [Tooltip("This just holds the references to all the things in the score screen so it can be set active at the end or set unactive at the beginning.")]
    [SerializeField] private GameObject[] finalScoreObjs;
    [Header("Wave Notification")]
    [Tooltip("This is the reference to the wave notification that pops up whenever you get a green firefly.")]
	[SerializeField] private GameObject WavePanel;
    [Header("Wave Notification Text")]
    [Tooltip("Used to indicate what number the wave is at.")]
    [SerializeField] private GameObject WaveText;
	[Header("Top Bound")]
	[Tooltip("Used as a reference point for the hanging jars in the forest scene.")]
	[SerializeField] private GameObject topBound;

    // TEXT -----------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Score Text")]
    [Tooltip("Shows the current score.")]
    public Text scoreText;
    [Header("Bugs Caught (Score Screen)")]
    [Tooltip("Shows the amount of bugs you caught at the end of the game.")]
    public Text bugsCaughtFG;
    [Header("Jars Filled (Score Screen)")]
    [Tooltip("Shows the amount of jars you filled at the end of the game.")]
	public Text jarsFilledFG;
    [Header("Total Score (Score Screen)")]
    [Tooltip("Shows the total score at the end of the game.")]
    public Text totalScoreFG;
    [Header("Countdown to Start of Game")]
    [Tooltip("Shows the countdown at the beginning of the game.")]
    public Text countdown;

    // IMAGE ----------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Finished Game Overlay")]
    [Tooltip("This is the kind of transparent image that shows the score screen. Only this needs to be set active because the texts and whatnot are under it.")]
    public Image FGOverlay;
    [Header("Progress Bar")]
    [Tooltip("This is basically a seeker, but it's not interactable. Used to trigger FinishGame in GameController as well.")]
    public Image progressBar;
    [Header("Menu Fade")]
    [Tooltip("Sets this gameobject to true when you want to fade to the menu.")]
	[SerializeField] Image toMenuFade;

    // SPRITE RENDERER -------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Firefly Glows")]
    [Tooltip("These are the glows in the jars that turn brighter every time you collect a firefly. They also reset back to 0 when the jars are reset.")]
    public SpriteRenderer[] glows;
    [Header("Jars")]
    [Tooltip("These are the references to the jars sprites so they can change when the player gets hit by a bad firefly.")]
    public SpriteRenderer[] jars;

    // SPRITE -----------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Jar Images")]
    [Tooltip("Holds a reference to all the jar images used in the game.")]
    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken
    //public Sprite[] jarImagesMultiplier; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken
    [Header("Score Screen BGs")]
    [Tooltip("The image shown during the score screen when the game is over.")]
    [SerializeField] private Sprite[] finalPanelBgs;

    // FLOAT ------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Light Intensities")]
    [Tooltip("Used to set the lights in the jars.")]
    [SerializeField] float[] intensities;
    [Header("Glow Alphas")]
    [Tooltip("These contain the alphas from the glows in the jars.")]
    [SerializeField] float[] fireflyColorConvert;
    [Header("Jars Y Positions")]
    [Tooltip("These are the y positions of the jars. They're used for dropping down the jars and resetting the jars.")]
    [SerializeField] float[] jarsY;
    [Header("Jar Distance")]
    [Tooltip("The height the jars are supposed to be when they aren't active.")]
    [SerializeField] float jarDistance;
    [Header("Jar Distance Offset")]
    [Tooltip("Set this number so the jars can drop lower/higher.")]
    [SerializeField] float jarDistanceOffset;
    [Header("Color Lerp Float")]
    [Tooltip("Used to lerp the glows and intensities.")]
    float lerpColorTime = 0;
    [Header("Max Light Intensity")]
    [Tooltip("Sets the max intensity the lights in the jars can go up to.")]
    float maxLightIntensity = 0.6f;
    [Header("Jars Y Lerp Float")]
    [Tooltip("Used to lerp the hanging jars y position.")]
    float jarsYLerpTime = 0;
    [Header("Wave Panel Height")]
    [Tooltip("The amount where the wave panel will stop when moving up.")]
    [SerializeField] private float desiredPanelHeight = -500.0f;
    [Header("Wave Panel Move Speed")]
    [Tooltip("How fast the panel moves up.")]
    [SerializeField] private float wavePanelMoveSpeed = 10.0f;
    [Header("Wave Panel Show Time")]
    [Tooltip("How long the wave panel is on the screen for.")]
    [SerializeField] private float panelUpTime = 1.0f;
    [Header("Progress Bar Flash Speed")]
    [Tooltip("How fast the flashing is in the progress bar when the song is almost over.")]
    [SerializeField] private float progressFlashSpeed = 5.0f;
    [Header("Score Screen Wait")]
    [Tooltip("Used to pause the score screen.")]
    [SerializeField] float FinalScoreFinishWait = 5.0f;
    [Header("Song Time Remaining")]
    [Tooltip("A variable that is used to check when the song is almost over.")]
	private float timeRemaining; 
    [Header("Menu Fade Speed")]
    [Tooltip("How fast the fade is for the toMenuFade image.")]
    [SerializeField] float toMenuFadeSpeed;

    // INT --------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("Max Fireflies Per Jar")]
    [Tooltip("The amount of fireflies needed per jar so the jars can switch when full.")]
    int maxFireflies = 10;
    [Header("Score")]
    [Tooltip("A counter for the score.")]
    int score = 0;
    [Header("Total Score")]
    [Tooltip("The score after being multiplied with the jars filled.")]
    int totalScore = 0;
    [Header("Score Counter")]
    [Tooltip("A counter that shows the score being counted up in the score screen.")]
    int tempScoreCounter = 0;
    [Header("Wave Panel Move Count")]
    [Tooltip("Used to check if the player has hit a green firefly so it can trigger the moveWavePanel coroutine.")]
    private int WavePanelMoveCount = 0;
    [Header("Wave Counter")]
    [Tooltip("Keeps track of the times you've hit the green firefly.")]
    private int WaveCount = 0;
    [Header("Score Multiplier")]
    [Tooltip("Is basically the jars filled.")]
    private int scoreMultiplier = 1;
    [Header("Firefly counter per Jar")]
    [Tooltip("This is just used to see how many fireflies are in each jar. When all the jars have been filled, it will reset to 0. The last element is just for us to see how many jars are filled.")]
    [SerializeField] int[] timesFireflyWentHere;

    // BOOL -------------------------------------------------------------------------------------------------------------------------------------------------------

    [Header("End Signal")]
    [Tooltip("This is the bool that checks if the player has touched the screen (or when testing, has pressed L) when the game is at the score counting screen.")]
    [SerializeField] bool hasTouchedAtEnd = false;
    [Header("Hanging Jars Bools")]
    [Tooltip("This gets set to true when the jars have been dropped down.")]
    [SerializeField] bool[] jarsYSetAlready;
    [Header("Starting Jar Pulse Bools")]
    [Tooltip("Every time a jar has been filled, a new jar will drop (in certain scenes), and the jar pulses to indicate that is the current jar you're filling up.")]
    [SerializeField] bool[] jarsPulseAlready;
    [Header("Score Counting Bool")]
    [Tooltip("This value is here so the CountUpScore coroutine gets called only once.")]
    bool calledCountUpCoroutine = false;
    [Header("Win/Lose Bool Condition")]
    [Tooltip("A bool used to set the game to end. There are various places where this variable is used: progress bar (to call GameController's FinishGame), GameController (FinishGame)")]
    [SerializeField] bool winGame = false;
    [Header("When the game actually starts")]
    [Tooltip("this bool will be set to true. Used in the countdown at the beginning to prevent the player from doing certain things.")]
    bool startedGame = false;
    [Header("Progress Bar Warning")]
    [Tooltip("Flashes the progress bar to tell you the song is almost over.")]
    bool startedProgressBarFlash = false;
    [Header("Final Countdown")]
    [Tooltip("Plays the countdown near the end of the song.")]
    bool startedFinalCountdown = false;
    [Header("Call Fade Coroutine")]
    [Tooltip("Sets whether to call fadeInScorePanel.")]
	bool calledFadeCoroutine = false;
    [Header("Is Moving Wave Panel")]
    [Tooltip("Checks if it is in the middle of moving the wave panel. If it is false, it will show the wave panel.")]
    private bool isMovingWave = false;
    [Header("If Menu is Fading")]
    [Tooltip("Checks if the menu is fading. If it's not, then it will call fadeEm.")]
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

		// 5:4
		if (Camera.main.aspect <= 1.3f)
		{
			jarDistanceOffset = 0.5f;
		}
		// 4:3
		else if (Camera.main.aspect <= 1.4f)
		{
			jarDistanceOffset = 0.5f;
		}
		// 3:2
		else if (Camera.main.aspect == 1.5f)
		{
			jarDistanceOffset = -0.5f;
		}
		// 16:10
		else if (Camera.main.aspect == 1.6f)
		{
			jarDistanceOffset = -2.2f;
		}
		// 16:9
		else if (Camera.main.aspect <= 1.8f)
		{
			jarDistanceOffset = -2.2f;
		}

        if (SceneManager.GetActiveScene().name == "Main_Mobile_DeepForest")
        {
            for (int i = 0; i < jars.Length; i++)
            {
                currentColor[i] = Color.clear;
                intensities[i] = 0.0f;
                jarsYSetAlready[i] = false;
                jarsPulseAlready[i] = false;

				float variedDistance = Mathf.Abs (jars [i].gameObject.transform.position.y - topBound.transform.position.y);

				jars[i].gameObject.transform.position = new Vector3(jars[i].gameObject.transform.position.x, topBound.transform.position.y + variedDistance + 1, jars[i].gameObject.transform.position.z);

                beginningJarPos[i] = new Vector3(jars[i].gameObject.transform.position.x, jars[i].gameObject.transform.position.y, jars[i].gameObject.transform.position.z);

                jarsY[i] = beginningJarPos[i].y;

				//SetJarYPos(i);
            }
        }
        else
        {
            jarsYSetAlready[0] = true;
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
		if (jarsYSetAlready [val] == true)
		{
			return;
		}

        if (val < jars.Length)
        {
            jarsYLerpTime = 0;
			jarsY[val] = Mathf.Abs(beginningJarPos[val].y - topBound.transform.position.y) - jarDistanceOffset;
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
			totalScoreFG.text = tempScoreCounter.ToString ();
            totalScore = score * multiplier;
        }
        else
        {
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
