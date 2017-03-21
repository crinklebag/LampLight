using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    TODO:
    Turn off all of the buttons once a bg is selected
    turn on all the buttons if the player decides to go back
    stop moving the location image - only move the text overlay
    shrink when player decides they want to pick dif bg
    set a variable that will select which bg the player has chosen
    load appropriate scene
*/

public class StretchUIMask : MonoBehaviour {

    MainMenuController mainMenuController;

    [SerializeField] int orderInLayer;
    [SerializeField] int sceneNumber;
    [SerializeField] GameObject buttons;

    bool selected = false;

    Vector3 maskStartPos;
    Vector3 maskEndPos;
    Vector2 maskStartSize;
    Vector2 maskEndSize;
    Vector3 imageStartPos;
    Vector3 imageEndPos;

    // Canvas size variables
    float startX = 0;
    float startWidth = 0;
    float endX = 0;
    float endWidth = 1920;
    float imageStartX = 0;
    float imageEndX = 960;

    // Growth Variables
    float currentTime = 0;
    float travelTime = 0.5f;
    float normalizedValue;

	// Use this for initialization
	void Start () {
        mainMenuController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainMenuController>();
		startX = this.GetComponent<RectTransform>().localPosition.x;
        startWidth = this.GetComponent<RectTransform>().rect.width;
        imageStartX = /* this.transform.GetChild(0).GetComponent<RectTransform>().localPosition.x*/ 480;
        maskStartPos = new Vector3(startX, 0, 0);
        maskEndPos = new Vector3(endX, 0, 0);
        maskStartSize = new Vector2(startWidth, 0);
        maskEndSize = new Vector2(endWidth, 0);
        imageStartPos = new Vector3(imageStartX, 0, 0);
        imageEndPos = new Vector3(imageEndX, 0, 0);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ChooseLocation();
        }

        // Debug.Log(mainMenuController.GetCurrentState());

        if (mainMenuController.GetCurrentState() == 2) {
            buttons.SetActive(true);
        } else {
            buttons.SetActive(false);
        }
    }

    public void ChooseLocation() {
        StopAllCoroutines();
        currentTime = 0;
        StartCoroutine("EnlargeObject");
        selected = true;
        this.GetComponent<Canvas>().sortingOrder = 0;

        // Set the player Pref
        PlayerPrefs.SetInt("bgNumber", sceneNumber);
    }

    public void DeselectLocation() {
        currentTime = 0;
        // Debug.Log("Deselect");
        StopAllCoroutines();
        StartCoroutine("ShrinkObject");
        selected = false;

        // Re-Set the player Pref
        PlayerPrefs.SetInt("bgNumber", -1);
    }

    IEnumerator EnlargeObject() {
        while (currentTime <= travelTime) {
            currentTime += Time.deltaTime;
            // When it is finsihed - call update menu 
            if (currentTime >= travelTime) {
                mainMenuController.UpdateMenuState(true);
            }
            normalizedValue = currentTime / travelTime;
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(maskStartPos, maskEndPos, normalizedValue);
            this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(imageStartPos, imageEndPos, normalizedValue);
            // this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = imageEndPos;
            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(maskStartSize, maskEndSize, normalizedValue);
            yield return null;
        }
    }

    IEnumerator ShrinkObject() {
        // Debug.Log("Please Shrink!");
        while (currentTime <= travelTime) {
            // Debug.Log("Shrinking");
            currentTime += Time.deltaTime;
            if (currentTime >= travelTime) {
                this.GetComponent<Canvas>().sortingOrder = orderInLayer;
            }
            normalizedValue = currentTime / travelTime;
            this.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(maskEndPos, maskStartPos, normalizedValue);
            this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(imageEndPos, imageStartPos, normalizedValue);
            this.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(maskEndSize, maskStartSize, normalizedValue);
            yield return null;
        }
    }

    public bool IsSelected() {
        return selected;
    }
}
