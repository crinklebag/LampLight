using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
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

    [SerializeField] bool finishGame = false;
    [SerializeField] bool stopDoingThis = false;

    [SerializeField] int bugCounter;
    [SerializeField] int realAmountOfBugs;
    [SerializeField] int filledJars;

    int bugGoal = 10;
    int maxBugs = 10;
    int maxDragonflies = 2;
    int jarDamageLimit = 3;

    int bugCount = 0;
    int maxBugCount = 30;

    [SerializeField] int jarCurrentDamage = 0;
    [SerializeField] bool hitAlready = false;
    public static int[] bandFrequencies;

    [SerializeField] bool startGame = false;
    [SerializeField] bool startGameInEditor = false;


    [SerializeField] Transform[] spawnPoints;//For the fireflies
    private int spawnIndex = 0;//Which spawn point are we using

    [SerializeField] GameObject[] boundsColliders;

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

        aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
		GameObject.Find("AudioManager").gameObject.GetComponent<AudioSource>().Stop();
		StartCoroutine(GameObject.Find("AudioManager").gameObject.GetComponent<AudioManager>().StartAudio());
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
        for(int j = 0; j < 10; j++) {
            // Debug.Log("Making Bug # " + j);
            // Instantiate a new bug at a random position
            //Vector2 randPos = new Vector2(Random.Range(bounds[2], bounds[3]), Random.Range(bounds[0], bounds[1]));
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject; //Instantitate at random spawn point

			newBug.GetComponent<FireFly>().startFireflyLife(boundsColliders);
            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            bugCount++;
        }
    }

    IEnumerator InstantiateNewBugs() {
        for (int j = 0; j < 10; j++)
        {
            //Debug.Log("Making Bug # " + j);
            // Instantiate a new bug at a random position
            //Vector2 randPos = new Vector2(Random.Range(bounds[2], bounds[3]), Random.Range(bounds[0], bounds[1]));
			spawnIndex = Random.Range(0, spawnPoints.Length);
			GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

			newBug.GetComponent<FireFly>().startFireflyLife(boundsColliders);
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
            float randX = Random.Range(bounds[2], bounds[3]);
            float randY = Random.Range(bounds[0], bounds[1]);
            //Vector3 randPos = new Vector3(randX, randY, -0.5f);
			spawnIndex = Random.Range(0, spawnPoints.Length);
			GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

			newBug.GetComponent<FireFly>().startFireflyLife(boundsColliders);

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

    public void CatchBug(string bugType, int jar)
    {
        if (!stopDoingThis)
        {
            bugCounter++;
            realAmountOfBugs++;
            bugCount--;

            if (bugCounter == 30) {

                if (filledJars < 5)
                {
					aSFX.playJarDrop();
                    filledJars++;
                }

                bugCounter = 0;
                uiController.SetJarYPos(filledJars);
            }

            uiController.AddBug(jar);
        }
    }

    public void FinishGameTime()
    {
        stopDoingThis = true;
		player.GetComponent<Drag>().SetEndGame (true);
        StopAllCoroutines();

        //show end game overlay and start end game audio
		uiController.showFGOverlay();
		StartCoroutine(GameObject.Find("AudioManager").GetComponent<AudioManager>().EndGame());//Perform audio tasks at end of game (fade out current audio, fade in end game audio)

        Destroy(player.GetComponent<Jar>());
        Destroy(player.GetComponent<BoxCollider2D>());
        Destroy(JarTopCollider);
        //uiController.FinishGame(filledJars);
		uiController.setStartJarParticles(true);
        pauseButton.SetActive(false);
    }

    public void FinishGameDie()
    {

        stopDoingThis = true;
        player.GetComponent<Drag>().SetEndGame(true);
        StopAllCoroutines();

		//show end game overlay and start end game audio
		uiController.showFGOverlay();
		StartCoroutine(GameObject.Find("AudioManager").GetComponent<AudioManager>().EndGame());//Perform audio tasks at end of game (fade out current audio, fade in end game audio)

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
                //AE - ReleaseBug(4);
            }
        }
        else
        {
            if (uiController.GetBugInJarColor(filledJars).a > 0)
            {
                //AE - ReleaseBug(filledJars);
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
            if (bugCount < maxBugCount) {

                // InstantiateBug();
                StartCoroutine(InstantiateNewBugs());
            }

            //Debug.Log("Went to CheckToMakeNewBug, seconds: " + seconds);
            int seconds = Random.Range(10, 20);
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

    public void makeLotsOfBugs()
    {
		for(int j = 0; j < 10; j++)
		{
            spawnIndex = Random.Range(0, spawnPoints.Length);
            GameObject newBug = Instantiate(fireflyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject; //Instantitate at random spawn point

			newBug.GetComponent<FireFly>().startFireflyLife(boundsColliders);
            newBug.GetComponentInChildren<Flicker>()._band = bandFrequencies[j];
            bugCount++;
        }
    }

    //Call on exit button press
    //fade audio back out, change to fun facts loading scene
    public void ReturnToMenu()
    {
    	SceneManager.LoadScene("LoadingFacts");

    	return;
    }
}
