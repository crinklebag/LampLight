using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBeatTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (AudioPeer.GetBeat ()) {
			move ();
		}
	}

	void move ()
	{
		if (this.transform.position.x >= 8.4) {
			Vector3 newPos = new Vector3(5.5f , transform.position.y, transform.position.z);
			this.transform.position = newPos;
		} else {
			Vector3 newPos = new Vector3(8.5f , transform.position.y, transform.position.z);
			this.transform.position = newPos;
		}

	}

}
