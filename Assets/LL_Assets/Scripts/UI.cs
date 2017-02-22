using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {
    public Text scoreTextFG;
    public Text totalScoreFG;
    public Image FGOverlay;
    public GameObject exitButtonFG;

    public ParticleSystem[] crackedJarFireflies;

    [SerializeField]
    public SpriteRenderer[] glows;
    [SerializeField]
    public SpriteRenderer[] jars;

    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    [SerializeField]
    GameController gc;

    [SerializeField]
    GameObject fireflyUI;

    [SerializeField]
    private Color32 currentColor = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 previousColor0 = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 previousColor1 = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 previousColor2 = new Color32(255, 255, 255, 0);


    [SerializeField]
    float fireflyColorConvert = 0;

    [SerializeField]
    byte fireflyTransparency = 0;

    float lerpColorTime = 0;

    [SerializeField]
    int score = 0;
    [SerializeField]
    int totalScore = 0;
    [SerializeField]
    int tempScoreCounter = 0;
    [SerializeField]
    bool startJarParticles = false;

    [SerializeField]
    bool hasTouchedAtEnd = false;

    // Use this for initialization
    void Start () {
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        
    }

    // Update is called once per frame
    void Update () {
        glows[0].color = Color32.Lerp(previousColor0, currentColor, (lerpColorTime += Time.deltaTime) / 1f);
        glows[1].color = Color32.Lerp(previousColor1, currentColor, (lerpColorTime += Time.deltaTime) / 1f);
        glows[2].color = Color32.Lerp(previousColor2, currentColor, (lerpColorTime += Time.deltaTime) / 1f);

        if (startJarParticles && glows[0].color == currentColor && glows[1].color == currentColor && glows[2].color == currentColor)
        {
            FinishGame(gc.GetFilledJars());
        }

        //Debug.Log(crackedJarFireflies.isPlaying);

        /*if (Input.GetKeyDown(KeyCode.J))
        {
            FinishGame(gc.GetFilledJars());
        }*/

        /*if (Input.GetKeyDown(KeyCode.K))
        {
            AddBug();
        }*/

        if (Input.GetKeyDown(KeyCode.L))
        {
            //ResetGlow();

            if (!hasTouchedAtEnd)
            {
                Debug.Log("Touched at end using key");
                hasTouchedAtEnd = true;
            }
        }
    }

    public void setStartJarParticles(bool val)
    {
        startJarParticles = val;
    }

    // get rid of the scaling when u have the final artwork for the cracked jars (with the strings holding it up)
    public void setJarImage(int val)
    {
        jars[0].gameObject.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        jars[1].gameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        jars[2].gameObject.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);

        for (int i = 0; i < jars.Length; i++)
        {
            jars[i].sprite = jarImages[val];
            glows[i].transform.localScale = new Vector3(4, 4, 4);
        }
    }

    public Color GetBugInJarColor(int bugNumber)
    {
        return glows[bugNumber].color;
    }

    public void ResetGlow()
    {
        lerpColorTime = 0;

        fireflyColorConvert = 0;

        fireflyTransparency = (byte)fireflyColorConvert;

        previousColor0 = glows[0].color;
        previousColor1 = glows[1].color;
        previousColor2 = glows[2].color;

        currentColor = new Color32(255, 255, 255, fireflyTransparency);

        if (startJarParticles)
        {
            crackedJarFireflies[0].Play();
            crackedJarFireflies[1].Play();
            crackedJarFireflies[2].Play();
        }
    }

    public void AddBug(int bugNumber) {
        lerpColorTime = 0;
        score += 10;

        if (fireflyColorConvert >= 0 && fireflyColorConvert < 255)
        {
            fireflyColorConvert += 25.5f;
        }

        //Mathf.Clamp(fireflyColorConvert, 0, 255);

        if (fireflyColorConvert > 255)
        {
            fireflyColorConvert = 255;
        }

        fireflyTransparency = (byte)fireflyColorConvert;

        if (bugNumber == 0)
        {
            previousColor0 = glows[0].color;
        } 
        else if (bugNumber == 1)
        {
            previousColor1 = glows[1].color;
        } 
        else if (bugNumber == 2)
        {
            previousColor2 = glows[2].color;
        }
        
        currentColor = new Color32(255, 255, 255, fireflyTransparency);
    }

    public void RemoveBug(int bugNumber)
    {
        lerpColorTime = 0;

        if (fireflyColorConvert > 0)
        {
            fireflyColorConvert -= 25.5f;
        }

        //Mathf.Clamp(fireflyColorConvert, 0, 255);

        if (fireflyColorConvert < 0)
        {
            fireflyColorConvert = 0;
        }

        fireflyTransparency = (byte)fireflyColorConvert;

        if (bugNumber == 0)
        {
            previousColor0 = glows[0].color;
        } else if (bugNumber == 1)
        {
            previousColor1 = glows[1].color;
        } else if (bugNumber == 2)
        {
            previousColor2 = glows[2].color;
        }


        currentColor = new Color32(255, 255, 255, fireflyTransparency);
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
}
