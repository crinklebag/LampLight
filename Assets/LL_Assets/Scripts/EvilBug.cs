using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBug : MonoBehaviour {

	private GameController gameController;

	private Vector3 destination;
	private Vector3 randomPosition;

	private float minMoveX = -10.0f;
    private float maxMoveX = 10.0f;
	private float minMoveY = -5.5f;
    private float maxMoveY = 5.5f;

    private float timeCount = 0.0f;

    //LookAt 2D
	private Vector3 normTarget;
	private float angle;
	private Quaternion rot;

    [SerializeField] float speed = 1.25f;
    [SerializeField] float rotSpeed = 5.0f;

    AudioSFX aSFX;

    void Awake ()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
	}

    void Update ()
	{
		CountTime();
		RandomPosition();
		lookAtPosition();
	}

	//Call to start life cycle
	public void StartBugLyfeCoroutine (float inT, float arT, float outT, GameObject[] bounds)
	{
		for (int i = 0; i < bounds.Length; i++) 
		{
			Physics2D.IgnoreCollision(bounds[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
		}

		//StartCoroutine(RandomPosition());
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
			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			
			yield return null;
		}
		yield return null;
	}

	//move around for a give time
	IEnumerator MoveAround (float t)
	{
		while(timeCount < t)
		{
			destination = randomPosition;

			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);

			yield return null;
		}
		yield return null;
	}

	//move off the screen for a given time
	IEnumerator MoveOut (float t)
	{
		destination = new Vector3(this.transform.position.x, -10.0f, -1.0f);
		while(timeCount < t)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
			
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

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("JarTop"))
		{
			aSFX.playDodo();

			//TODO: Disable Collider? or end lyfe cycle?

			gameController.CrackJar();
			gameController.GetComponent<VibrationController>().Vibrate();
		}
	}
}
