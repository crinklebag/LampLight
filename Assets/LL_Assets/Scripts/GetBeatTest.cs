using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBeatTest : MonoBehaviour {

	bool moveLeft = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (AudioManager.beatCheck) {
			move ();
		}
	}

	void move ()
	{
		if (moveLeft) {
			Vector3 newPos = new Vector3(5.5f , transform.position.y, transform.position.z);
			this.transform.position = newPos;
			moveLeft = false;
		} 
		else 
		{
			Vector3 newPos = new Vector3(8.5f , transform.position.y, transform.position.z);
			this.transform.position = newPos;
			moveLeft = true;
		}

	}

}
