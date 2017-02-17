using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFly : MonoBehaviour {

	public float glowlevel = 0.0f;
	public float playtime =2.0f;
	// Use this for initialization

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Play")
		{
			Glow ();
		}
		else
			glowlevel = Mathf.Lerp(glowlevel,0.0f, Time.deltaTime);	
	}

	// Update is called once per frame
	void Update () {


		Color tempColor = this.GetComponent<SpriteRenderer> ().color;
		tempColor.a = glowlevel;
		if (Input.GetKey ("up")) {
			glowlevel = Mathf.Lerp (glowlevel, 1.0f, Time.deltaTime);
		} else {
			glowlevel = Mathf.Lerp(glowlevel,0.0f, Time.deltaTime);	
		}

		if (Input.GetKeyDown ("down")) 
		{
			glowlevel -= 0.05f;
		}

		this.GetComponent<SpriteRenderer> ().color = tempColor;
}

	void Glow(){
		//glowlevel += 0.5f;
		glowlevel = Mathf.Lerp(glowlevel,1.0f, Time.deltaTime * 10);
		playtime -= Time.deltaTime;
		if (playtime <= 0) 
		{
			//LOAD SCENE
		}

	}
}
