using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("Flickering");
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Flickering() {
        yield return new WaitForSeconds(0.1f);

        Color tempColor = this.GetComponent<SpriteRenderer>().color;
        float newAlpha = Random.Range(0.5f, 1.0f);
        // Debug.Log("Flickering: " + newAlpha);
        tempColor.a = newAlpha;
        this.GetComponent<SpriteRenderer>().color = tempColor;

        StartCoroutine("Flickering");
    }
}
