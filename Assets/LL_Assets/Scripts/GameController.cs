﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{

    //Jar player;
    UI uiController;

    GameObject player;

    [SerializeField]
    GameObject fireflyPrefab;
    [SerializeField]
    GameObject fireflyLightPrefab;
    [SerializeField]
    private GameObject[] fireflies;

    [SerializeField]
    GameObject dragonflyPrefab;
    [SerializeField]
    private GameObject[] dragonflies;

    [SerializeField]
    float[] bounds;

    [SerializeField]
    bool finishGame = false;
    [SerializeField]
    bool stopDoingThis = false;

    [SerializeField]
    int bugCounter;
    [SerializeField]
    int filledJars;

    int bugGoal = 10;

    int maxBugs = 10;
    int maxDragonflies = 2;

    int jarDamageLimit = 3;

    [SerializeField]
    int jarCurrentDamage = 0;

    bool hitAlready = false;

    public static int[] bandFrequencies;

    // Use this for initialization
    void Start()
    {
        bounds = new float[4];
        fireflies = new GameObject[10];
        dragonflies = new GameObject[2];
        bandFrequencies = new int[10];

        bounds[0] = GameObject.Find("Top").gameObject.transform.position.y;
        bounds[1] = GameObject.Find("Bottom").gameObject.transform.position.y;
        bounds[2] = GameObject.Find("Left").gameObject.transform.position.x;
        bounds[3] = GameObject.Find("Right").gameObject.transform.position.x;

        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Jar>();
        player = GameObject.FindGameObjectWithTag("Player");
        InitializeBugs();
    }

    // Update is called once per frame
    void Update()
    {
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
        /*int decideIfMakeNewDragonfly = Random.Range(0, 1);

        if (decideIfMakeNewDragonfly == 0)
        {
            return;
        }*/

        for (int i = 0; i < dragonflies.Length; i++)
        {
            if (dragonflies[i] == null && i == 0)
            {
                // Instantiate a new bug at a random position
                Vector2 randPos = new Vector2(bounds[2] - 4.0f, Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(dragonflyPrefab, randPos, Quaternion.identity) as GameObject;

                newBug.GetComponent<Dragonfly>().SetSpawnSide(true);

                dragonflies[i] = newBug;

                return;
            }
            else if (dragonflies[i] == null && i == 1)
            {
                // Instantiate a new bug at a random position
                Vector2 randPos = new Vector2(bounds[3] + 4.0f, Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(dragonflyPrefab, randPos, Quaternion.identity) as GameObject;

                newBug.GetComponent<Dragonfly>().SetSpawnSide(false);

                dragonflies[i] = newBug;

                return;
            }
        }
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

        StartCoroutine("CheckToMakeNewBug");
    }

    public void CatchBug(string bugType)
    {
        if (!stopDoingThis)
        {
            bugCounter++;

            if (bugCounter == 10)
            {
                filledJars++;
                bugCounter = 0;
                uiController.ResetGlow();
            }

            uiController.AddBug();
        }

        if (finishGame)
        {
            if (bugCounter >= bugGoal)
            {
                stopDoingThis = true;
                StopAllCoroutines();
                Destroy(player);
                Destroy(GameObject.Find("Top Collider").gameObject);
                uiController.FinishGame(filledJars);
                finishGame = false;
            }
        }
    }

    public void ReleaseBug()
    {
        if (!stopDoingThis)
        {
            // Remove the Bug from the UI
            uiController.RemoveBug();

            if (bugCounter > 0)
            {
                InstantiateBug();
                // decrease the counter
                bugCounter--;
            }
        }
    }

    public int GetFilledJars()
    {
        return filledJars;
    }

    public int GetBugCount()
    {
        return bugCounter;
    }

    public void CrackJar()
    {
        if (hitAlready)
        {
            return;
        }

        StartCoroutine("PlayerDragonflyCooldown");

        if (bugCounter > 0)
        {
            bugCounter--;
        }

        jarCurrentDamage++;

        if (jarCurrentDamage == jarDamageLimit)
        {
            Destroy(player.GetComponent<Jar>());
            uiController.setStartJarParticles(true);
            uiController.ResetGlow();
        }
    }

    IEnumerator CheckToMakeNewBug()
    {
        if (!stopDoingThis)
        {
            int seconds = Random.Range(1, 10);

            InstantiateBug();
            InstantiateDragonfly();

            //Debug.Log("Went to CheckToMakeNewBug, seconds: " + seconds);

            yield return new WaitForSecondsRealtime(seconds);

            StartCoroutine("CheckToMakeNewBug");
        }
    }

    IEnumerator PlayerDragonflyCooldown()
    {
        hitAlready = true;

        yield return new WaitForSecondsRealtime(2.0f);

        hitAlready = false;
    }
}
