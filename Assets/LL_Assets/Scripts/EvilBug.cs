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

	[SerializeField] GameObject glow;
	[SerializeField] GameObject sprite;
	[SerializeField] GameObject hitParticle;
	[SerializeField] GameObject notification;

    [SerializeField] private float speed = 1.25f;
    [SerializeField] private float rotSpeed = 5.0f;

	[SerializeField] private float margin = 1.0f;

	[SerializeField] private float notificationScale = 4.0f;
	[SerializeField] private float notificationScaleMul = 1.5f;
	[SerializeField] private float notificationScaleSpeed = 5.0f;
	[SerializeField] private float notificationActiveScaleSpeed = 15.0f;

    AudioSFX aSFX;

    private bool beenHit = false;

    private bool isNotificationOff = false;
    private bool isNotificationOn = false;
    private bool canFlash = false;

    void Awake ()
	{
		//Ensure notification is proper size to size up and turn it off
		notification.transform.localScale = new Vector3(0.05f, 0.05f, 1.0f); 
		notification.SetActive(false);

		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		aSFX = GameObject.Find("SFXController").GetComponent<AudioSFX>();
	}

    void Update ()
	{
		CountTime();
		RandomPosition();
		lookAtPosition();
		showNotification();
		flashNotificationCheck();
	}

	//Call to start life cycle
	public void StartBugLyfeCoroutine (float inT, float arT, float outT)
	{
		for (int i = 0; i < gameController.BoundsGameObjects.Length; i++) 
		{
			Physics2D.IgnoreCollision(gameController.BoundsGameObjects[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
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

	//Check if object is visible
	//Check which side were on
	//Set notification position & set active
	void showNotification()
	{
		float posX = this.transform.position.x;
		float posY = this.transform.position.y;

		if(posX <= (minMoveX - margin) && posY <= maxMoveY)//left side
		{
			//Debug.Log("Left Side");

			notification.transform.position = new Vector3(minMoveX + 0.5f, this.transform.position.y, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX >= (maxMoveX + margin)&& posY <= maxMoveY)//right side
		{
			//Debug.Log("Right Side");

			notification.transform.position = new Vector3(maxMoveX - 0.5f, this.transform.position.y, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posY >= (maxMoveY + margin) && posX >= minMoveX && posX <= maxMoveX)//top side
		{
			//Debug.Log("Top Side");

			notification.transform.position = new Vector3(this.transform.position.x, maxMoveY - 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX <= (minMoveX - margin) && posX <= maxMoveX && posY >= (maxMoveY + margin))//top left corner
		{
			//Debug.Log("Top Left Side");

			notification.transform.position = new Vector3(minMoveX + 0.5f, maxMoveY - 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else if (posX >= (maxMoveX + margin) && posX >= minMoveX && posY >= (maxMoveY + margin))//top right corner
		{
			//Debug.Log("Top Right Side");

			notification.transform.position = new Vector3(maxMoveX - 0.5f, maxMoveY - 0.5f, this.transform.position.z);
			if(!isNotificationOn)
			{
				isNotificationOn = true;
				StartCoroutine(turnOnNotification());
			}
		}
		else
		{
			//Debug.Log("Visible!");
			if(!isNotificationOff)
			{
				isNotificationOff = true;
				StartCoroutine(turnOffNotification());
			}

		}
	}

	//turn sprites off and turn particle effect on
	void endLyfe()
	{
		glow.SetActive(false);
		sprite.SetActive(false);

		GameObject RedParticle = Instantiate(hitParticle) as GameObject;
		RedParticle.transform.position = this.transform.position;
	}

	void flashNotificationCheck()
	{
		if(notification.activeSelf && canFlash)
		{
			if(AudioManager.beatCheck)
			{
				StartCoroutine(flashNotification());
			}
		}
	}

	IEnumerator flashNotification()
	{
		float currSize = notification.transform.localScale.x;
		float desiredSize = notification.transform.localScale.x * notificationScaleMul;
		float tempSize = notification.transform.localScale.x;

		while(tempSize < desiredSize)
		{
			tempSize = Mathf.MoveTowards(tempSize, desiredSize, Time.deltaTime * notificationScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		while(tempSize > currSize)
		{
			tempSize = Mathf.MoveTowards(tempSize, currSize, Time.deltaTime * notificationScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		yield return null;
	}

	IEnumerator turnOnNotification()
	{
		notification.SetActive(true);

		float tempSize = notification.transform.localScale.x;

		while(tempSize <= notificationScale - 0.05f)
		{
			tempSize = Mathf.MoveTowards(tempSize, notificationScale, Time.deltaTime * notificationActiveScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		canFlash = true;
		yield return null;
	}

	IEnumerator turnOffNotification()
	{
		float tempSize = notification.transform.localScale.x;

		while(tempSize > 0.05f)
		{
			tempSize = Mathf.MoveTowards(tempSize, 0.05f, Time.deltaTime * notificationActiveScaleSpeed);
			notification.transform.localScale = new Vector3(tempSize, tempSize, 1.0f);
			yield return null;
		}

		notification.SetActive(false);
		yield return null;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(!beenHit)
		{
			if (other.gameObject.CompareTag("JarTop"))
			{
				beenHit = true;

				aSFX.playDodo();

				endLyfe();

				gameController.CrackJar();
				gameController.GetComponent<VibrationController>().Vibrate();
			}
		}
	}
}