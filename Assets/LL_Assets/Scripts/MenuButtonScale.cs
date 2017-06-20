using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonScale : MonoBehaviour {

	bool scale = false;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update ()
	{
		if (AudioManager.beatCheck) {
			Scale ();
		}
	}

	void Scale()
	{
		if (scale) {
			Vector3 newScale = new Vector3(1.5f , 1.5f);
			this.transform.localScale += newScale;
			scale = false;
		} 
		else 
		{
			Vector3 newScale = new Vector3(1.0f ,1.0f);
			this.transform.localScale -= newScale;
			scale = true;
		}

	}

}
