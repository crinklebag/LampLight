using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunFactController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(Wait());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(7.0f);
		this.GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");
	}
}
