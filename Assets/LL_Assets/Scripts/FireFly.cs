using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireFly : MonoBehaviour {

    public GameObject fireflySparklePrefab;

    public 

    GameController gameController;
    BugController bugController;
    [SerializeField] GameObject[] jars;
    GameObject player;

    //[SerializeField] Light bugLight;
    [SerializeField] public GameObject image;
    [SerializeField] public GameObject glow;
	[SerializeField]  GameObject OuterGlow;
    [SerializeField] GameObject particles;
    [SerializeField] float speed = 0.05f;
	[SerializeField] float speedMul = 2.0f;
	[SerializeField] float speedSlowMul = 5.0f;
	[SerializeField] float rotSpeed = 2.5f;
	[SerializeField] float autoCatchSpeed = 4.0f;
	[SerializeField] float autoCatchDistance = 1.5f;
	//[SerializeField] float autoCatchRotSpeed = 500.0f;
	//[SerializeField] float autoCatchFaceSpeed = 200.0f;
    [SerializeField] GameObject destination;


	public bool isOn;
    [SerializeField]
    bool caught = false;
    float minMoveX = -10.0f;
    float maxMoveX = 10.0f;
	float minMoveY = -5.5f;
    float maxMoveY = 5.5f;
    Vector2 newPos;

	//look at 2d
	private Vector3 normTarget;
	private float angle;
	private Quaternion rot;

    [SerializeField] float freqMoveThresh;

    private float timeTillNewPosition;

    Vector3 startMarker = Vector3.zero;
    float startTime = 0;
    float journeyLength = 0;
    bool setDest = false;

    int randJar = 0;

    private bool canMove = true;

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		bugController = GameObject.FindGameObjectWithTag("GameController").GetComponent<BugController>();

        minMoveX = gameController.Bounds[2];
        minMoveY = gameController.Bounds[0];
        maxMoveX = gameController.Bounds[3];
        maxMoveY = gameController.Bounds[1];

        //StartCoroutine("ChoosePath");
		StartCoroutine("RandomPosition");
		StartCoroutine(ChangeState());

        destination = GameObject.Find("Destination");

        jars = new GameObject[5];
        jars[0] = GameObject.Find("Jar1");
        jars[1] = GameObject.Find("Jar2");
        jars[2] = GameObject.Find("Jar3");
        jars[3] = GameObject.Find("Jar4");
        jars[4] = GameObject.Find("Jar5");

        image.SetActive(false);

		for (int i = 0; i < gameController.BoundsGameObjects.Length; i++) 
		{
			Physics2D.IgnoreCollision(gameController.BoundsGameObjects[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
		}

		player = GameObject.FindGameObjectWithTag("Player");
	}

	public void startFireflyLife()
	{
        /*
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        for (int i = 0; i < gameController.BoundsGameObjects.Length; i++) 
		{
			Physics2D.IgnoreCollision(gameController.BoundsGameObjects[i].GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
		}
		*/

		/*image.SetActive(true);
		glow.SetActive(true);
		OuterGlow.SetActive(true);
		particles.SetActive(false);
		*/
		caught = false;
		canMove = true;
		this.GetComponent<CircleCollider2D>().enabled = true;

		StartCoroutine(TempSpeedIncrease());

	}

    // Update is called once per frame
    void Update()
    {
        SetDestination();

        if (caught)
        {
            // if (this.transform.position.x < destination.transform.position.x - 0.1f)
            //  {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination.transform.position, 10 * Time.deltaTime);

            if ((this.transform.position.x < destination.transform.position.x - 0.01f || this.transform.position.x > destination.transform.position.x + 0.01f) &&
                (this.transform.position.y < destination.transform.position.y - 0.01f || this.transform.position.y > destination.transform.position.y + 0.01f))
            {
                //Debug.Log("Travelling");
                float distCovered = (Time.time - startTime) * speed;
                float fracJourney = distCovered / journeyLength;
                this.transform.position = Vector3.MoveTowards(this.transform.position, destination.transform.position, fracJourney);
            } 
            else
            {
                gameController.CatchBug(randJar);
                Destroy(this.gameObject);
				/*image.SetActive(false);
				glow.SetActive(false);
				OuterGlow.SetActive(false);
				particles.SetActive(false);*/

				canMove = false;
            }

        } 
        else
        {
        	if(!gameController.gameObject.GetComponent<PauseController>().isPaused)
        	{
				moveBug();
        	}
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        /*if(other.gameObject.CompareTag("JarTop")){
            Debug.Log("Hit Jar Top");
        }*/

        if (other.gameObject.CompareTag("JarTop") && isOn ) 
        {
           if(!caught)
           {
           		catchBug();
           }
            
        }
    }

    void SetDestination()
    {
        setDest = true;
        randJar = gameController.WhichJar();
        //Debug.Log(randJar);
        destination = jars[randJar];
    }

    /*
    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("Dragonfly") && isOn)
        {
            //Debug.Log("Hit Dragonfly");
            //Destroy(other.gameObject);
            Destroy(this.gameObject);
            //gameController.CatchBug("Firefly");
        }
    }
    */

    //Check if band frequency is above the threshold, if so give it a new target position
    IEnumerator ChoosePath() 
    {
		yield return new WaitForSeconds(0.1f);

    	int band = glow.GetComponent<Flicker>()._band;
		float bandFreq = AudioPeer._audioBandBuffer[band];

    	if(bandFreq >= freqMoveThresh)
    	{
			float randY = Random.Range(minMoveY, maxMoveY);
       		float randX = Random.Range(minMoveX, maxMoveX);

        	newPos = new Vector2(randX, randY);
    	}
        StartCoroutine(ChoosePath());
    }

	IEnumerator RandomPosition()
	{
        if (!caught)
        {
            timeTillNewPosition = Random.Range(0.1f, 3.5f);

            yield return new WaitForSecondsRealtime(timeTillNewPosition);

            float randY = Random.Range(minMoveY, maxMoveY);
            float randX = Random.Range(minMoveX, maxMoveX);

            newPos = new Vector2(randX, randY);

            StartCoroutine(RandomPosition());
        }
	}

    //Move to and face the target position
	void moveBug()
    {
    	if(canMove)
    	{
			transform.position = Vector2.MoveTowards(this.transform.position, newPos, speed * Time.deltaTime);

			//Look at 2D
			normTarget = (newPos - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
			angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;

			rot = new Quaternion();
			rot.eulerAngles = new Vector3(0,0,angle-90);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
    	}
    }

	// If the bug's light is on turn the bug image off, if it's off turn the bug image on
	IEnumerator ChangeState() 
	{
        if (isOn) {
            // Debug.Log("Turning Off");
            image.SetActive(false);
        } else {
            // Debug.Log("Turning On");
            image.SetActive(true);
        }
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(ChangeState());
	}

    public bool IsOn() 
    {
        return isOn;
    }

    IEnumerator TempSpeedIncrease()
    {
    	float temp = speed;
    	speed = speed * speedMul;

    	while(speed > temp + 0.01)
    	{
    		speed = Mathf.Lerp(speed, temp, Time.deltaTime * speedSlowMul);
    		
    		yield return null;
    	}

    	yield return null;
    }

    public void setCanMove(bool b)
    {
    	canMove = b;
    }

    public void StartAutoCatch()
    {
    	StartCoroutine(AutoCatchBug());
    }

    IEnumerator AutoCatchBug()
    {
    	canMove = false;
		this.GetComponent<CircleCollider2D>().enabled = false;

		Vector2	pos = new Vector2(this.gameObject.GetComponent<Transform>().position.x, this.gameObject.GetComponent<Transform>().position.y);
		Vector2	playerPos = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);

		float z = this.gameObject.GetComponent<Transform>().position.z;

		Vector2 dir = pos - playerPos;
    	float dist = dir.magnitude;

		while(dist > autoCatchDistance)
		{
			//Look at 2D
			/*
			normTarget = (pos - playerPos).normalized;
			angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;
			rot = new Quaternion();
			rot.eulerAngles = new Vector3(0,0,angle+90);
			*/

			//Face, Rotate Around, Move Towards the player
			//this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
			//this.transform.RotateAround(player.transform.position, Vector3.forward, autoCatchRotSpeed);
			//this.transform.Translate(-this.transform.up * autoCatchSpeed * Time.deltaTime);

			//Translate Towards
			pos = Vector2.Lerp(pos, playerPos, Time.deltaTime * autoCatchSpeed);

			//Move
			this.gameObject.GetComponent<Transform>().position = new Vector3(pos.x, pos.y, z);

			//Reset
			playerPos = new Vector2(player.GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y);
			dir = pos - playerPos;
    		dist = dir.magnitude;

			yield return null;
		}

		if(!caught)
		{
			catchBug();
		}		

    	yield return null;
    }

    void catchBug()
    {
		caught = true;

		GameObject fireflySparkle = GameObject.Instantiate(fireflySparklePrefab);

		bugController.incCaughtCounter();

        //Debug.Log("Hit Jar Top Trigger Enter");
        // gameController.CatchBug("Firefly");
        // Turn off bug and glow
        image.SetActive(false);
        glow.SetActive(false);
		OuterGlow.SetActive(false);

        // turn particles on
        particles.SetActive(true);

        // turn the collider into a trigger
        this.GetComponent<CircleCollider2D>().enabled = false;

        // Play Sound
        //this.GetComponent<AudioSource>().Play();
        // Calculate the journey length and get the start pos
        startMarker = this.transform.position;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker, destination.transform.position);
    }
}
