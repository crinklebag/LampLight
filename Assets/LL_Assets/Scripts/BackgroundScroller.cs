using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundScroller : MonoBehaviour
{
    public SpriteRenderer nightTree;
    public SpriteRenderer sunriseTree;

    [SerializeField]
    private float y;

    public GameObject parent;
    public GameObject audioPeer;
    public GameObject[] bounds;

    [SerializeField]
    private bool setHeight = false;
    [SerializeField]
    private bool startScroll = false;

    private Color32 transparentColor = new Color32(255, 255, 255, 0);

    [SerializeField]
    private Color32 treeColorBar;

    [SerializeField]
    private Vector4 treeColor;

    [SerializeField]
    private float clipLength;

    [SerializeField]
    private float startTime;

    [SerializeField]
    private float startTime4Tree;

    [SerializeField]
    bool finishedTreeCoroutine = false;
    [SerializeField]
    bool startTreeCoroutine = false;

    [SerializeField]
    float treeYVal = 0;
    [SerializeField]
    float treeYValCoroutine = 0;

    Bounds b;
    float width;
    float height;

    [SerializeField]
    float timeMult = 0;

    void Start()
    {
        bounds = new GameObject[6];

        bounds[0] = GameObject.Find("Top");
        bounds[1] = GameObject.Find("Bottom");
        bounds[2] = GameObject.Find("Left");
        bounds[3] = GameObject.Find("Right");
        bounds[4] = GameObject.Find("Dragonfly Destroyer Left");
        bounds[5] = GameObject.Find("Dragonfly Destroyer Right");

        audioPeer = GameObject.Find("AudioPeer");

        width = -transform.localScale.y * Screen.width / Screen.height;

        transform.localScale = new Vector3(width, transform.localScale.y, 1);

        height = Camera.main.orthographicSize * 2.0f;

        width = height / Screen.height * Screen.width;

        nightTree.gameObject.transform.localScale = new Vector3(width / nightTree.sprite.bounds.size.x, width / nightTree.bounds.size.x, 1);
        sunriseTree.gameObject.transform.localScale = new Vector3(width / sunriseTree.sprite.bounds.size.x, width / sunriseTree.bounds.size.x, 1);

        width = bounds[0].transform.localScale.y * Screen.width / 10.0f;

        //Debug.Log("WxH: " + Screen.width + " x " + Screen.height);

        bounds[0].gameObject.transform.localScale = new Vector3(width, bounds[0].gameObject.transform.localScale.y, 1);
        bounds[1].gameObject.transform.localScale = new Vector3(width, bounds[1].gameObject.transform.localScale.y, 1);
        bounds[2].gameObject.transform.localScale = new Vector3(height / bounds[2].GetComponent<SpriteRenderer>().bounds.size.y + 10.0f, bounds[2].gameObject.transform.localScale.y, 1);
        bounds[3].gameObject.transform.localScale = new Vector3(height / bounds[3].GetComponent<SpriteRenderer>().bounds.size.y + 10.0f, bounds[3].gameObject.transform.localScale.y, 1);

        bounds[4].gameObject.transform.localScale = new Vector3(width, bounds[4].gameObject.transform.localScale.y, 1);
        bounds[5].gameObject.transform.localScale = new Vector3(width, bounds[5].gameObject.transform.localScale.y, 1);

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        b = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        bounds[0].gameObject.transform.position = new Vector3(bounds[0].gameObject.transform.position.x, b.max.y + 0.5f);
        bounds[1].gameObject.transform.position = new Vector3(bounds[1].gameObject.transform.position.x, b.min.y - 0.5f);
        bounds[2].gameObject.transform.position = new Vector3(b.min.x - 0.5f, bounds[2].gameObject.transform.position.y);
        bounds[3].gameObject.transform.position = new Vector3(b.max.x + 0.5f, bounds[3].gameObject.transform.position.y);

        bounds[4].gameObject.transform.position = new Vector3(b.min.x - 10.0f, bounds[2].gameObject.transform.position.y);
        bounds[5].gameObject.transform.position = new Vector3(b.max.x + 10.0f, bounds[3].gameObject.transform.position.y);

        clipLength = audioPeer.GetComponent<AudioSource>().clip.length ;

        //clipLength = 10;

        Vector3 convertedPosition = Vector3.zero;

        convertedPosition = new Vector3(b.max.x - 3.5f, b.min.y + 1.0f, 1);

        GameObject.Find("Destination").gameObject.transform.position = convertedPosition;

        //convertedPosition = Camera.main.WorldToScreenPoint( GameObject.Find("Jar").GetComponent<RectTransform>().transform.TransformPoint(GameObject.Find("Jar").GetComponent<RectTransform>().transform.position));

        //Debug.Log(convertedPosition);

        //Debug.Log(Camera.main.ScreenToWorldPoint(GameObject.Find("Jar").gameObject.transform.position));

        Reset(clipLength);
    }

    void Update()
    {
        if (clipLength != 0 && !setHeight)
        {
            // scale height based on the scrollSpeed (music length)
            // this also depends on the gradient (how long each colored section is)
            float height = transform.localScale.y * clipLength;

            // scale parent so the bg top stays at the current position
            parent.transform.localScale = new Vector3(parent.transform.localScale.x, height, 1);

            setHeight = true;
        }

        if (startScroll)
        {
            treeYVal = -y / 2;
             treeYValCoroutine = treeYVal + startTime4Tree;

            startTime = audioPeer.GetComponent<AudioSource>().time;

            y = startTime / -clipLength;

            Vector2 offset = new Vector2(0, y);
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

            if (-y > 0.65f && !startTreeCoroutine)
            {
                StartCoroutine(TreeYCoroutine(treeYVal, treeYValCoroutine));
                startTreeCoroutine = true;
            }

            if (!startTreeCoroutine)
            {
                nightTree.color = Color32.Lerp(Color.white, transparentColor, treeYVal);
                //Debug.Log("IN ELSE ");
                //Debug.Log(temp2);
            }
            else if (finishedTreeCoroutine)
            {
                timeMult = (-y * treeYValCoroutine) + (treeYVal * -y) - ((startTime4Tree / 4) * ((-y / startTime4Tree) - 2.0f));

                nightTree.color = Color32.Lerp(Color.white, transparentColor, timeMult);
                //Debug.Break();
                //Debug.Log("IN ELSE 2");
                //Debug.Log(temp2 + (temp2 * temp));
            }

            treeColor = nightTree.color;
            treeColorBar = nightTree.color;
        }

        if (y <= -0.999f && startScroll)
        {
            GameObject.Find("GameController").GetComponent<GameController>().FinishGameTime();
            startScroll = false;
            //Debug.Log("Called GAMECONTROLLER FinishGameTime");
            return;
        }

        //Debug.Log(tree.color);
    }

    public void Reset(float cl)
    {
        //Debug.Log("Called reset to play song: " + audioPeer.GetComponent<AudioSource>().clip.name + " with length: " + cl);

        startScroll = false;
        setHeight = false;

        clipLength = cl;
        startTime = 0;

        nightTree.color = Color.white;

        Vector2 offset = new Vector2(0, 0);
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

        startScroll = true;
    }

    IEnumerator TreeYCoroutine(float temp, float temp2)
    {
        if (nightTree.color.a > -(y + 0.15f))
        {
            //Debug.Break();

            startTime4Tree += Time.fixedDeltaTime / 5;
            temp2 = temp + startTime4Tree;

            nightTree.color = Color32.Lerp(Color.white, transparentColor, temp2);

            //Debug.Log("IN IF ");

            //yield return new WaitForSeconds(startTime4Tree);
            yield return new WaitForSecondsRealtime(startTime4Tree);

            StartCoroutine(TreeYCoroutine(temp, temp2));
        }
        else
        {
            finishedTreeCoroutine = true;
        }

        yield return new WaitForFixedUpdate();
    }

    public void ResizeObjectToBounds(SpriteRenderer thing2Resize)
    {
        height = Camera.main.orthographicSize * 2.0f;

        width = height / Screen.height * Screen.width;

        thing2Resize.gameObject.transform.localScale = new Vector3(width / thing2Resize.sprite.bounds.size.x, width / thing2Resize.bounds.size.x, 1);
    }
}
