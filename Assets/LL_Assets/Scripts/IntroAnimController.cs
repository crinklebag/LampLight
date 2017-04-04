using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimController : MonoBehaviour {

	[SerializeField] GameObject IntroAnim;
	[SerializeField] float time;

	public bool animIsDone = false;

	void Awake()
	{
		StartCoroutine(StopAnim());
	}

	IEnumerator StopAnim()
	{
		yield return new WaitForSeconds(time);
		animIsDone = true;
		IntroAnim.SetActive(false);
	}
}
