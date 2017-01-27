using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundScroller : MonoBehaviour
{
    public SpriteRenderer tree;

    [SerializeField]
    private float y;

    public GameObject parent;
    public GameObject audioPeer;
    public GameObject[] bounds;

    private bool setHeight = false;
    private bool startScroll = false;

    private Color32 darkGrey = new Color32(25, 25, 25, 255);

    [SerializeField]
    private Color32 treeColorBar;

    [SerializeField]
    private Vector4 treeColor;

    [SerializeField]
    private float clipLength;

    [SerializeField]
    private float startTime;

    void Start()
    {
        bounds = new GameObject[4];

        bounds[0] = GameObject.Find("Top");
        bounds[1] = GameObject.Find("Bottom");
        bounds[2] = GameObject.Find("Left");
        bounds[3] = GameObject.Find("Right");

        audioPeer = GameObject.Find("AudioPeer");

        float width = -transform.localScale.y * Screen.width / Screen.height;

        transform.localScale = new Vector3(width, transform.localScale.y, 1);

        float height = Camera.main.orthographicSize * 2.0f;

        width = height / Screen.height * Screen.width;

        tree.gameObject.transform.localScale = new Vector3(width / tree.sprite.bounds.size.x, width / tree.bounds.size.x, 1);

        width = bounds[0].transform.localScale.y * Screen.width / 10.0f;

        //Debug.Log("WxH: " + Screen.width + " x " + Screen.height);

        bounds[0].gameObject.transform.localScale = new Vector3(width, bounds[0].gameObject.transform.localScale.y, 1);
        bounds[1].gameObject.transform.localScale = new Vector3(width, bounds[1].gameObject.transform.localScale.y, 1);
        bounds[2].gameObject.transform.localScale = new Vector3(height / bounds[2].GetComponent<SpriteRenderer>().bounds.size.y + 10.0f, bounds[2].gameObject.transform.localScale.y, 1);
        bounds[3].gameObject.transform.localScale = new Vector3(height / bounds[3].GetComponent<SpriteRenderer>().bounds.size.y + 10.0f, bounds[3].gameObject.transform.localScale.y, 1);

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        Bounds b = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));

        bounds[0].gameObject.transform.position = new Vector3(bounds[0].gameObject.transform.position.x, b.max.y + 1.0f);
        bounds[1].gameObject.transform.position = new Vector3(bounds[1].gameObject.transform.position.x, b.min.y - 1.0f);
        bounds[2].gameObject.transform.position = new Vector3(b.min.x - 1.0f, bounds[2].gameObject.transform.position.y);
        bounds[3].gameObject.transform.position = new Vector3(b.max.x + 1.0f, bounds[3].gameObject.transform.position.y);

        clipLength = audioPeer.GetComponent<AudioSource>().clip.length;

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
            startTime = audioPeer.GetComponent<AudioSource>().time;

            y = startTime / -clipLength;

            Vector2 offset = new Vector2(0, y);
            GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

            tree.color = Color32.Lerp(Color.white, Color.white, -y);

            treeColor = tree.color;
            treeColorBar = tree.color;
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

        tree.color = darkGrey;

        Vector2 offset = new Vector2(0, 0);
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

        startScroll = true;
    }
}
