using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarPulse : MonoBehaviour {

	public GameObject jarPulseCopy;

	[SerializeField]
	Vector3 originalScale;
	[SerializeField]
	Vector3 maxScale;

	[SerializeField]
	Vector3 maxScaleJarCopies;

	[SerializeField]
	bool startPulse = false;

	[SerializeField]
	int[] startJars;
	[SerializeField]
	int maxJars = 3;

	[SerializeField]
	GameObject[] copies;

	[SerializeField]
	float lerpTime = 0;
	[SerializeField]
	float[] lerpTimeCopies;
	[SerializeField]
	float[] lerpTimeAdds;

	[SerializeField] float[] multiplierNumbers;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		maxScale = new Vector3(originalScale.x + 0.02f, originalScale.y + 0.02f, originalScale.z);

		copies = new GameObject[maxJars];
		startJars = new int[maxJars];

		multiplierNumbers = new float[maxJars];
		multiplierNumbers[0] =  ((float)Screen.width / (float) Screen.height) * 0.09f;
		multiplierNumbers[1] =  ((float)Screen.width / (float) Screen.height) * 0.05f;
		multiplierNumbers[2] =  ((float)Screen.width / (float) Screen.height) * 0.03f;

		for (int i = 0; i < startJars.Length; i++)
		{
			startJars[i] = 0;
		}

		lerpTimeAdds = new float[maxJars];

		lerpTimeAdds[0] = 0.9f;
		lerpTimeAdds[1] = 0.7f;
		lerpTimeAdds[2] = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		
		/*if (Input.GetKeyDown(KeyCode.D))
		{
			if (this.name == "Jar1")
			{
				SetPulse(true);
			}
		}*/

		UpdateJars(0, multiplierNumbers[0]);
		UpdateJars(1, multiplierNumbers[1]);
		UpdateJars(2, multiplierNumbers[2]);

		if (startPulse && transform.localScale == maxScale)
		{
			lerpTime = 0;
			startPulse = false;
		}
		
		if (startPulse)
		{
			lerpTime += (2.0f * Time.deltaTime);
			transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, maxScale.x, lerpTime), Mathf.Lerp(transform.localScale.y, maxScale.y, lerpTime), transform.localScale.z);
		}
		else
		{
			lerpTime += (2.0f * Time.deltaTime);
			transform.localScale = new Vector3(Mathf.Lerp(maxScale.x, originalScale.x, lerpTime), Mathf.Lerp(maxScale.y, originalScale.y, lerpTime), transform.localScale.z);
		}
	}

	public void SetPulse(bool val)
	{
		lerpTime = 0;
		lerpTimeCopies = new float[maxJars];
		startPulse = val;

		if (startPulse == true)
		{
			for (int i = 0; i < copies.Length; i++)
			{
				if (copies[i] != null)
				{
					Destroy(copies[i].gameObject);
				}
			}
			
			copies = new GameObject[maxJars];

			for (int i = 0; i < copies.Length; i++)
			{
				startJars[i] = 1;
				lerpTimeCopies[i] = 0;
				GameObject temp = GameObject.Instantiate(jarPulseCopy);

				temp.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;

				temp.transform.position = transform.position;
				temp.transform.localScale = transform.localScale;

				maxScaleJarCopies = new Vector3(maxScale.x + 0.09f, maxScale.y + 0.09f, 0.2f);

				temp.transform.SetParent(this.gameObject.transform);
				
				//temp.transform.localPosition = new Vector3(temp.transform.localPosition.x, temp.transform.localPosition.y, temp.transform.localPosition.z);

				temp.GetComponent<SpriteRenderer>().color = Color.clear;
				copies[i] = temp;
			}
		}
	}

	void UpdateJars(int val, float maxScaleMult)
	{
		if (startJars[val] == 1)
		{
			if (copies[val] != null)
			{
				lerpTimeCopies[val] += (2.0f * Time.deltaTime * lerpTimeAdds[val]);

				float additions = lerpTimeCopies[val] * maxScaleMult;

				copies[val].transform.localScale = new Vector3(copies[val].transform.localScale.x + additions, copies[val].transform.localScale.y  + additions, transform.localScale.z);

				copies[val].transform.localPosition = new Vector3(copies[val].transform.localPosition.x, (transform.localPosition.y - 0.03f), copies[val].transform.localPosition.z);

				copies[val].GetComponent<SpriteRenderer>().color = Color32.Lerp(copies[val].GetComponent<SpriteRenderer>().color, Color.white, lerpTimeCopies[val]);

				if (copies[val].transform.localScale.x >= maxScaleJarCopies.x && copies[val].GetComponent<SpriteRenderer>().color.a >= Color.white.a - maxScaleMult)
				{
					lerpTimeCopies[val] = 0;
					startJars[val] = 2;
				}
			}
		}
		else if (startJars[val] == 2)
		{
			if (copies[val] != null)
			{
				lerpTimeCopies[val] += (Time.deltaTime * (lerpTimeAdds[val] / 4) );

				copies[val].GetComponent<SpriteRenderer>().color = Color32.Lerp(copies[val].GetComponent<SpriteRenderer>().color, Color.clear, lerpTimeCopies[val]);

				if (copies[val].GetComponent<SpriteRenderer>().color == Color.clear)
				{
					Destroy(copies[val].gameObject);
				}
			}
		}
	}
}
