using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour {

	//private AudioManager am;
	private GameController gameController;
	private AudioSFX aSFX;

	private float topBound;
	private float leftBound;
	private float rightBound;

	private float spawnPos = 0.0f;

	[SerializeField] private float minSpawnTime = 3.0f;
	[SerializeField] private float maxSpawnTime = 5.0f;
	private float spawnTime = 0.0f;

	[SerializeField] private float speed = 3.0f;
	[SerializeField] private float bobSpeed = 2.0f;

	[SerializeField] private float minDist = 2.0f; //lower distance from top bound
	[SerializeField] private float maxDist = 0.0f;
	private float dist = 0.0f; //how far the spider is travelling
	private float desiredPosition = 0.0f; //the desired position
	private float hangCenterPos = 0.0f; //hold the original hanging position for the bobbing effect

	[SerializeField] private float minDownTime = 1.0f; //time the spider remains hanging min & max
	[SerializeField] private float maxdownTime = 3.0f;
	private float downTime = 0; //length of time to hang

	[SerializeField] private float bobDistance = 1.0f;

	private float count = 0;

	private bool isBobbing = false;
	private bool isBobbingUp = false;

	private bool beenHit = false;

	private bool hasFirefly = false;

	void Start()
	{
		aSFX = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSFX>();
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

		topBound = GameObject.Find("Top").transform.position.y;
		leftBound = GameObject.Find("Left").transform.position.x + 1.5f;
		rightBound = GameObject.Find("Right").transform.position.x - 1.5f;

		StartCoroutine(LyfeCycle());
	}

	void Update()
	{
		Counter();
	}

	void Counter()
	{
		count += Time.deltaTime;
	}

	void ResetCounter()
	{
		count = 0.0f;
	}

	IEnumerator LyfeCycle()
	{
		beenHit = false;
		hasFirefly = false;

		spawnPos = Random.Range(leftBound, rightBound);
		this.transform.position = new Vector3(spawnPos, this.transform.position.y, this.transform.position.z);

		spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
		yield return new WaitForSeconds(spawnTime);

		yield return StartCoroutine(MoveDown());

		yield return StartCoroutine(Hang());

		yield return StartCoroutine(MoveUp());

		StartCoroutine(LyfeCycle());

		yield return null;
	}

	IEnumerator MoveDown()
	{
		dist = Random.Range(minDist, maxDist);
		desiredPosition = topBound - dist;
		
		while(this.transform.position.y >= desiredPosition + 0.1f)
		{
			//Debug.Log("Moving Down");

			float temp = Mathf.MoveTowards(this.transform.position.y, desiredPosition, Time.deltaTime * speed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);
				
			yield return null;
		}

		yield return null;
	}

	IEnumerator MoveUp()
	{
		desiredPosition = topBound + 1.0f;

		while(this.transform.position.y <= desiredPosition - 0.1f)
		{
			//Debug.Log("Moving Up");

			float temp = Mathf.MoveTowards(this.transform.position.y, desiredPosition, Time.deltaTime * speed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);

			yield return null;
		}

		yield return null;
	}

	IEnumerator Hang()
	{
		ResetCounter();

		downTime = Random.Range(minDownTime, maxdownTime);

		while(count <= downTime)
		{
			//Debug.Log("Hanging");

			if(AudioManager.beatCheck && !isBobbing)
			{
				isBobbing = true;
				yield return StartCoroutine(Bob());
				isBobbing = false;
			}

			yield return null;
		}

		yield return null;
	}

	IEnumerator Bob()
	{
		hangCenterPos = this.transform.position.y;
		isBobbingUp = !isBobbingUp;

		if(isBobbingUp)
			yield return StartCoroutine(BobUp());
		else
			yield return StartCoroutine(BobDown());

		yield return null;
	}

	IEnumerator BobUp()
	{
		desiredPosition = this.transform.position.y + bobDistance;

		//Move up
		while(this.transform.position.y <= desiredPosition - 0.01f)
		{
			float temp = Mathf.Lerp(this.transform.position.y, desiredPosition, Time.deltaTime * bobSpeed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);

			yield return null;
		}

		//Move back to the original hanging position
		while(this.transform.position.y >= hangCenterPos + 0.01f)
		{
			float temp = Mathf.Lerp(this.transform.position.y, hangCenterPos, Time.deltaTime * bobSpeed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);

			yield return null;
		}

		yield return null;
	}

	IEnumerator BobDown()
	{
		desiredPosition = this.transform.position.y - bobDistance;

		//Move down
		while(this.transform.position.y >= desiredPosition + 0.01f)
		{
			float temp = Mathf.Lerp(this.transform.position.y, desiredPosition, Time.deltaTime * bobSpeed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);
				
			yield return null;
		}

		//Move back to original hanging position
		while(this.transform.position.y <= hangCenterPos - 0.01f)
		{
			float temp = Mathf.Lerp(this.transform.position.y, hangCenterPos, Time.deltaTime * bobSpeed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);

			yield return null;
		}

		yield return null;
	}

	IEnumerator ResetLyfeCycle()
	{
		//yield return StartCoroutine(MoveUp());
		//MoveUp()
		desiredPosition = topBound + 1.0f;
		while(this.transform.position.y <= desiredPosition - 0.1f)
		{
			//Debug.Log("Moving Up");

			float temp = Mathf.MoveTowards(this.transform.position.y, desiredPosition, Time.deltaTime * speed);

			this.transform.position = new Vector3(this.transform.position.x, temp, this.transform.position.z);

			yield return null;
		}

		beenHit = false;

		StartCoroutine(LyfeCycle());

		yield return null;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "JarTop")
		{
			if(!beenHit)
			{
				beenHit = true;

				StopAllCoroutines();

				StartCoroutine(ResetLyfeCycle());

				gameController.CrackJar();
				gameController.GetComponent<VibrationController>().Vibrate();
			}
		}
	}

	public bool GetHasFirefly()
	{
		return hasFirefly;
	}

	public void SetHasFirefly()
	{
		hasFirefly = true;
	}
}
