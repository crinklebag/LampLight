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
        audioPeer = GameObject.Find("AudioPeer");

        float width = -transform.localScale.y * Screen.width / Screen.height;

        transform.localScale = new Vector3(width, transform.localScale.y, 1);

        float height = Camera.main.orthographicSize * 2.0f;

        width = tree.transform.localScale.y * Screen.width / Screen.height;

        tree.gameObject.transform.localScale = new Vector3(width, height / tree.bounds.size.y, 1);

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

            tree.color = Color32.Lerp(darkGrey, Color.white, -y);

            treeColor = tree.color;
            treeColorBar = tree.color;
        }

        //Debug.Log(tree.color);
    }

    public void Reset(float cl)
    {
        Debug.Log("Called reset to play song: " + audioPeer.GetComponent<AudioSource>().clip.name + " with length: " + cl);

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
