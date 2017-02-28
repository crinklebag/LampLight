using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour {

	[SerializeField]private AudioManager AM;
	[SerializeField]private SpriteRenderer FadeObj;

	[SerializeField] private float fadeSpeed = 1.5f;
	[SerializeField] private float fadeInSpeed = 2.5f;
	[SerializeField] private float fadeInSpeed2 = 2.5f;
	[SerializeField] private float FadeStartWait = 1.0f;
	[SerializeField] private float AudioStartAtAlpha = 0.4f;

	private Color newCol;

	private bool fadeDone = false;

	[SerializeField] private Camera mainCam;
	[SerializeField] private Vector3[] cameraPosition;
	private int posIndex = 0;

	[SerializeField] private GameObject textObj;
	[SerializeField] private string[] name;
	[SerializeField] private Vector3[] UIpos;

	void Start () 
	{
		newCol = FadeObj.color;
		mainCam.transform.position = cameraPosition[posIndex];
		StartCoroutine(StartCredits());
	}

	void Update ()
	{
		if (fadeDone) 
		{
			if (AudioManager.beatCheckEighth) 
			{
				StartCoroutine(FadeOnBeat());
			}
		}
	}

	IEnumerator StartCredits ()
	{
		yield return new WaitForSeconds(FadeStartWait);
		StartCoroutine(FadeStart());
		yield return null;
	}

	IEnumerator FadeStart ()
	{
		while(newCol.a >= AudioStartAtAlpha)
		{
			newCol.a = Mathf.Lerp(newCol.a, 0.0f, Time.deltaTime * fadeInSpeed);
			FadeObj.color = newCol;
			yield return null;
		}
		Debug.Log("StartAudio");
		AM.startAudioCoroutine(AM.songIndex);

		while(newCol.a >= 0.1f)
		{
			newCol.a = Mathf.Lerp(newCol.a, 0.0f, Time.deltaTime * fadeInSpeed2);
			FadeObj.color = newCol;
			yield return null;
		}
		newCol.a = 0.0f;
		FadeObj.color = newCol;
		Debug.Log("Fade In Done");
		fadeDone = true;
		yield return null;
	}

	IEnumerator FadeOnBeat ()
	{
		yield return StartCoroutine (FadeDown ());

		posIndex += 1;
		Debug.Log(cameraPosition.Length);
		if (posIndex > cameraPosition.Length - 1) 
		{
			posIndex = 0;
		}
		mainCam.transform.position = cameraPosition[posIndex];

		yield return StartCoroutine(FadeUp());
		yield return null;
	}

	IEnumerator FadeUp ()
	{
		while (newCol.a >= .1f) 
		{
			newCol.a = Mathf.Lerp (newCol.a, 0.0f, Time.deltaTime * fadeSpeed);
			FadeObj.color = newCol;
			//Debug.Log(newCol.a);
			yield return null;
		}
		Debug.Log("fade up");
		yield return null;
	}

	IEnumerator FadeDown()
	{
		while (newCol.a <= 0.995f) {
			newCol.a = Mathf.Lerp (newCol.a, 1.0f, Time.deltaTime * fadeSpeed);
			FadeObj.color = newCol;
			//Debug.Log(newCol.a);
			yield return null;
		}
		Debug.Log("fade down");
		yield return null;
	}
}
