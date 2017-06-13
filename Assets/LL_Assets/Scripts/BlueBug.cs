﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBug : MonoBehaviour {

	//Controllers
	private GameController gameController;
	private BugController bugController;
	AudioSFX aSFX;

	//Movement Positions
	private Vector3 destination; //Centre of the screen, for bug start life cycle
	private Vector3 randomPosition; //Random position to move to, for main life cycle

	//Game Bounds
	private float minMoveX = -10.0f;
    private float maxMoveX = 10.0f;
	private float minMoveY = -5.5f;
    private float maxMoveY = 5.5f;

	//Time to check agains't each life cycles assigned length
    private float timeCount = 0.0f;

    //LookAt 2D
	private Vector3 normTarget;
	private float angle;
	private Quaternion rot;

	//Child objects and particle
	[SerializeField] GameObject glow;
	[SerializeField] GameObject sprite;
	[SerializeField] GameObject hitParticle;
	[SerializeField] GameObject innerGlow;
	[SerializeField] GameObject followParticle;

	//Movement Variables
    [SerializeField] float speed = 1.25f;
    [SerializeField] float rotSpeed = 5.0f;

    //Collision bool
	private bool beenHit = false;

	//Hacky Pause Bool
    private bool isPaused = false;

    //Inner glow circle
    [SerializeField] float innerGlowSpeed = 5.0f;
    [SerializeField] float innerGlowMaxSize = 2.5f;
    [SerializeField] float innerGlowMinSize = 1.5f;
    private bool isInnerGlowing = false;


	//Find game controller and find sfx controller
    void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		bugController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BugController>();
		aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
	}

	//Update functions
    void Update ()
	{
		isPaused = gameController.gameObject.GetComponent<PauseController>().isPaused;

		if(!isPaused)
		{
			CountTime();
			RandomPosition();
			lookAtPosition();

			if(AudioManager.beatCheck && !isInnerGlowing)
			{
				isInnerGlowing = true;
				StartCoroutine(scaleInnerGlow());
			}
		}
	}

	//Call to start life cycle
	public void StartBugLyfeCoroutine (float inT, float arT, float outT)
	{
		for (int i = 0; i < gameController.BoundsGameObjects.Length; i++) 
		{
			Physics2D.IgnoreCollision(gameController.BoundsGameObjects[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
		}

		followParticle.SetActive(true);
		StartCoroutine(BugLyfe(inT, arT, outT));
	}

	//Controls bug's life cycle
	IEnumerator BugLyfe (float inTime, float aroundTime, float outTime)
	{
		ResetCountTime();
		yield return StartCoroutine(MoveIn(inTime));
		ResetCountTime();
		yield return StartCoroutine(MoveAround(aroundTime));
		ResetCountTime();
		yield return StartCoroutine(MoveOut(outTime));

		yield return null;
	}

	//Move in from off the screen for a give time
	IEnumerator MoveIn (float t)
	{				
		destination = new Vector3(0.0f, 0.0f, -1.0f);

		while(timeCount < t)
		{
			if(!isPaused)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			}
			yield return null;
		}
		yield return null;
	}

	//Move around for a give time
	IEnumerator MoveAround (float t)
	{
		while(timeCount < t)
		{
			destination = randomPosition;

			if(!isPaused)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			}
			yield return null;
		}
		yield return null;
	}

	//Move off the screen for a given time
	IEnumerator MoveOut (float t)
	{
		destination = new Vector3(this.transform.position.x, -10.0f, -1.0f);
		while(timeCount < t)
		{
			if(!isPaused)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			}

			yield return null;
		}
		yield return null;
	}

	//Set randomPosition to a random position within the give range when audioManager returns a beat
	void RandomPosition ()
	{
		if (AudioManager.beatCheckHalf)
		{
			float randY = Random.Range(minMoveY, maxMoveY);
            float randX = Random.Range(minMoveX, maxMoveX);

            randomPosition = new Vector3(randX, randY, -1.0f);
		}
	}

	//Constantly force the bug to face the in the direction of its destination
	void lookAtPosition ()
	{
		normTarget = (new Vector2(destination.x, destination.y) - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
		angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;

		rot = new Quaternion();
		rot.eulerAngles = new Vector3(0,0,angle-90);
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
	}

	//Increase count by delta time
	void CountTime ()
	{
		timeCount += Time.deltaTime;
	}

	//Call before each part of the life cycle to ensure life cycle runs the correct amonut of time
	void ResetCountTime()
	{
		timeCount = 0.0f;
	}

	//Turn sprites off and turn particle effect on
	void endLyfe()
	{
		glow.SetActive(false);
		innerGlow.SetActive(false);
		sprite.SetActive(false);
		followParticle.SetActive(false);
		beenHit = false;

		GameObject BlueParticle = Instantiate(hitParticle) as GameObject;
		BlueParticle.transform.position = this.transform.position;
	}

	//Scale up and down circle sprite on beat
	IEnumerator scaleInnerGlow()
	{
		float scaleX = innerGlow.transform.localScale.x;

		while(scaleX < innerGlowMaxSize - 0.01f)
		{
			scaleX = Mathf.MoveTowards(scaleX, innerGlowMaxSize, Time.deltaTime * innerGlowSpeed);
			innerGlow.transform.localScale = new Vector3(scaleX, scaleX, 1.0f);
			yield return null;
		}

		while(scaleX > innerGlowMinSize + 0.01f)
		{
			scaleX = Mathf.MoveTowards(scaleX, innerGlowMinSize, Time.deltaTime * innerGlowSpeed);
			innerGlow.transform.localScale = new Vector3(scaleX, scaleX, 1.0f);
			yield return null;
		}

		isInnerGlowing = false;
		yield return null;
	}

	//Check it the bug has already been hit & check if it is colliding with the player
	//Set bool to ensure no double hit, play sound effect, end bug life
	//Tell game controller that the player has been hit, tell vibration controller to vibrate
	void OnTriggerEnter2D(Collider2D other)
	{
		if(!beenHit)
		{
			if (other.gameObject.CompareTag("JarTop"))
			{
				beenHit = true;
				Debug.Log("Green Hit");

				aSFX.playGreenDodo();
				endLyfe();
				//gameController.makeLotsOfBugs();

				bugController.clearWave();
			}
		}
	}
}