using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public int FilledJars { get { return filledJars; } set { filledJars = value; } }

    public float[] Bounds { get { return bounds; } }

    public Texture[] netStates;
    public SkinnedMeshRenderer net;

    //Jar player;
    UI uiController;

    private AudioSFX aSFX;

    GameObject player;
    GameObject JarTopCollider;
    [SerializeField] GameObject fireflyPrefab;
    [SerializeField] GameObject fireflyLightPrefab;
    // [SerializeField] private GameObject[] fireflies;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject dragonflyPrefab;

    [SerializeField] float[] bounds;

    public static int[] bandFrequencies;
    [SerializeField] int realAmountOfBugs;
    [SerializeField] int filledJars;
    int maxBugs = 10;
    int maxDragonflies = 2;
    int jarDamageLimit = 3;
    int bugCount = 0;
    int maxBugCount = 30;
    [SerializeField] int jarCurrentDamage = 0;
    
    [SerializeField] bool hitAlready = false;
    [SerializeField] bool startGameInEditor = false;
    [SerializeField] bool stopDoingThis = false;

    // Use this for initialization
    void Start()
    {
        bounds = new float[4];
        // fireflies = new GameObject[10];
        bandFrequencies = new int[10];

        bounds[0] = GameObject.Find("Top").gameObject.transform.position.y;
        bounds[1] = GameObject.Find("Bottom").gameObject.transform.position.y;
        bounds[2] = GameObject.Find("Left").gameObject.transform.position.x;
        bounds[3] = GameObject.Find("Right").gameObject.transform.position.x;

        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        player = GameObject.FindGameObjectWithTag("Player");
        JarTopCollider = GameObject.FindGameObjectWithTag("JarTop");

        player.GetComponent<Jar>().enabled = false;
        player.GetComponent<Drag>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGameAfterCountdown()
    {
        if (Application.isEditor)
        {
            PlayerPrefs.SetString("sceneNumber", "Dream Giver");
        }

        player.GetComponent<Jar>().enabled = true;
        player.GetComponent<Drag>().enabled = true;

        aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();

		GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
		StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio());

		GameObject.Find("Directional light").GetComponent<LightController>().SetGame();

        InitializeBugs();
    }

    void InstantiateBug()
    {
        for(int j = 0; j < maxBugs; j++) {
            // Debug.Log("Making Bug # " + j);
            // Instantiate a new bug at a random position
            Vector2 randPos = new Vector2(Random.Range(bounds[2] + bounds[2], bounds[3] * 2), Random.Range(bounds[0] + bounds[0], bounds[1] * 2));
            GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            bugCount++;
        }
    }

    IEnumerator InstantiateNewBugs() {
        for (int j = 0; j < maxBugs; j++)
        {
            //Debug.Log("Making Bug # " + j);
            // Instantiate a new bug at a random position
            Vector2 randPos = new Vector2(Random.Range(bounds[2] + bounds[2], bounds[3] * 2), Random.Range(bounds[0] + bounds[0], bounds[1] * 2));
            GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;

            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            bugCount++;

            float randWaitTime = Random.Range(0.4f, 1.5f);
            yield return new WaitForSeconds(randWaitTime);
        }
    }

    void InitializeBugs()
    {
        for (int i = 0; i < maxBugs; i++)
        {
            float randX = Random.Range(bounds[2] - bounds[2], bounds[3] * 2);
            float randY = Random.Range(bounds[0] - bounds[0], bounds[1] * 2);
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

            bandFrequencies[i] = newBug.GetComponentInChildren<Flicker>()._band;

            //Debug.Log("Firefly " + i + " with band frequency " + bandFrequencies[i]);
        }

        StartCoroutine("CheckToMakeNewFirefly");
    }

    public void CatchBug(int jar)
    {
        if (!stopDoingThis)
        {
            realAmountOfBugs++;
            bugCount--;

            if (uiController.TimesFireflyWentHere[jar] == 10)
            {
				aSFX.playJarDrop();
            }

            uiController.AddBug(jar);
        }
    }

    public void FinishGame()
    {
        stopDoingThis = true;
        player.GetComponent<Drag>().SetEndGame(true);
        StopAllCoroutines();
        Destroy(player.GetComponent<Jar>());
        Destroy(player.GetComponent<BoxCollider2D>());
        Destroy(JarTopCollider);

        if (jarCurrentDamage == jarDamageLimit)
        {
            uiController.ResetGlow();
        }
        
        uiController.WinGame = true;
        pauseButton.SetActive(false);
    }

    public void ReleaseBug(int bugNumber)
    {
        if (!stopDoingThis)
        {
            // Remove the Bug from the UI
            uiController.RemoveBug(bugNumber);

            if (uiController.TimesFireflyWentHere[bugNumber] > 0)
            {
                InstantiateBug();
                // decrease the counter
                realAmountOfBugs--;
            }
        }
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

        if (uiController.GetBugInJarColor(filledJars % 5).a > 0)
        {
            ReleaseBug(filledJars % 5);
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
            net.material.SetTexture("_MainTex",  netStates[jarCurrentDamage]);
            player.GetComponent<Drag>().FlashJar();
        }


        if (jarCurrentDamage == jarDamageLimit)
        {
            FinishGame();
        }
    }

    IEnumerator CheckToMakeNewFirefly()
    {
        if (!stopDoingThis)
        {
            if (bugCount < maxBugCount) {

                // InstantiateBug();
                StartCoroutine(InstantiateNewBugs());
            }

            //Debug.Log("Went to CheckToMakeNewBug, seconds: " + seconds);
            int seconds = Random.Range(5, 10);
            yield return new WaitForSecondsRealtime(seconds);

            StartCoroutine("CheckToMakeNewFirefly");
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
        return filledJars % 5;
    }
}
