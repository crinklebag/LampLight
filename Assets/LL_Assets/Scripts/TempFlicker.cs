using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFlicker : MonoBehaviour {

    [SerializeField] SpriteRenderer bugImage;

	// Use this for initialization
	void Start () {

        StartCoroutine(FlickaTheHorse());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator FlickaTheHorse() {
        yield return new WaitForSeconds(0.1f);

        float randIntensity = Random.Range(0, this.GetComponent<Light>().intensity);

        this.GetComponent<Light>().intensity = randIntensity;

        Color temp = Color.white;
        temp.a = randIntensity;
        bugImage.color = temp;

        StartCoroutine(FlickaTheHorse());
    }
}
