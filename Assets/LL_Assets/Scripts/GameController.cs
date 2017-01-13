using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

    Jar player;
    UI uiController;

    [SerializeField] GameObject fireflyPrefab;
	[SerializeField] GameObject fireflyLightPrefab;

    int bugGoal = 10;
    int bugCounter;
    float maxX = 3;
    float minX = -10;
    float maxY = 6;
    float minY = -4;

    // Use this for initialization
    void Start () {
        uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UI>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Jar>();
        InitializeBugs();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void InitializeBugs ()
	{
		int count = uiController.GetBugCount ();

		for (int i = 0; i < count; i++) {
			float randX = Random.Range (minX, maxX);
			float randY = Random.Range (minY, maxY);
			Vector3 randPos = new Vector3 (randX, randY, -0.5f);
			GameObject newBug = Instantiate (fireflyPrefab, randPos, Quaternion.identity) as GameObject;

			//Assign each bug a frequency band,band range 0-5
			if (i > 5) {
				int tempBand = i - 6;
				newBug.GetComponentInChildren<Flicker> ()._band = tempBand;
			} else {
				newBug.GetComponentInChildren<Flicker> ()._band = i;
			}

        }
    }

    public void CatchBug(string bugType) {
        bugCounter++;
        uiController.AddBug();

        if (bugCounter == bugGoal) {
            Debug.Log("End Game");
        }
    }

    public void ReleaseBug() {
        if (bugCounter > 0) {
            // Remove the Bug from the UI
            uiController.RemoveBug();
            // Intsantiate a new bug at a random position
            Vector2 randPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            GameObject newBug = Instantiate(fireflyPrefab, randPos, Quaternion.identity) as GameObject;
            // decrease the counter
            bugCounter--;
        }
    }

    public int GetBugCount() {
        return bugCounter;
    }
}
