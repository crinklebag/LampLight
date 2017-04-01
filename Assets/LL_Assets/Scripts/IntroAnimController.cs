using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimController : MonoBehaviour {

	[SerializeField] GameObject IntroAnim;
	[SerializeField] float time;

	// Use this for initialization
	void Start () {
		//StartCoroutine(StopAnim());
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(StopAnim());
	
	}

	IEnumerator StopAnim(){
	yield return new WaitForSeconds(time);
	IntroAnim.SetActive(false);
	}
}
