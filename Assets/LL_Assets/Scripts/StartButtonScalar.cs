using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StartButtonScalar : MonoBehaviour {

	[SerializeField]float min;
	[SerializeField]float max;
	[SerializeField] public float speed;
	[SerializeField]Text text;
	[SerializeField]GameObject Button;
	 public float tempScaleX ;




	// Use this for initialization
	void Start () {
		
		StartCoroutine (ScaleText ());
		//this.transform.localScale = new Vector3 (0.75f, 0.75f, 0.0f);
		tempScaleX = text.transform.localScale.x;
		tempScaleX = 0.9f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}


	IEnumerator ScaleText(){
		//float tempScaleX = text.transform.localScale.x;
		/*while (tempScaleX > min + 0.01f) {
			tempScaleX = Mathf.MoveTowards (tempScaleX, min, Time.deltaTime * speed);
			text.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			if (SceneManager.GetActiveScene ().buildIndex == 2) {
				Button.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			}
			//Debug.Log ("DOWN");
			yield return null;
		}
		*/

		while(tempScaleX < max - 0.01f){
			tempScaleX = Mathf.MoveTowards (tempScaleX, max, Time.deltaTime * speed);
			text.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);

			if (SceneManager.GetActiveScene ().buildIndex == 2) {
				Button.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			}
			//Debug.Log ("UP");
			yield return null;
		}

		while (tempScaleX > min + 0.01f) {
			tempScaleX = Mathf.MoveTowards (tempScaleX, min, Time.deltaTime * speed);
			text.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			if (SceneManager.GetActiveScene ().buildIndex == 2) {
				Button.transform.localScale = new Vector3 (tempScaleX, tempScaleX, 1.0f);
			}
			//Debug.Log ("DOWN");
			yield return null;
		}
		 

		StartCoroutine (ScaleText ());
	}
}

