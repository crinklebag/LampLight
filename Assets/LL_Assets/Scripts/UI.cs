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

    [SerializeField]
    GameController gc;

    [SerializeField]
    GameObject fireflyUI;

    [SerializeField]
    private Color32 currentColor = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 previousColor = new Color32(255, 255, 255, 0);

    [SerializeField]
    int fireflyColorConvert = 0;

    [SerializeField]
    byte fireflyTransparency = 0;

    float lerpColorTime = 0;

    Image fireflyUIImage;

    int score = 0;
    int totalScore = 0;
    int tempScoreCounter = 0;

    // Use this for initialization
    void Start () {
        fireflyUIImage = fireflyUI.gameObject.GetComponent<Image>();
        gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update () {
        multiplierText.text = "x " + gc.GetBugCount().ToString();
        scoreText.text = score.ToString();

        fireflyUIImage.color = Color32.Lerp(previousColor, currentColor, (lerpColorTime += Time.deltaTime) / 1f);
    }

    public void AddBug() {
        lerpColorTime = 0;
        score += 10;

        if (fireflyColorConvert >= 0 && fireflyColorConvert < 255)
        {
            fireflyColorConvert += 10;
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
        if (fireflyColorConvert > 0)
        {
            fireflyColorConvert -= 10;
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

        totalScore = score * multiplier;

        StartCoroutine("CountUpScore");
    }

    IEnumerator CountUpScore()
    {
        yield return new WaitForSecondsRealtime(0.001f);
        //yield return new WaitForFixedUpdate();
        //yield return new WaitForEndOfFrame();

        tempScoreCounter++;

        totalScoreFG.text = tempScoreCounter.ToString();

        if (tempScoreCounter < totalScore)
        {
            StartCoroutine("CountUpScore");
        }

        if (tempScoreCounter == totalScore)
        {
            exitButtonFG.SetActive(true);
        }
    }
}
