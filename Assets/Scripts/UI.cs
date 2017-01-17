using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    [SerializeField] List<GameObject> offBugs;

    List<GameObject> onBugs;
    [SerializeField]
    int bugCount;

	// Use this for initialization
	void Start () {
        onBugs = new List<GameObject>();
        bugCount = offBugs.Count;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
		
    public void AddBug() {
        // Choose a random Bug
        int randBug = Random.Range(0, offBugs.Count - 1);
        // Debug.Log("Adding Bug: " + randBug);
        // Turn that bug on
        offBugs[randBug].SetActive(true);
        // Remove the Bug from the off list
        onBugs.Add(offBugs[randBug]);
        // Add it to the on list
        offBugs.RemoveAt(randBug);
    }

    public void RemoveBug() {
        // Choose a random bug
        int randBug = Random.Range(0, onBugs.Count - 1);
        Debug.Log("Removing Bug: " + randBug);
        // Turn off the sprite
        onBugs[randBug].SetActive(false);
        // Add it to the out list
        offBugs.Add(onBugs[randBug]);
        // Remove it form the in list
        onBugs.RemoveAt(randBug);
    }

    public int GetBugCount() {
        return bugCount;
    }
}
