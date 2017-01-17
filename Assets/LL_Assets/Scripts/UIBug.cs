using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIBug : MonoBehaviour
{

    [SerializeField]
    string number;

    // Use this for initialization
    void Start()
    {
        StartCoroutine("Flicker");
        number = gameObject.name.Substring(gameObject.name.IndexOf('(') + 1, 1);

        Debug.Log(number);
    }


    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Flicker()
    {
        yield return new WaitForSeconds(0.1f);

        Color tempColor = this.GetComponent<Image>().color;

        float newAlpha = AudioPeer._audioBandBuffer[GameController.bandFrequencies[int.Parse(number)]];

        tempColor = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, newAlpha);

        this.GetComponent<Image>().color = tempColor;

        StartCoroutine("Flicker");
    }

    void OnEnable()
    {
        StartCoroutine("Flicker");
    }
}
