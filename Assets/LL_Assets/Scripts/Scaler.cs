using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scaler : MonoBehaviour
{
    public GameObject[] bounds;

    public GameObject nightLayers;
    Bounds b;

    [SerializeField]
    float width;
    [SerializeField]
    float height;

    [SerializeField]
    float screenWidth, screenHeight;

    [SerializeField]
    bool didStart = false;

    float baseY = 0.0f;

    void Awake()
    {
        baseY = nightLayers.transform.position.y;

        bounds = new GameObject[6];

        bounds[0] = GameObject.Find("Top");
        bounds[1] = GameObject.Find("Bottom");
        bounds[2] = GameObject.Find("Left");
        bounds[3] = GameObject.Find("Right");
        //bounds[4] = GameObject.Find("Dragonfly Destroyer Left");
        //bounds[5] = GameObject.Find("Dragonfly Destroyer Right");
        bounds[4] = GameObject.Find("TopBorder");
        bounds[5] = GameObject.Find("BottomBorder");

        screenWidth = Screen.width;
        screenHeight = Screen.height;

        height = Camera.main.orthographicSize * 2.0f;

        width = height * (Screen.width / Screen.height);

        height = 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * -10.0f;
        width = height * Camera.main.aspect;

		float aspectHeight = 1;

		// 5:4
		if (Camera.main.aspect <= 1.3f)
		{
			aspectHeight = 4.0f;
		}
		// 4:3
		else if (Camera.main.aspect <= 1.4f)
		{
			aspectHeight = 3.0f;
		}
        // 3:2
		else if (Camera.main.aspect == 1.5f)
		{
			aspectHeight = 2.0f;
		}
		// 16:10
		else if (Camera.main.aspect == 1.6f)
		{
			//aspectHeight = 10.0f;
		}
		// 16:9
		else if (Camera.main.aspect <= 1.8f)
		{
			//aspectHeight = 9.0f;
		}

        nightLayers.transform.localScale = new Vector3(Mathf.Abs(width / 19.1f), Mathf.Abs(width / 19.1f), nightLayers.transform.localScale.z);

        nightLayers.transform.position = new Vector3(nightLayers.transform.position.x, baseY / aspectHeight, nightLayers.transform.position.x);
        //Debug.Log(Camera.main.aspect);
        //Debug.Log(aspectHeight);

        width = bounds[0].transform.localScale.y * Screen.width / Camera.main.orthographicSize;

        //Debug.Log("WxH: " + Screen.width + " x " + Screen.height);

        bounds[0].gameObject.transform.localScale = new Vector3(width, bounds[0].gameObject.transform.localScale.y, 1);
        bounds[1].gameObject.transform.localScale = new Vector3(width, bounds[1].gameObject.transform.localScale.y, 1);
        bounds[2].gameObject.transform.localScale = new Vector3(height / bounds[2].GetComponent<SpriteRenderer>().bounds.size.y + 15.0f, bounds[2].gameObject.transform.localScale.y, 1);
        bounds[3].gameObject.transform.localScale = new Vector3(height / bounds[3].GetComponent<SpriteRenderer>().bounds.size.y + 15.0f, bounds[3].gameObject.transform.localScale.y, 1);

        bounds[4].gameObject.transform.localScale = new Vector3(width, bounds[4].gameObject.transform.localScale.y, 1);
        bounds[5].gameObject.transform.localScale = new Vector3(width, bounds[5].gameObject.transform.localScale.y, 1);

        //bounds[6].gameObject.transform.localScale = new Vector3(width, bounds[6].gameObject.transform.localScale.y, 1);
        //bounds[7].gameObject.transform.localScale = new Vector3(width, bounds[7].gameObject.transform.localScale.y, 1);

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        b = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        bounds[0].gameObject.transform.position = new Vector3(bounds[0].gameObject.transform.position.x, b.max.y + 0.5f);
        bounds[1].gameObject.transform.position = new Vector3(bounds[1].gameObject.transform.position.x, b.min.y - 0.5f);
        bounds[2].gameObject.transform.position = new Vector3(b.min.x - 0.8f, bounds[2].gameObject.transform.position.y);
        bounds[3].gameObject.transform.position = new Vector3(b.max.x + 0.8f, bounds[3].gameObject.transform.position.y);

        //bounds[4].gameObject.transform.position = new Vector3(b.min.x - 10.0f, bounds[2].gameObject.transform.position.y);
        //bounds[5].gameObject.transform.position = new Vector3(b.max.x + 10.0f, bounds[3].gameObject.transform.position.y);

        //float spriteYTop = GameObject.Find("Frame 3-01").GetComponent<SpriteRenderer>().bounds.max.y * 1.43f;
        //float spriteYBottom = GameObject.Find("Frame 3-01").GetComponent<SpriteRenderer>().bounds.min.y * 1.35f;

        //bounds[6].gameObject.transform.localPosition = new Vector3(bounds[6].gameObject.transform.position.x, spriteYTop);
        //bounds[7].gameObject.transform.localPosition = new Vector3(bounds[7].gameObject.transform.position.x, spriteYBottom);

        //Vector3 convertedPosition = Vector3.zero;

        //convertedPosition = new Vector3(b.max.x - 3.5f, b.min.y + 1.0f, 1);

        //GameObject.Find("Destination").gameObject.transform.position = convertedPosition;

        didStart = true;
    }

    void Update()
    {
        if (didStart)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            height = Camera.main.orthographicSize * 2.0f;

            width = height * (Screen.width / Screen.height);

            height = 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * -10.0f;
            width = height * Camera.main.aspect;

            float aspectHeight = 1;

            // 5:4
            if (Camera.main.aspect <= 1.3f)
            {
                aspectHeight = 4.0f;
            }
            // 4:3
            else if (Camera.main.aspect <= 1.4f)
            {
                aspectHeight = 3.0f;
            }
            // 3:2
            else if (Camera.main.aspect == 1.5f)
            {
                aspectHeight = 2.0f;
            }
            // 16:10
            else if (Camera.main.aspect == 1.6f)
            {
                //aspectHeight = 10.0f;
            }
            // 16:9
            else if (Camera.main.aspect <= 1.8f)
            {
                //aspectHeight = 9.0f;
            }

            nightLayers.transform.localScale = new Vector3(Mathf.Abs(width / 19.1f), Mathf.Abs(width / 19.1f), nightLayers.transform.localScale.z);

            nightLayers.transform.position = new Vector3(nightLayers.transform.position.x, baseY / aspectHeight, nightLayers.transform.position.x);
            //Debug.Log(Camera.main.aspect);
            //Debug.Log(aspectHeight);

            width = bounds[0].transform.localScale.y * Screen.width / Camera.main.orthographicSize;

            //Debug.Log("WxH: " + Screen.width + " x " + Screen.height);

            bounds[0].gameObject.transform.localScale = new Vector3(width, bounds[0].gameObject.transform.localScale.y, 1);
            bounds[1].gameObject.transform.localScale = new Vector3(width, bounds[1].gameObject.transform.localScale.y, 1);
            bounds[2].gameObject.transform.localScale = new Vector3(height / bounds[2].GetComponent<SpriteRenderer>().bounds.size.y + 15.0f, bounds[2].gameObject.transform.localScale.y, 1);
            bounds[3].gameObject.transform.localScale = new Vector3(height / bounds[3].GetComponent<SpriteRenderer>().bounds.size.y + 15.0f, bounds[3].gameObject.transform.localScale.y, 1);

            bounds[4].gameObject.transform.localScale = new Vector3(width, bounds[4].gameObject.transform.localScale.y, 1);
            bounds[5].gameObject.transform.localScale = new Vector3(width, bounds[5].gameObject.transform.localScale.y, 1);

            //bounds[6].gameObject.transform.localScale = new Vector3(width, bounds[6].gameObject.transform.localScale.y, 1);
            //bounds[7].gameObject.transform.localScale = new Vector3(width, bounds[7].gameObject.transform.localScale.y, 1);

            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = Camera.main.orthographicSize * 2;
            b = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

            bounds[0].gameObject.transform.position = new Vector3(bounds[0].gameObject.transform.position.x, b.max.y + 0.5f);
            bounds[1].gameObject.transform.position = new Vector3(bounds[1].gameObject.transform.position.x, b.min.y - 0.5f);
            bounds[2].gameObject.transform.position = new Vector3(b.min.x - 0.8f, bounds[2].gameObject.transform.position.y);
            bounds[3].gameObject.transform.position = new Vector3(b.max.x + 0.8f, bounds[3].gameObject.transform.position.y);
        }
    }

    public void ResizeObjectToBounds(SpriteRenderer thing2Resize)
    {
        height = Camera.main.orthographicSize * 2.0f;

        width = height / Screen.height * Screen.width;

        thing2Resize.gameObject.transform.localScale = new Vector3(width / thing2Resize.sprite.bounds.size.x, width / thing2Resize.bounds.size.x, 1);
    }

    public Bounds GetBounds()
    {
        return b;
    }
}
