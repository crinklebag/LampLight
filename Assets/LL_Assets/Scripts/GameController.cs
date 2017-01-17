using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

    Jar player;
    UI uiController;

    [SerializeField]
    GameObject fireflyPrefab;
    [SerializeField]
    GameObject fireflyLightPrefab;

    int bugGoal = 10;
    int bugCounter;

    [SerializeField]
    float[] bounds;

    [SerializeField]
    private GameObject[] fireflies;

    public static int[] bandFrequencies;

    // Use this for initialization
    void Start()
    {
        bounds = new float[4];
        fireflies = new GameObject[10];
        bandFrequencies = new int[10];

        bounds[0] = GameObject.Find("Top").gameObject.transform.position.y;
        bounds[1] = GameObject.Find("Bottom").gameObject.transform.position.y;
        bounds[2] = GameObject.Find("Left").gameObject.transform.position.x;
        bounds[3] = GameObject.Find("Right").gameObject.transform.position.x;

        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Jar>();
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
                // Intsantiate a new bug at a random position
                Vector2 randPos = new Vector2(Random.Range(bounds[2], bounds[3]), Random.Range(bounds[0], bounds[1]));
                GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

                newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[i];
                fireflies[i] = newBug;
                Debug.LogWarning("Firefly " + i + " with band frequency " + bandFrequencies[i]);
                return;
            }
        }
    }

    void InitializeBugs()
    {
        int count = uiController.GetBugCount();

        for (int i = 0; i < count; i++)
        {
            float randX = Random.Range(bounds[2], bounds[3]);
            float randY = Random.Range(bounds[0], bounds[1]);
            Vector3 randPos = new Vector3(randX, randY, -0.5f);
            GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

            //Assign each bug a frequency band,band range 0-5
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
    }

    public void CatchBug(string bugType)
    {
        bugCounter++;
        uiController.AddBug();

        if (bugCounter == bugGoal)
        {
            Debug.Log("End Game");
        }
    }

    public void ReleaseBug()
    {
        if (bugCounter > 0)
        {
            // Remove the Bug from the UI
            uiController.RemoveBug();
            InstantiateBug();
            // decrease the counter
            bugCounter--;
        }
    }

    public int GetBugCount()
    {
        return bugCounter;
    }
}
