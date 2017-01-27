using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBeatBandFreq : MonoBehaviour {

public float scaleMultiplier = 5.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.localScale = new Vector3(scaleMultiplier * AudioPeer.beatAverage, transform.localScale.y, transform.localScale.z);
	}
}
