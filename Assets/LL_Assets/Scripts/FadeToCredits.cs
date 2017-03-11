using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToCredits : MonoBehaviour {

	[SerializeField] private float InactiveTime = 3.0f;
	private float timer = 0.0f;

	[SerializeField] private float fadeSpeed = 5.0f;


	void Update ()
	{
		if (!Input.anyKeyDown) 
		{
			timer += Time.deltaTime;
			if (timer >= InactiveTime) 
			{
				Color newVal = this.GetComponent<SpriteRenderer> ().color;
				newVal.a = Mathf.Lerp (newVal.a, 1.0f, Time.deltaTime * fadeSpeed);
				this.GetComponent<SpriteRenderer> ().color = newVal;
				if (newVal.a >= 0.995f)
				{
					this.GetComponent<SceneLoad>().LoadScene("Credits");
					this.enabled = false;
				}
			}
		} 
		else
		{
			timer = 0.0f;
			Color col = this.GetComponent<SpriteRenderer>().color;
			this.GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, 0.0f);
		}
	}
}