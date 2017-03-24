using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunFactController : MonoBehaviour {

	private string[] facts;
	private int numOfFacts = 5;

	[SerializeField] Text factsText;
    [SerializeField]
    float waitTime = 7.0f;

	void Awake()
	{
		facts = new string[numOfFacts];
		facts[0] = "this is fact 1";
		facts[1] = "this is fact 2";
		facts[2] = "this is fact 3";
		facts[3] = "this is fact 4";
		facts[4] = "this is fact 5";
	}

	void Start () 
	{
		factsText.text = facts[Random.Range(0, facts.Length)];

		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(waitTime);
        this.GetComponent<SceneLoad>().LoadScene("MainMenu_Mobile");
	}
}
