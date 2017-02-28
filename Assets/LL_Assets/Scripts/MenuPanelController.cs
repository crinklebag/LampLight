using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelController : MonoBehaviour {

    [SerializeField] float panelNumber;

	// Use this for initialization
	void Start () {
        FitToScreenSize();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FitToScreenSize() {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();

        transform.localScale = Vector3.one;
    }

    void Position() {

    }
}
