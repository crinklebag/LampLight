using UnityEngine;
using System.Collections;

public class Spider : MonoBehaviour {

    GameController gameController;

    public enum SpiderState { IDLE, LOWER, RAISE };
    SpiderState currentState = SpiderState.IDLE;

    [SerializeField] float speed = 5;

    TrailRenderer trail;
    float minLower = -3;
    float maxLower = 5.5f;
    bool top = true;
    bool setPosition = false;
    Vector3 newDestination;
    Vector3 homeDestination;

	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        homeDestination = this.transform.position;
        StartCoroutine("ChangeState");
        trail = this.GetComponent<TrailRenderer>();
        trail.sortingLayerName = "Particles";
        trail.sortingOrder = 2;
	}
	
	// Update is called once per frame
	void Update () {
        if (currentState == SpiderState.LOWER) {
            Lower();
        }

        if (currentState == SpiderState.RAISE) {
            Raise();
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("JarTop") && gameController.GetBugCount() > 0) {
            //gameController.ReleaseBug();
            RaiseSpider();
        }
    }

    void Lower() {
        transform.position = Vector3.MoveTowards(this.transform.position, newDestination, speed * Time.deltaTime);
        if (transform.position.y < newDestination.y + 0.2f) { currentState = SpiderState.IDLE; }
    }

    void Raise() {
        transform.position = Vector3.MoveTowards(this.transform.position, homeDestination, speed * Time.deltaTime);
        if (transform.position.y > homeDestination.y - 0.2f) {
            currentState = SpiderState.IDLE;
            trail.Clear();
        }
    }

    void DropSpider() {
        if (!setPosition)  {
            setPosition = true;
            float randY = Random.Range(minLower, maxLower);
            newDestination = new Vector3(this.transform.position.x, randY, -1);
            currentState = SpiderState.LOWER;
        }
    }

    void RaiseSpider() {
        currentState = SpiderState.RAISE;
        setPosition = false;
    }

    IEnumerator ChangeState() {
        // Wait
        float newWait = Random.Range(10, 20);
        yield return new WaitForSeconds(newWait);
        // Lower
        DropSpider();
        // Wait
        newWait = Random.Range(4, 10);
        yield return new WaitForSeconds(newWait);
        // Raise
        RaiseSpider();
        // Start Again
        StartCoroutine("ChangeState");
    }
}
