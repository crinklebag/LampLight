using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedPanelController : MonoBehaviour {

    float speed = 4;

    int degreeCounter  = 0;
    int maxDegrees = 10;

    bool canShake = false;
    bool shakeRight = true;
    bool stopShake = false;

    Vector3 originalRotation;

    // Use this for initialization
    void Start () {
		originalRotation = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		if (canShake) {
            if (shakeRight) {
                ShakeRight();
            } else {
                ShakeLeft();
            }
        }
	}

    void ShakeLeft() {
        if (degreeCounter < maxDegrees) {
            transform.Rotate(-Vector3.forward * speed);
            degreeCounter++;
        }
        else {
            stopShake = true;
            shakeRight = true;
            degreeCounter = 0;
        }
    }

    void ShakeRight() {
        if (degreeCounter < maxDegrees / 2) {
            transform.Rotate(Vector3.forward * speed);
            degreeCounter++;
        } else {
            degreeCounter = 0;
            if (stopShake) { canShake = false; }
            else { shakeRight = false; }
        }
    }

    public void ShakeLock() {
        Debug.Log("Shaking Lock?");
        degreeCounter = 0;
        shakeRight = true;
        stopShake = false;
        canShake = true;
        transform.eulerAngles = originalRotation;
    }
}
