using UnityEngine;
using System.Collections;

public class Bug : MonoBehaviour {

    GameController gameController;

    [SerializeField] Light bugLight;
    [SerializeField] GameObject image;
    [SerializeField] GameObject glow;
    [SerializeField] GameObject particles;
    [SerializeField] float speed = 0.05f;
    [SerializeField] GameObject destination;

    bool caught = false;
    bool isOn;
    float lightMin = 0.3f;
    float lightMax = 0.8f;
    float maxWaitTime = 6;
    float minWaitTime = 2;
    float minMove = -6;
    float maxMove = 6;
    Vector2 newPos;
   

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        isOn = false;
        StartCoroutine("ChangeState");
        StartCoroutine("ChoosePath");
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector2.MoveTowards(this.transform.position, newPos, speed * Time.deltaTime);

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

    IEnumerator ChoosePath() {
        
        float randY = Random.Range(minMove, maxMove);
        float randX = Random.Range(minMove, maxMove);

        newPos = new Vector2(randX, randY);

        yield return new WaitForSeconds(4.0f);

        StartCoroutine(ChoosePath());

    }

    IEnumerator ChangeState() {
        
        // If the bugs light is on turn it off, if its off turn it on
        if (isOn) {
            // Debug.Log("Turning Off");
            image.SetActive(true);
            glow.SetActive(false);
            isOn = false;
            StartCoroutine("ChoosePath");
            bugLight.intensity = 0;
        }
        else {
            // Debug.Log("Turning On");
            image.SetActive(false);
            glow.SetActive(true);
            isOn = true;
        }

        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        StartCoroutine(ChangeState());
    }

    public bool IsOn() {
        return isOn;
    }
}
