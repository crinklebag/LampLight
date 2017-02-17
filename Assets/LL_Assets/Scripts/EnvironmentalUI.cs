using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalUI : MonoBehaviour {

    [SerializeField] float maxLightIntensity = 0.3f;
    [SerializeField] GameObject[] destinationObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FillJar() {
        int randJar = Random.Range(0, destinationObjects.Length - 1);
        Vector3 newDestination = destinationObjects[randJar].transform.position;

    }
}
