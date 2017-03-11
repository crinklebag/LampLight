using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilBugGlow : MonoBehaviour {

	private float glowSize = 0.0f;
	[SerializeField] float glowSpeed = 5.0f;

	void Awake ()
	{
		this.GetComponent<ParticleSystemRenderer>().minParticleSize = 0.0f;
	}

	void Update ()
	{
		if (AudioManager.beatCheck) 
		{
			StartCoroutine(GlowCycle());
		}	
	}

	IEnumerator GlowCycle ()
	{
		yield return StartCoroutine(GlowUp());
		yield return StartCoroutine(GlowDown());

	}

	IEnumerator GlowUp ()
	{
		while (glowSize < 0.09f) 
		{
			glowSize = Mathf.MoveTowards(glowSize, 0.09f, Time.deltaTime * glowSpeed);
			this.GetComponent<ParticleSystemRenderer>().minParticleSize = glowSize;
			yield return null;
		}
		yield return null;
	}

	IEnumerator GlowDown ()
	{
		while (glowSize > 0.0f) 
		{
			glowSize = Mathf.MoveTowards(glowSize, 0.0f, Time.deltaTime * glowSpeed);
			this.GetComponent<ParticleSystemRenderer>().minParticleSize = glowSize;
			yield return null;
		}
		yield return null;
	}


}
