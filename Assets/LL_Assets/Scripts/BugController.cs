using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : MonoBehaviour {

	GameController gameController;
	UI uiController;

    //private List<GameObject> bugPool;//object pool
	//[SerializeField] int bugCount = 8;//pool size
	[SerializeField] GameObject bug;//firefly prefab
	//private int indexToUse = 0;//bug to enable
	public int bugsCaught = 0;

	[SerializeField] int bugsAllowed = 3;//Allow only this many bugs per wave
	public int enabledBugCount = 0;//amonut of currently enabled bugs

	[SerializeField] float instMinTime = 5.0f;//min instantiate time
	[SerializeField] float instMaxTime = 12.0f;//max instantiate time
	private float instantiateTime = 5.0f;//the instantiate time

	private float countTime;//counter to check for instatiation
	//private bool canEnable = true;//check if the previous bug has finised being enabled

	//[SerializeField] Transform[] spawnPoints;//points to spawn the bugs at
	private int spawnIndex = 0;//which point to spawn at

	private bool isPaused = false;//for spawn counter

	[SerializeField] Transform[] spawnPoints;//For the fireflies

	void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
		instantiateTime = 10.0f;
	}


	void Update ()
	{
		isPaused = gameController.gameObject.GetComponent<PauseController>().isPaused;

		if(!isPaused)
		{
			counter ();
			checkIfAllCaught();

			if (countTime >= instantiateTime)
			{
				reset();

				if(enabledBugCount < bugsAllowed)
				{
					spawnBug();
				}
			}
		}
	}

	//Reset timeCount and pass new time to instantiate time
	void reset()
	{
		countTime = 0;
		instantiateTime = Random.Range (instMinTime, instMaxTime);
	}

	//Keep track of time
	void counter ()
	{
		countTime += Time.deltaTime;
	}

	public void startWaveCoroutine()
	{
		StartCoroutine(startWave());
	}

	IEnumerator startWave()
	{
		uiController.incWaveCount();

		//reset
		bugsCaught = 0;
		enabledBugCount = 0;

		//set bugs allowed
		bugsAllowed += 5;

		Debug.Log("Wave " + uiController.getWaveCount() + " Start\n" + bugsAllowed + " bugs");

		//spawn 3/4 of the wave right away
		for(int i = 0; i < bugsAllowed * 0.75f; i++)
		{
			spawnIndex = Random.Range(0, spawnPoints.Length);
			GameObject newBug = Instantiate(bug, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

			//Assign each bug a frequency band, band range 0-5
			if(i > 5)
			{
				newBug.GetComponentInChildren<Flicker>()._band = i % 6;
			}
			else
			{
				newBug.GetComponentInChildren<Flicker>()._band = i;
			}

			newBug.GetComponent<FireFly>().startFireflyLife();
            enabledBugCount++;
        }

		yield return null;
	}

	//Called on Update
	//Give each bug a random band to read from
	void spawnBug()
	{
		int ran = Random.Range(0, 5);

		GameObject newBug = Instantiate(bug, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;
		newBug.GetComponentInChildren<Flicker>()._band = ran;
		newBug.GetComponent<FireFly>().startFireflyLife();

		enabledBugCount++;
	}

	//Called on firefly hit...Firefly.cs
	public void incCaughtCounter()
	{
		bugsCaught++;
		//Debug.Log(bugsCaught + "/" + bugsAllowed);
	}

	//Called on update
	void checkIfAllCaught()
	{
		if(bugsCaught >= bugsAllowed)
		{
			StartCoroutine(startWave());
		}
	}

	//Called on green bug hit...BlueBug.cs
	//Find all fireflies in the game, start their auto catch function
	public void clearWave()
	{
		GameObject[] bugs = GameObject.FindGameObjectsWithTag("Bug");

		for(int i = 0; i < bugs.Length; i++)
		{
			bugs[i].GetComponent<FireFly>().StartAutoCatch();
		}
	}

}
