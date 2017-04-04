using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCredits : MonoBehaviour {

	[SerializeField] private int numOfPanels;

	[SerializeField] private Image[] img;
	[SerializeField] private GameObject scrollingTxt;
	[SerializeField] private GameObject lamplightAnimation;
	[SerializeField] private SpriteRenderer lamplightAnimImage;

	[SerializeField] private float textFadeInSpeed = 5.0f;
	[SerializeField] private float textFadeOutSpeed = 5.0f;
	[SerializeField] private float scrollingSpeed = 2.5f;

	[SerializeField] private float endHeight = 1000.0f;

	[SerializeField] private float animWaitTime = 2.45f;

	void Awake()
	{
		for(int i = 0; i < numOfPanels; i++)
		{
			img[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			img[i].gameObject.SetActive(false);
		}

		lamplightAnimImage.gameObject.SetActive(false);
		lamplightAnimation.SetActive(false);

		StartCoroutine(runCredits());
	}

	IEnumerator runCredits()
	{
		yield return StartCoroutine(waitForAnimation());
		yield return StartCoroutine(fadeAnimPanel());
		yield return StartCoroutine(fadePanels());
		yield return StartCoroutine(rollTheRest());

		this.GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");

		yield return null;
	}

	IEnumerator waitForAnimation()
	{
		lamplightAnimation.SetActive(true);

		yield return new WaitForSeconds(animWaitTime);
		/*while(!lamplightAnimation.GetComponent<IntroAnimController>().animIsDone)
		{
			Debug.Log("Waiting");
			yield return null;
		}*/
		lamplightAnimation.SetActive(false);

		//lamplightAnimation.SetActive(false);

		yield return null;
	}

	IEnumerator fadeAnimPanel()
	{
		lamplightAnimImage.gameObject.SetActive(true);

		float tempAlpha = lamplightAnimImage.color.a;

		while(tempAlpha > 0.001f)
		{
			tempAlpha = Mathf.MoveTowards(tempAlpha, 0.0f, Time.deltaTime * textFadeOutSpeed);
			lamplightAnimImage.color = new Color(1.0f, 1.0f, 1.0f, tempAlpha);
			yield return null;
		}

		lamplightAnimImage.gameObject.SetActive(false);

		yield return null;
	}

	IEnumerator fadePanels()
	{
		for(int i = 0; i < numOfPanels; i++)
		{
			yield return StartCoroutine(fadeInPanel(i));

			yield return StartCoroutine(fadeOutPanel(i));
		}
		
		yield return null;
	}

	IEnumerator fadeInPanel(int index)
	{
		img[index].gameObject.SetActive(true);

		float tempAlpha = img[index].color.a;

		while(tempAlpha < 0.999f)
		{
			tempAlpha = Mathf.MoveTowards(tempAlpha, 1.0f, Time.deltaTime * textFadeInSpeed);
			img[index].color = new Color(1.0f, 1.0f, 1.0f, tempAlpha);
			yield return null;
		}

		img[index].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

		yield return null;
	}

	IEnumerator fadeOutPanel(int index)
	{
		float tempAlpha = img[index].color.a;

		while(tempAlpha > 0.001f)
		{
			tempAlpha = Mathf.MoveTowards(tempAlpha, 0.0f, Time.deltaTime * textFadeInSpeed);
			img[index].color = new Color(1.0f, 1.0f, 1.0f, tempAlpha);
			yield return null;
		}

		img[index].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

		yield return null;
	}

	IEnumerator rollTheRest()
	{
		scrollingTxt.SetActive(true);

		float posY = scrollingTxt.transform.localPosition.y;

		while(posY < endHeight - 0.001f)
		{
			posY = Mathf.MoveTowards(posY, endHeight, Time.deltaTime * scrollingSpeed);
			scrollingTxt.transform.localPosition = new Vector3(0.0f, posY, Time.deltaTime * scrollingSpeed);
			yield return null;
		}

		yield return null;
	}

}
