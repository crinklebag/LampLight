using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Light))]
[DisallowMultipleComponent]
public class LightController : MonoBehaviour
{

	private GameObject gameController;

    public GameObject audioManager;
    public GameObject godRod1;
    public GameObject godRod2;

    public Color godRod1Color;
    public Color godRod2Color;
    bool startGodRays = false;

    private Light lightOfMyLife;

    [Header("Scene Lighting Variables")]
    [SerializeField]
    Color nightLightColour;
    [SerializeField]
    Color dayLightColour;
    [Tooltip("Duration in seconds.")]
    [SerializeField]
    float duration = 20;
    float lightLerpControl = 0;

    [Header("Night Sky Variables")]
    [SerializeField]
    GameObject skyMaterial;
    [SerializeField]
    Color nightSkyColor;
    [SerializeField]
    Color daySkyColor;

    [Header("God Ray Variables")]
    [SerializeField]
    private Color32 previousColorGodRod1 = Color.clear;
    [SerializeField]
    private Color32 currentColorGodRod1 = Color.white;
    [SerializeField]
    private Color32 previousColorGodRod2 = Color.clear;
    [SerializeField]
    private Color32 currentColorGodRod2 = Color.white;

    [SerializeField]
    private float lerpColorTimeGodRod1 = 0;
    [SerializeField]
    private float lerpColorTimeGodRod2 = 0;
    [SerializeField]
    private float lerpColorTimeGodRod1Duration = 10;
    [SerializeField]
    private float lerpColorTimeGodRod2Duration = 15;

    [SerializeField]
    private float intensity = 0;
    [SerializeField]
    private float maxIntensity = 0.15f;
    [SerializeField]
    private float clipLength = 0;
    [SerializeField]
    private float startTime = 0;

    [SerializeField]
    private float lerpPositionRandomNumber1 = 60;
    [SerializeField]
    private float lerpPositionRandomNumber2 = 60;

    [SerializeField]
    private byte godRodLimit = 150;

    [SerializeField]
    private bool setAlready1 = false;
    [SerializeField]
    private bool setAlready2 = false;
    [SerializeField]
    private bool startGame = false;

    [SerializeField]
    private Bounds b;

    [SerializeField]
    private bool endSong = false;

    //Dumb pause check bool
    private bool isPaused = false;

    // Use this for initialization
    void Start()
    {
        lightOfMyLife = GetComponent<Light>();

        //clipLength = audioManager.GetComponent<AudioSource>().clip.length;

        godRod1Color = godRod1.GetComponent<MeshRenderer>().material.color;
        godRod2Color = godRod2.GetComponent<MeshRenderer>().material.color;

        previousColorGodRod1 = godRod1Color;
        previousColorGodRod2 = godRod2Color;

        //   b = GameObject.FindGameObjectWithTag("Scaler").GetComponent<Scaler>().GetBounds();

        lerpPositionRandomNumber1 = Random.Range(clipLength / 4.0f, clipLength);
        lerpPositionRandomNumber2 = Random.Range(clipLength / 4.0f, clipLength);

        gameController = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
    	isPaused = gameController.GetComponent<PauseController>().isPaused;

        if (startGame && !isPaused)
        {
            lightOfMyLife.color = Color.Lerp(nightLightColour, dayLightColour, lightLerpControl);
            skyMaterial.GetComponent<MeshRenderer>().material.color = Color.Lerp(nightSkyColor, daySkyColor, lightLerpControl);
            if (lightLerpControl < 1)
            {
                lightLerpControl += Time.deltaTime / duration;
                if (lightLerpControl > 0.6f && SceneManager.GetActiveScene().name != "Beach")
                {
                    // Start God Rays
                    startGodRays = true;
                }
            }

            if (startGodRays)
            {
                if (!godRod1.activeSelf)
                {
                    godRod1.SetActive(true);
                    godRod2.SetActive(true);
                }
                CheckGodRod(1);
                CheckGodRod(2);
            }
        }
    }

