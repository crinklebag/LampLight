using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadDelay());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadLevel() {
        //Load selected scene
        switch (PlayerPrefs.GetInt("bgNumber"))
        {
            case 1:
                SceneManager.LoadScene("Main_Mobile_DeepForest");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                break;
            case 2:
                SceneManager.LoadScene("Main_Mobile_DeepForest");
                // SceneManager.LoadScene("Main_Mobile");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile");
                break;
            case 3:
                SceneManager.LoadScene("Main_Mobile_Waterfall");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                break;
            case 4:
                SceneManager.LoadScene("Main_Mobile_DeepForest");
                //GetComponent<LoadingScreen>().LoadScene("Main_Mobile_DeepForest");
                break;
        }
    }

    IEnumerator LoadDelay() {
        yield return new WaitForSeconds(7);

        LoadLevel();

    }
}
