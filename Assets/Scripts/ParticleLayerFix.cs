using UnityEngine;
using System.Collections;

public class ParticleLayerFix : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().sortingLayerName = "Particles";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
