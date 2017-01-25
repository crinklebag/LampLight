using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPMest : MonoBehaviour {

	[SerializeField] Text text;

	// Update is called once per frame
	void Update () {
		text.text = "BPM ESTIMATE: " + AudioPeer.EstimateBPM;
	}
}
