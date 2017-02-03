using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    public Text multiplierText;
    public Text scoreText;

    public Text scoreTextFG;
    public Text totalScoreFG;
    public Image FGOverlay;
    public GameObject exitButtonFG;

    public ParticleSystem crackedJarFireflies;
    public Image jar;
    public Sprite[] jarImages; // 0 = not cracked, 1 = a little crack, 2 = halfway, 3 = broken

    [SerializeField]
    GameController gc;

    [SerializeField]
    GameObject fireflyUI;

    [SerializeField]
    private Color32 currentColor = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 previousColor = new Color32(255, 255, 255, 0);

    [SerializeField]
    float fireflyColorConvert = 0;

    [SerializeField]
    byte fireflyTransparency = 0;

    float lerpColorTime = 0;

    Image fireflyUIImage;

    [SerializeField]
    int score = 0;
    [SerializeField]
    int totalScore = 0;
    [SerializeField]
    int tempScoreCounter = 0;
    [SerializeField]
    bool startJarParticles = false;

    // Use this for initialization
    void Start () {
        fireflyUIImage = fireflyUI.gameObject.GetComponent<Image>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        crackedJarFireflies = GameObject.Find("CrackedJarFireflies").GetComponent<ParticleSystem>();
        jar = GameObject.Find("Jar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update () {
        multiplierText.text = "x " + gc.GetFilledJars().ToString();
        scoreText.text = score.ToString();

        fireflyUIImage.color = Color32.Lerp(previousColor, currentColor, (lerpColorTime += Time.deltaTime) / 1f);

        if (startJarParticles && fireflyUIImage.color == currentColor)
        {
            FinishGame(gc.GetFilledJars());
        }

        Debug.Log(crackedJarFireflies.isPlaying);

        /*if (Input.GetKeyDown(KeyCode.J))
        {
            RemoveBug();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddBug();
        }*/

        if (Input.GetKeyDown(KeyCode.L))
        {
            ResetGlow();
        }
    }

    public void setStartJarParticles(bool val)
    {
        startJarParticles = val;
    }

    public void setJarImage(int val)
    {
        jar.sprite = jarImages[val];
    }

    public void ResetGlow()
    {
        lerpColorTime = 0;

        fireflyColorConvert = 0;

        fireflyTransparency = (byte)fireflyColorConvert;

        previousColor = fireflyUIImage.color;
        currentColor = new Color32(255, 255, 255, fireflyTransparency);

        if (startJarParticles)
        {
            crackedJarFireflies.Play();
        }
    }

    public void AddBug() {
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

        previousColor = fireflyUIImage.color;
        currentColor = new Color32(255, 255, 255, fireflyTransparency);
    }

    public void RemoveBug()
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

        previousColor = fireflyUIImage.color;
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

        if (tempScoreCounter == totalScore)
        {
            exitButtonFG.SetActive(true);
        }

        StartCoroutine("CountUpScore");
    }
}