    void CheckGodRod(int val)
    {
        switch (val)
        {
            case 1:
                {
                    if (lerpColorTimeGodRod1 >= 1.0f && !setAlready1)
                    {
                        //Debug.Log("Set alpha to godRodLimit");
                        previousColorGodRod1 = godRod1Color;
                        currentColorGodRod1.a = godRodLimit;
                        lerpColorTimeGodRod1 = 0;
                        lerpColorTimeGodRod1Duration = Random.Range(5.0f, 10.0f);
                        setAlready1 = true;
                    }
                    else if (lerpColorTimeGodRod1 >= 1.0f && setAlready1)
                    {
                        //Debug.Log("Set alpha to 0");
                        previousColorGodRod1 = godRod1Color;
                        currentColorGodRod1.a = 0;
                        lerpColorTimeGodRod1 = 0;
                        lerpColorTimeGodRod1Duration = Random.Range(5.0f, 10.0f);
                        setAlready1 = false;
                    }

                    lerpColorTimeGodRod1 += (Time.deltaTime / lerpColorTimeGodRod1Duration);

                    godRod1Color = Color32.Lerp(previousColorGodRod1, currentColorGodRod1, lerpColorTimeGodRod1);
                    godRod1.GetComponent<MeshRenderer>().material.color = godRod1Color;
                }
                break;
            case 2:
                {
                    if (lerpColorTimeGodRod2 >= 1.0f && !setAlready2)
                    {
                        //Debug.Log("Set alpha to godRodLimit");
                        previousColorGodRod2 = godRod2Color;
                        currentColorGodRod2.a = godRodLimit;
                        lerpColorTimeGodRod2 = 0;
                        lerpColorTimeGodRod2Duration = Random.Range(5.0f, 10.0f);
                        setAlready2 = true;
                    }
                    else if (lerpColorTimeGodRod2 >= 1.0f && setAlready2)
                    {
                        //Debug.Log("Set alpha to 0");
                        previousColorGodRod2 = godRod2Color;
                        currentColorGodRod2.a = 0;
                        lerpColorTimeGodRod2 = 0;
                        lerpColorTimeGodRod2Duration = Random.Range(5.0f, 10.0f);
                        setAlready2 = false;
                    }

                    lerpColorTimeGodRod2 += (Time.deltaTime / lerpColorTimeGodRod2Duration);

                    godRod2Color = Color32.Lerp(previousColorGodRod2, currentColorGodRod2, lerpColorTimeGodRod2);
                    godRod2.GetComponent<MeshRenderer>().material.color = godRod2Color;
                }
                break;
            default:
                break;
        }
    }

    void MoveGodRods()
    {
        float lerpPositionTime1 = (Time.deltaTime / lerpPositionRandomNumber1);
        float lerpPositionTime2 = (Time.deltaTime / lerpPositionRandomNumber2);

        float lerpPositionX1 = Mathf.Lerp(godRod1.transform.position.x, b.max.x, lerpPositionTime1);
        float lerpPositionX2 = Mathf.Lerp(godRod2.transform.position.x, b.max.x, lerpPositionTime2);

        godRod1.transform.position = new Vector3(lerpPositionX1, godRod1.transform.position.y, godRod1.transform.position.z);
        godRod2.transform.position = new Vector3(lerpPositionX2, godRod2.transform.position.y, godRod2.transform.position.z);

        if (lerpPositionX1 >= b.max.x - 1)
        {
            lerpPositionRandomNumber1 = Random.Range(clipLength / 4.0f, clipLength);
            godRod1.transform.position = new Vector3(b.min.x * 2, godRod1.transform.position.y, godRod1.transform.position.z);
        }

        if (lerpPositionX2 >= b.max.x - 1)
        {
            lerpPositionRandomNumber2 = Random.Range(clipLength / 4.0f, clipLength);
            godRod2.transform.position = new Vector3(b.min.x * 2, godRod2.transform.position.y, godRod2.transform.position.z);
        }
    }

    void RotateLight()
    {
        float lerpRotate = Mathf.LerpAngle(transform.eulerAngles.y, -40, (Time.deltaTime / (clipLength / 4.0f)));
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, lerpRotate, transform.eulerAngles.z);

        float lerpRotateGodRod1 = Mathf.LerpAngle(godRod1.transform.eulerAngles.z, -40, (Time.deltaTime / (clipLength / 4.0f)));
        float lerpRotateGodRod2 = Mathf.LerpAngle(godRod2.transform.eulerAngles.z, -60, (Time.deltaTime / (clipLength / 4.0f)));

        godRod1.transform.eulerAngles = new Vector3(godRod1.transform.eulerAngles.x, godRod1.transform.eulerAngles.y, lerpRotateGodRod1);
        godRod2.transform.eulerAngles = new Vector3(godRod2.transform.eulerAngles.x, godRod2.transform.eulerAngles.y, lerpRotateGodRod2);
    }

    public void SetGame()
    {
        clipLength = audioManager.GetComponent<AudioSource>().clip.length;
        startGame = true;
    }
}
