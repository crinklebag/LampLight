using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBug : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        StartCoroutine("Flicker");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Flicker() {
        yield return new WaitForSeconds(0.1f);

        Color tempColor = this.GetComponent<Image>().color;
        float newAlpha = Random.Range(0.5f, 1.0f);
        // Debug.Log("Flickering: " + newAlpha);
        tempColor.a = newAlpha;
        this.GetComponent<Image>().color = tempColor;

        StartCoroutine("Flicker");
    }
    
}
