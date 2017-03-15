﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    //Jar player;
    UI uiController;

    private AudioSFX aSFX;

    GameObject player;
    GameObject JarTopCollider;
    
    [SerializeField]
    GameObject fireflyPrefab;
    [SerializeField]
    GameObject fireflyLightPrefab;
    [SerializeField]
    private GameObject[] fireflies;
    [SerializeField] GameObject pauseButton;

    [SerializeField]
    GameObject dragonflyPrefab;
    //[SerializeField]
    //private GameObject[] dragonflies;

    [SerializeField]
    float[] bounds;

    [SerializeField]
    bool finishGame = false;
    [SerializeField]
    bool stopDoingThis = false;

    [SerializeField]
    int bugCounter;

    [SerializeField]
    int realAmountOfBugs;

    [SerializeField]
    int filledJars;

    int bugGoal = 10;

    int maxBugs = 10;
    int maxDragonflies = 2;

    int jarDamageLimit = 3;

    [SerializeField]
    int jarCurrentDamage = 0;

    [SerializeField]
    bool hitAlready = false;

    public static int[] bandFrequencies;

    [SerializeField]
    bool startGame = false;

    [SerializeField]
    bool startGameInEditor = false;

    // Use this for initialization
    void Start()
    {
        bounds = new float[4];
        fireflies = new GameObject[10];
        //dragonflies = new GameObject[2];
        bandFrequencies = new int[10];

        bounds[0] = GameObject.Find("Top").gameObject.transform.position.y;
        bounds[1] = GameObject.Find("Bottom").gameObject.transform.position.y;
        bounds[2] = GameObject.Find("Left").gameObject.transform.position.x;
        bounds[3] = GameObject.Find("Right").gameObject.transform.position.x;

        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Jar>();
        player = GameObject.FindGameObjectWithTag("Player");
        JarTopCollider = GameObject.FindGameObjectWithTag("JarTop");

        /*if (Application.isEditor && startGameInEditor && GameObject.Find("ObjectKeeper ORIGINAL") == null)
        {
            GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
            StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio(PlayerPrefs.GetInt("sceneNumber")));
            //StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio(1));
            GameObject.Find("Directional light").GetComponent<LightController>().SetGame();
            SetStartGame(true);
        }*/

        aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
		GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
		StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio());
		//StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio(1));
		GameObject.Find("Directional light").GetComponent<LightController>().SetGame();
		SetStartGame(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame)
        {
            InitializeBugs();
            startGame = false;
        }
    }

    void InstantiateBug()
    {
        for (int i = 0; i < fireflies.Length; i++)
        {
            if (fireflies[i] == null)
            {
                // Instantiate a new bug at a random position
                Vector2 randPos = new Vector2(Random.Range(bounds[2], bounds[3]), Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

                newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[i];
                fireflies[i] = newBug;
                //Debug.LogWarning("Firefly " + i + " with band frequency " + bandFrequencies[i]);
                return;
            }
        }
    }

    void InstantiateDragonfly()
    {
    /*
        for (int i = 0; i < dragonflies.Length; i++)
        {
            if (dragonflies[i] == null && i == 0)
            {
                // Instantiate a new bug at a random position
                Vector2 randPos = new Vector2(bounds[2] - 4.0f, Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(dragonflyPrefab, randPos, Quaternion.identity) as GameObject;

                //Debug.Log("Starting Bug: " + randPos);

                newBug.GetComponent<Dragonfly>().SetSpawnSide(false);

                dragonflies[i] = newBug;

                return;
            }
            else if (dragonflies[i] == null && i == 1)
            {
                // Instantiate a new bug at a random position
                Vector2 randPos = new Vector2(bounds[3] + 4.0f, Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(dragonflyPrefab, randPos, Quaternion.identity) as GameObject;

                newBug.GetComponent<Dragonfly>().SetSpawnSide(true);

                dragonflies[i] = newBug;

                return;
            }
        }*/
    }
    void InitializeBugs()
    {
        for (int i = 0; i < maxBugs; i++)
        {
            float randX = Random.Range(bounds[2], bounds[3]);
            float randY = Random.Range(bounds[0], bounds[1]);
            Vector3 randPos = new Vector3(randX, randY, -0.5f);
            GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

            //Assign each bug a frequency band, band range 0-5
            if (i > 5)
            {
                int tempBand = i - 6;
                newBug.GetComponentInChildren<Flicker>()._band = tempBand;
            }
            else
            {
                newBug.GetComponentInChildren<Flicker>()._band = i;
            }

            fireflies[i] = newBug;
            bandFrequencies[i] = newBug.GetComponentInChildren<Flicker>()._band;

            //Debug.Log("Firefly " + i + " with band frequency " + bandFrequencies[i]);
        }

        StartCoroutine("CheckToMakeNewFirefly");
        StartCoroutine("CheckToMakeNewDragonfly");
    }

    public void CatchBug(string bugType, int jar)
    {
        if (!stopDoingThis)
        {
            bugCounter++;
            realAmountOfBugs++;

            if (bugCounter == 10)
            {
				aSFX.playJarDrop();
                filledJars++;
                bugCounter = 0;
                uiController.SetJarYPos(filledJars);
            }

            uiController.AddBug(jar);

        }

        if (finishGame)
        {
            if (bugCounter >= bugGoal)
            {
                stopDoingThis = true;
                FinishGameTime();
                finishGame = false;
            }
        }
    }

    public void FinishGameTime()
    {
        stopDoingThis = true;
		player.GetComponent<Drag>().SetEndGame (true);
        StopAllCoroutines();
        Destroy(player.GetComponent<Jar>());
        Destroy(player.GetComponent<BoxCollider2D>());
        Destroy(JarTopCollider);
        uiController.FinishGame(filledJars);
        pauseButton.SetActive(false);
    }

    public void FinishGameDie()
    {
        stopDoingThis = true;
        player.GetComponent<Drag>().SetEndGame(true);
        StopAllCoroutines();
        Destroy(player.GetComponent<Jar>());
        Destroy(player.GetComponent<BoxCollider2D>());
        Destroy(JarTopCollider);
        uiController.setStartJarParticles(true);
        uiController.ResetGlow();
        pauseButton.SetActive(false);
    }

    public void ReleaseBug(int bugNumber)
    {
        if (!stopDoingThis)
        {
            // Remove the Bug from the UI
            uiController.RemoveBug(bugNumber);

            if (bugCounter > 0 && realAmountOfBugs > 0)
            {
                InstantiateBug();
                // decrease the counter
                bugCounter--;
                realAmountOfBugs--;
            }
        }
    }

    public int GetFilledJars()
    {
        return filledJars;
    }

    public void SetStartGame(bool val)
    {
        startGame = val;
    }


    public int GetBugCount()
    {
        return bugCounter;
    }

    public int GetAmountOfBugs()
    {
        return realAmountOfBugs;
    }

    public void CrackJar()
    {
        if (hitAlready)
        {
            return;
        }

        if (filledJars >= 5)
        {
            if (uiController.GetBugInJarColor(4).a > 0)
            {
                ReleaseBug(4);
            }
        }
        else
        {
            if (uiController.GetBugInJarColor(filledJars).a > 0)
            {
                ReleaseBug(filledJars);
            }
        }

        jarCurrentDamage++;

        if (jarCurrentDamage < jarDamageLimit)
        {
            Handheld.Vibrate();

            StartCoroutine("PlayerDragonflyCooldown");
        }

        if (jarCurrentDamage <= jarDamageLimit)
        {
            uiController.setJarImage(jarCurrentDamage);
            player.GetComponent<Drag>().FlashJar();
        }

        if (jarCurrentDamage == jarDamageLimit)
        {
            FinishGameDie();
        }
    }

    IEnumerator CheckToMakeNewFirefly()
    {
        if (!stopDoingThis)
        {
            int seconds = Random.Range(1, 4);

            InstantiateBug();

            //Debug.Log("Went to CheckToMakeNewBug, seconds: " + seconds);

            yield return new WaitForSecondsRealtime(seconds);

            StartCoroutine("CheckToMakeNewFirefly");
        }
    }

    IEnumerator CheckToMakeNewDragonfly()
    {
        if (!stopDoingThis)
        {
            int seconds = Random.Range(5, 10);

            yield return new WaitForSecondsRealtime(seconds);

            InstantiateDragonfly();

            StartCoroutine("CheckToMakeNewDragonfly");
        }
    }

    IEnumerator PlayerDragonflyCooldown()
    {
        hitAlready = true;

        StartCoroutine(player.GetComponent<Drag>().FlashJar());

        yield return new WaitForSecondsRealtime(2.5f);

        hitAlready = false;
    }

    public int WhichJar()
    {
        if (filledJars < 1)
        {
            return 0;
        }
        else if (filledJars < 2)
        {
            return 1;
        }
        else if (filledJars < 3)
        {
            return 2;
        }
        else if (filledJars < 4)
        {
            return 3;
        }
        else if (filledJars < 5)
        {
            return 4;
        }
        else
        {
            return Random.Range(0,4);
        }
    }
}
