using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonController : MonoBehaviour {

    MainMenuController mmController;

    bool canPulse = false;
    bool increaseSize = true;

    [SerializeField] Vector3 startSize;
    [SerializeField] Vector3 endSize;

    float startTime;
    float journeyLength;
    float speed = 3;

	// Use this for initialization
	void Start () {
        mmController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuController>();
        startSize = Vector3.one;
        endSize = new Vector3(1.3f, 1.3f, 1.3f);
	}
	
	// Update is called once per frame
	void Update () {
		if (canPulse) {
            if (increaseSize) {
                IncreaseSize();
            } else {
                DecreaseSize();
            }
        }
	}

    void IncreaseSize() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        this.transform.localScale = Vector3.Lerp(startSize, endSize, fracJourney);
        if (fracJourney >= 1) {
            startTime = Time.time;
            increaseSize = false; 
        }
    }

    void DecreaseSize() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        this.transform.localScale = Vector3.Lerp(endSize, startSize, fracJourney);
        if (fracJourney >= 1) {
            mmController.LoadScene();
            canPulse = false;
        }
    }

    public void StartPlay() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startSize, endSize);
        canPulse = true;
    }
}
