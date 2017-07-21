using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimController : MonoBehaviour {

	[SerializeField] GameObject IntroAnim;
	[SerializeField] float time;

	public bool animIsDone = false;
	public bool inMainMenu = false;

	public GameObject[] thingsToHide;

	void Awake()
	{
		StartCoroutine(StopAnim());
	}

	IEnumerator StopAnim()
	{
		if (inMainMenu) {
			// if in main menu (so if anyone touches where the buttons are, it doesn't trigger when this animation is still playing)
			for (int i = 0; i < thingsToHide.Length; i++) {
				thingsToHide [i].SetActive (false);
			}
		}

		yield return new WaitForSeconds(time);
		animIsDone = true;

		if (inMainMenu) {
			for (int i = 0; i < thingsToHide.Length; i++) {
				thingsToHide [i].SetActive (true);
			}
		}

		IntroAnim.SetActive(false);
	}
}
