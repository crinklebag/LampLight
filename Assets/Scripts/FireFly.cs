using UnityEngine;
using System.Collections;

public class FireFly : MonoBehaviour {

    GameController gameController;

    //[SerializeField] Light bugLight;
    [SerializeField] GameObject image;
    [SerializeField] GameObject glow;
    [SerializeField] GameObject particles;
    [SerializeField] float speed = 0.05f;
	[SerializeField] float rotSpeed = 2.5f;
    [SerializeField] GameObject destination;

	public bool isOn;
    bool caught = false;
    float lightMin = 0.3f;
    float lightMax = 0.8f;
    float maxWaitTime = 6.0f;
    float minWaitTime = 2.0f;
    float minMoveX = -9.0f;
    float maxMoveX = 3.0f;
	float minMoveY = -3.5f;
    float maxMoveY = 5.5f;
    Vector2 newPos;

	//look at 2d
	private Vector3 normTarget;
	private float angle;
	private Quaternion rot;

    [SerializeField] float freqMoveThresh;
   

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        StartCoroutine("ChoosePath");
		StartCoroutine(ChangeState());

		image.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		moveBug();

        if (caught) {
            if (this.transform.position.x < destination.transform.position.x - 0.1f) {
                this.transform.position = Vector3.MoveTowards(this.transform.position, destination.transform.position,  10 * Time.deltaTime);
            } else {
                gameController.CatchBug("Firefly");
                Destroy(this.gameObject);
            }
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        /*if(other.gameObject.CompareTag("JarTop")){
            Debug.Log("Hit Jar Top");
        }*/
        
        if (other.gameObject.CompareTag("JarTop") && isOn && other.GetComponentInParent<Jar>().Boosting()) {
            Debug.Log("Hit Jar Top");
            // gameController.CatchBug("Firefly");
            caught = true;
            // Turn off bug and glow
            image.SetActive(false);
            glow.SetActive(false);
            // turn particles on
            particles.SetActive(true);
            // turn the collider into a trigger
            this.GetComponent<CircleCollider2D>().enabled = false;
            // Play Sound
            this.GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.CompareTag("JarTop") && isOn && other.GetComponentInParent<Jar>().Boosting()) {
            // Debug.Log("Hit Jar Top");
            Destroy(this.gameObject);
            gameController.CatchBug("Firefly");
        }
    }

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

    //Move to and face the target position
	void moveBug()
    {
		transform.position = Vector2.MoveTowards(this.transform.position, newPos, speed * Time.deltaTime);

		//Look at 2D
		normTarget = (newPos - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
		angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;

		rot = new Quaternion();
		rot.eulerAngles = new Vector3(0,0,angle-90);
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

	// If the bug's light is on turn the bug image off, if it's off turn the bug image on
	IEnumerator ChangeState() 
	{
        if (isOn) {
            // Debug.Log("Turning On");
            image.SetActive(false);
        } else {
            // Debug.Log("Turning Off");
            image.SetActive(true);
        }
		yield return new WaitForSeconds(0.1f);
		StartCoroutine(ChangeState());
	}

    public bool IsOn() 
    {
        return isOn;
    }
}
