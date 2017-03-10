using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    private bool loadScene = false;

    [SerializeField]
    private string scene;
    public Text loadingText;
    public Text[] loadingDots;
    public Image overlay;

    private int something = 0;


    // Updates once per frame
    void Update()
    {
        if (loadScene == true)
        {
            //loadingText.gameObject.transform.Rotate(-Vector3.forward * 60.0f * Time.deltaTime);
        }
    }

    IEnumerator LoadNewScene()
    {
        yield return new WaitForSeconds(0);

        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        while (!async.isDone)
        {
            Debug.Log("bleh");

            yield return new WaitForFixedUpdate();
        }

    }

    public void LoadScene(string name)
    {
        scene = name;
        loadScene = true;

        overlay.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);

        StartCoroutine(LoadingThing());
        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadingThing()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        something++;

        if (something > 3)
        {
            something = 0;
        }

        switch (something)
        {
            case 0:
                loadingDots[0].gameObject.SetActive(false);
                loadingDots[1].gameObject.SetActive(false);
                loadingDots[2].gameObject.SetActive(false);
                break;
            case 1:
                loadingDots[0].gameObject.SetActive(true);
                loadingDots[1].gameObject.SetActive(false);
                loadingDots[2].gameObject.SetActive(false);
                break;
            case 2:
                loadingDots[0].gameObject.SetActive(true);
                loadingDots[1].gameObject.SetActive(true);
                loadingDots[2].gameObject.SetActive(false);
                break;
            case 3:
                loadingDots[0].gameObject.SetActive(true);
                loadingDots[1].gameObject.SetActive(true);
                loadingDots[2].gameObject.SetActive(true);
                break;
            default:
                break;
        }

        StartCoroutine(LoadingThing());
    }
}
