using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFly : MonoBehaviour {

	[SerializeField] float glowlevel = 1.0f;
	[SerializeField] float playtime =2.0f;
	[SerializeField] float glowSpeed = 5;

	bool glowing = false;

	// Use this for initialization

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Play") {
			
			Debug.Log ("collision");
			glowing = true;
		} 
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.tag == "Play") {
			//Glow ();
			glowlevel = Mathf.Lerp(glowlevel,0.0f, Time.deltaTime);
			Debug.Log ("collisionExit");
			glowing = false;
		} 
	}
	// Update is called once per frame
	void Update () {

		if (glowing) {
			glowlevel = Mathf.Lerp(glowlevel,1.0f, Time.deltaTime * glowSpeed);
		} else {
			glowlevel = Mathf.Lerp(glowlevel,0.0f, Time.deltaTime * glowSpeed);
		}

		Color tempColor = this.GetComponent<SpriteRenderer> ().color;
		tempColor.a = glowlevel;
		if (Input.GetKey ("up")) {
			glowlevel = Mathf.Lerp (glowlevel, 1.0f, Time.deltaTime);
		} 

		if (Input.GetKeyDown ("down")) 
		{
			glowlevel -= 0.05f;
		}

		this.GetComponent<SpriteRenderer> ().color = tempColor;
	}
		
}
