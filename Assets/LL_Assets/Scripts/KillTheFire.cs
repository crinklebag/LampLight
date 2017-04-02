using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTheFire : MonoBehaviour {

	private AudioManager AM;
	[SerializeField] private Light fireLight;
	private float percentagePassed = 0.0f;
	[SerializeField] private float fadeSpeed = 5.0f;
	private bool hasFaded = false;

	void Start () 
	{
		AM = GameObject.FindGameObjectWithTag("AM").GetComponent<AudioManager>();
	}

	void Update () 
	{
		percentagePassed = AM.GetComponent<AudioSource>().time / AM.GetComponent<AudioSource>().clip.length;
		if(percentagePassed > 0.75f && !hasFaded)
		{
			hasFaded = true;
			StartCoroutine(FadeLight());
		}
	}

	IEnumerator FadeLight()
	{
		float range = fireLight.range;

		while(range > 9.0f)
		{
			range = Mathf.MoveTowards(range, 0.0f, Time.deltaTime * fadeSpeed);
			fireLight.range = range;
			yield return null;
		}

		this.GetComponent<ParticleSystem>().Stop();

		while(range > 5.0f)
		{
			range = Mathf.MoveTowards(range, 0.0f, Time.deltaTime * (fadeSpeed * 1.5f));
			fireLight.range = range;
			yield return null;
		}

		while(range > 0.0f)
		{
			range = Mathf.MoveTowards(range, 0.0f, Time.deltaTime * (fadeSpeed * 1.5f));
			fireLight.intensity = Mathf.MoveTowards(fireLight.intensity, 0.0f, Time.deltaTime * 0.5f);
			fireLight.range = range;
			yield return null;
		}

		yield return null;
	}
}
