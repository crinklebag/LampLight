using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

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

    public Text scoreTextFG;
    public Text totalScoreFG;
    public Image FGOverlay;
    public GameObject exitButtonFG;

    public Image progressBar;

    public ParticleSystem[] crackedJarFireflies;

    [SerializeField] public SpriteRenderer[] glows;
    [SerializeField] public SpriteRenderer[] jars;

    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken
    //public Sprite[] jarImagesMultiplier; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    public GameObject[] brokenHalfJars;
    public GameObject am;

    [SerializeField] GameController gc;

    //[SerializeField] GameObject fireflyUI;

    //[SerializeField] private Color32 currentColorMultiplier;

    [SerializeField] private Color32[] currentColor;
    [SerializeField] private Color32[] previousColor;

    [SerializeField] float[] fireflyColorConvert;
    [SerializeField] float[] jarsY;
    [SerializeField] float lerpColorTime = 0;
    [SerializeField] float fireflyColorConvertUI;
    float jarsYLerpTime = 0;

    [SerializeField] int score = 0;
    [SerializeField] int totalScore = 0;
    [SerializeField] int tempScoreCounter = 0;

    [SerializeField] bool startJarParticles = false;
    [SerializeField] bool hasTouchedAtEnd = false;
    [SerializeField] bool[] jarsYSetAlready;

    [SerializeField] bool[] jarsPulseAlready;

    void Awake()
    {
        previousColor = new Color32[jars.Length];
        currentColor = new Color32[jars.Length];
        jarsY = new float[jars.Length];
        fireflyColorConvert = new float[jars.Length];
        jarsYSetAlready = new bool[jars.Length];
        jarsPulseAlready = new bool[jars.Length];

        for (int i = 0; i < jars.Length; i++)
        {
            jars[i].gameObject.transform.position = new Vector3(jars[i].gameObject.transform.position.x, jars[i].gameObject.transform.position.y + 4.0f, jars[i].gameObject.transform.position.z);

            previousColor[i] = glows[i].color;
            currentColor[i] = Color.clear;
            jarsY[i] = jars[i].gameObject.transform.position.y;
            jarsYSetAlready[i] = false;
            jarsPulseAlready[i] = false;
        }

        theState = IngameMenuStates.PLAY;

        //currentColorMultiplier = Color.clear;
    }

    // Use this for initialization
    void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        SetJarYPos(0);
    }

    // Update is called once per frame
    void Update () {

        lerpColorTime = (lerpColorTime += Time.deltaTime) / 1f;

        //fireflyJarMultiplier.color = Color32.Lerp(fireflyJarMultiplier.color, currentColorMultiplier, lerpColorTime);

        /*if (fireflyJarMultiplier.color == Color.white)
        {
            lerpColorTime = 0;
            fireflyColorConvertUI = 0;
            currentColorMultiplier = Color.clear;
        }*/

        for (int i = 0; i < glows.Length; i++)
        {
            glows[i].color = Color32.Lerp(previousColor[i], currentColor[i], lerpColorTime);

            jars[i].gameObject.transform.position = new Vector3(jars[i].gameObject.transform.position.x, Mathf.Lerp(jars[i].gameObject.transform.position.y, jarsY[i], jarsYLerpTime += (Time.deltaTime * 0.01f)), jars[i].gameObject.transform.position.z);

            if (jarsYSetAlready[i] && !jarsPulseAlready[i] && jars[i].gameObject.transform.position.y <= jarsY[i] + 0.1f)
            {
                jars[i].gameObject.GetComponent<JarPulse>().SetPulse(true);
                jarsPulseAlready[i] = true;
            }
        }

        if (startJarParticles && glows[0].color == currentColor[0] && glows[1].color == currentColor[1] && glows[2].color == currentColor[2] && glows[3].color == currentColor[3] && glows[4].color == currentColor[4])
        {
            FinishGame(gc.GetFilledJars());
        }

        //Debug.Log(crackedJarFireflies.isPlaying);

        /*if (Input.GetKeyDown(KeyCode.J))
        {
            FinishGame(gc.GetFilledJars());
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddBug(1);
        }*/

        progressBar.fillAmount = am.GetComponent<AudioSource>().time / am.GetComponent<AudioSource>().clip.length;

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
        //multiplierText.text = "x " + gc.GetFilledJars();

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

    public void SetJarYPos(int val)
    {
        if (val < jars.Length)
        {
            jarsYLerpTime = 0;
            jarsY[val] = jars[val].gameObject.transform.position.y - 5.0f;
            jarsYSetAlready[val] = true;
        }
    }

    public void setStartJarParticles(bool val)
    {
        startJarParticles = val;
    }

    // get rid of the scaling when u have the final artwork for the cracked jars (with the strings holding it up)
    public void setJarImage(int val)
    {
        /*jars[0].gameObject.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        jars[1].gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        jars[2].gameObject.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
        jars[3].gameObject.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        jars[4].gameObject.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);*/

        //uiJarMultiplier.sprite = jarImagesMultiplier[val];

        for (int i = 0; i < jars.Length; i++)
        {
            jars[i].sprite = jarImages[val];
            //glows[i].transform.localScale = new Vector3(4, 4, 4);
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

        for (int i = 0; i < previousColor.Length; i++)
        {
            if (jarsYSetAlready[i] == true)
            {
                fireflyColorConvert[i] = 0;
                previousColor[i] = glows[i].color;
                currentColor[i] = new Color32(255,255,255, (byte)fireflyColorConvert[i]);

                brokenHalfJars[i].SetActive(true);
                brokenHalfJars[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), Random.Range(-30, -10)));

                crackedJarFireflies[i].Play();
            }
        }
    }
    
    public void AddBug(int bugNumber) {
        lerpColorTime = 0;
        score += 10;

        if (fireflyColorConvert[bugNumber] >= 0 && fireflyColorConvert[bugNumber] < 255)
        {
            fireflyColorConvert[bugNumber] += 25.5f;
        }

        //Mathf.Clamp(fireflyColorConvert, 0, 255);

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

        previousColor[bugNumber] = glows[bugNumber].color;
        
        currentColor[bugNumber] = new Color32(255, 255, 255,(byte)fireflyColorConvert[bugNumber]);

        //currentColorMultiplier = new Color32(255, 255, 255, (byte)fireflyColorConvertUI);

        jars[bugNumber].GetComponent<JarPulse>().SetPulse(true);
    }

    public void RemoveBug(int bugNumber)
    {
        lerpColorTime = 0;

        if (fireflyColorConvert[bugNumber] > 0)
        {
            fireflyColorConvert[bugNumber] -= 25.5f;
        }

        //Mathf.Clamp(fireflyColorConvert, 0, 255);

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

        previousColor[bugNumber] = glows[bugNumber].color;
        
        currentColor[bugNumber] = new Color32(255, 255, 255, (byte)fireflyColorConvert[bugNumber]);

        //currentColorMultiplier = new Color32(255, 255, 255, (byte)fireflyColorConvertUI);
    }

    public void FinishGame(int multiplier)
    {
        scoreTextFG.text = score.ToString() + " x " + multiplier.ToString();
        totalScoreFG.text = tempScoreCounter.ToString();

        FGOverlay.gameObject.SetActive(true);

        if (multiplier > 0)
        {
            totalScore = score * multiplier;
        }
        else
        {
            totalScore = score;
        }
        

        StartCoroutine("CountUpScore");
    }

    public void SetHasTouchedAtEnd(bool val)
    {
        hasTouchedAtEnd = val;
    }

    IEnumerator CountUpScore()
    {
        yield return new WaitForSecondsRealtime(0.001f);
        //yield return new WaitForFixedUpdate();
        //yield return new WaitForEndOfFrame();

        totalScoreFG.text = tempScoreCounter.ToString();

        if (tempScoreCounter < totalScore)
        {
            tempScoreCounter++;
        }

        if (tempScoreCounter == totalScore || hasTouchedAtEnd)
        {
            tempScoreCounter = totalScore;
            exitButtonFG.SetActive(true);
        }

        StartCoroutine("CountUpScore");
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
}
