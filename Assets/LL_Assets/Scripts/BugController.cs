using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugController : MonoBehaviour {

	[SerializeField] private GameObject WavePanel;

	GameController gameController;
	UI uiController;

	[SerializeField] GameObject bug;//firefly prefab
	private int bugsCaught = 0;
	private int bugsEaten = 0;

	[SerializeField] int bugsAllowed = 3;//Allow only this many bugs per wave
	public int enabledBugCount = 0;//amonut of currently enabled bugs

	[SerializeField] float instMinTime = 5.0f;//min instantiate time
	[SerializeField] float instMaxTime = 12.0f;//max instantiate time
	private float instantiateTime = 5.0f;//the instantiate time

	private float countTime;//counter to check for instatiation
	private int spawnIndex = 0;//which point to spawn at
	private bool isPaused = false;//for spawn counter
	[SerializeField] Transform[] spawnPoints;//For the fireflies

	float _startScale;
	[SerializeField] float[] scales;

	void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
		instantiateTime = 10.0f;

		//Define scale sizes for bugs
		//scales [2.0, 2.4, 2.8, 3.2, 3.6, 4.0]
		scales = new float[6];
		scales[0] = 2.6f;
		scales[1] = 2.3f;
		scales[2] = 2.0f;
		scales[3] = 1.7f;
		scales[4] = 1.4f;
		scales[5] = 1.1f;
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
		bugsEaten = 0;
		enabledBugCount = 0;

		//set bugs allowed
		bugsAllowed += 5;

		Debug.Log("Wave " + uiController.getWaveCount() + " Start\n" + bugsAllowed + " bugs");
		WavePanel.SetActive (true);
		WavePanel.GetComponent<Animator> ().SetTrigger ("PlayAnim");
	
		Debug.Log ("waveUp");
		yield return new WaitForSeconds(1.0f);

		//spawn 3/4 of the wave right away
		for(int i = 0; i < bugsAllowed * 0.75f; i++)
		{
			spawnIndex = Random.Range(0, spawnPoints.Length);
			GameObject newBug = Instantiate(bug, spawnPoints[spawnIndex].position, Quaternion.identity) as GameObject;

			//Assign each bug a frequency band, band range 0-5
			if(i > 5)
			{
				newBug.GetComponentInChildren<Flicker>()._band = i % 6;
				newBug.GetComponentInChildren<Flicker>().setStartScale(scales[i % 6]);
			}
			else
			{
				newBug.GetComponentInChildren<Flicker>()._band = i;
				newBug.GetComponentInChildren<Flicker>().setStartScale(scales[i]);
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
		newBug.GetComponentInChildren<Flicker>().setStartScale(scales[ran]);
		newBug.GetComponent<FireFly>().startFireflyLife();

		enabledBugCount++;
	}

	//Called on firefly hit...Firefly.cs
	public void incCaughtCounter()
	{
		bugsCaught++;
		Debug.Log(bugsCaught + "/" + bugsAllowed);
	}

	public void incEatenCounter()
	{
		bugsEaten++;
		Debug.Log("Bug Eaten");
	}

	//Called on update
	void checkIfAllCaught()
	{
		if((bugsCaught + bugsEaten) >= bugsAllowed)
		{
			//Debug.Log("Start Wave");
			StartCoroutine(startWave());
		}
	}

	//Called on green bug hit...BlueBug.cs
	//Find all fireflies in the game, start their auto catch function
	public void clearWave()
	{
		GameObject[] bugs = GameObject.FindGameObjectsWithTag("Bug");

		//Debug.Log("Clear Wave");

		for(int i = 0; i < bugs.Length; i++)
		{
			bugs[i].GetComponent<FireFly>().StartAutoCatch();
		}
	}

}
