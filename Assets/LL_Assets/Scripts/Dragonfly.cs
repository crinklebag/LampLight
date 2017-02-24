using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dragonfly : MonoBehaviour {

    public GameObject sprite;
    public GameObject musicClef;
    public GameObject musicClefPrefab;

    GameController gameController;

    [SerializeField] float force;
	[SerializeField] float jumpDistance;
	[SerializeField] float jumpSpeed;
	[SerializeField] float lowPoint;
	[SerializeField] float highPoint;

    [SerializeField]
    private Color32 opaqueColor = new Color32(255, 255, 255, 10);
    [SerializeField]
    private Color32 transparentColor = new Color32(255, 255, 255, 0);
    [SerializeField]
    private Color32 currentColor = new Color32(255, 255, 255, 0);

    [SerializeField]
    private bool goingUp = false;
    [SerializeField]
    private bool goingLeft = false;

	private Rigidbody2D rb;

    [SerializeField]
    float lerpColorTime = 0;

    [SerializeField]
    bool boundsComingIn = false;
    [SerializeField]
    bool boundsGoingOut = false;

    [SerializeField]
    float[] jumpPositions = { 4.42f, 2.545f, 0.5f, -1.51f, -3.53f };

    [SerializeField]
    int jumpPosCounter = 0;

    void Start ()
	{
        force = 3300.0f;

        /*jumpPositions = new float[5];
        jumpPositions[0] = 4.42f;
        jumpPositions[1] = 2.545f;
        jumpPositions[2] = 0.5f;
        jumpPositions[3] = -1.51f;
        jumpPositions[4] = -3.53f;*/

        musicClef = GameObject.Instantiate(musicClefPrefab);
        musicClef.GetComponent<SpriteRenderer>().sortingLayerName = "Bug";
        GameObject.Find("BG").GetComponent<Scaler>().ResizeObjectToBounds(musicClef.GetComponent<SpriteRenderer>());

        jumpPositions = new float[musicClef.GetComponent<MusicClef>().GetLinesLength()];
        jumpPositions[0] = musicClef.GetComponent<MusicClef>().GetYPos(0);
        jumpPositions[1] = musicClef.GetComponent<MusicClef>().GetYPos(1);
        jumpPositions[2] = musicClef.GetComponent<MusicClef>().GetYPos(2);
        jumpPositions[3] = musicClef.GetComponent<MusicClef>().GetYPos(3);
        jumpPositions[4] = musicClef.GetComponent<MusicClef>().GetYPos(4);

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        rb = GetComponent<Rigidbody2D> ();

        //Debug.Log("Before: " + sprite.gameObject.transform.rotation);

        if (goingLeft)
        {
            sprite.gameObject.transform.rotation = new Quaternion(sprite.gameObject.transform.rotation.x, sprite.gameObject.transform.rotation.y, 1.0f, sprite.gameObject.transform.rotation.w);
            GetComponent<BoxCollider2D>().offset = new Vector2(-0.6f, 0);
        }
        else
        {
            sprite.gameObject.transform.rotation = new Quaternion(sprite.gameObject.transform.rotation.x, sprite.gameObject.transform.rotation.y, -1.0f, sprite.gameObject.transform.rotation.w);
            GetComponent<BoxCollider2D>().offset = new Vector2(0.6f, 0);
        }

        //Debug.Log("After: " + sprite.gameObject.transform.rotation);

        highPoint = GameObject.Find("Top").gameObject.transform.position.y - 2.5f;
        lowPoint = GameObject.Find("Bottom").gameObject.transform.position.y + 2.0f;

        //Add Force to move across screen
        StartCoroutine(MoveHorizontal()); 
	}

	void Update ()
	{
		CheckDirection();//check if we need to switch vertical directiond

		if (AudioManager.beatCheckHalf) { //Move vertically on beat
			MoveVertical();

            if (boundsComingIn == true && boundsGoingOut == false)
            {
                if (musicClef.GetComponent<SpriteRenderer>().color != transparentColor)
                {
                    currentColor = musicClef.GetComponent<SpriteRenderer>().color;

                    musicClef.GetComponent<SpriteRenderer>().color = Color32.Lerp(currentColor, opaqueColor, (lerpColorTime += Time.deltaTime * 5.0f) / 1f);
                }
                else
                {
                    musicClef.GetComponent<SpriteRenderer>().color = Color32.Lerp(transparentColor, opaqueColor, (lerpColorTime += Time.deltaTime * 5.0f) / 1f);
                }
            }
		}
        else
        {
            if (boundsComingIn == true && boundsGoingOut == false)
            {
                musicClef.GetComponent<SpriteRenderer>().color = Color32.Lerp(opaqueColor, transparentColor, (lerpColorTime += Time.deltaTime * 2.0f) / 1f);
            }
            else if (boundsComingIn == true && boundsGoingOut == true)
            {
                musicClef.GetComponent<SpriteRenderer>().color = transparentColor;
            }
        }
	}

	//Add/Subtract the jump distance and this position based off direction
	void MoveVertical ()
	{
        lerpColorTime = 0;

        if (!goingLeft)
        {
            if (transform.position.x > GameObject.Find("Left").gameObject.transform.position.x + 0.5f)
            {
                boundsComingIn = true;
                boundsGoingOut = false;
            }

            if (transform.position.x > GameObject.Find("Right").gameObject.transform.position.x - 0.5f)
            {
                boundsGoingOut = true;
            }
        }
        else
        {
            if (transform.position.x < GameObject.Find("Left").gameObject.transform.position.x + 1.0f)
            {
                boundsGoingOut = true;
            }

            if (transform.position.x < GameObject.Find("Right").gameObject.transform.position.x + 0.5f && transform.position.x > 0)
            {
                boundsComingIn = true;
                boundsGoingOut = false;
            }
        }

        if (goingUp) {
            //Debug.Log("goingUp");
            //rb.MovePosition(this.transform.localPosition + new Vector3(0,jumpDistance,0));
            //float newY = this.transform.position.y + jumpDistance;
            //double roundedY = System.Math.Round(newY, 2);

            //Vector3 newPos = new Vector3(this.transform.localPosition.x, (float)roundedY, this.transform.position.z);
            //this.transform.position = Vector3.Lerp(this.transform.position, newPos, jumpSpeed * Time.deltaTime);

            jumpPosCounter--;
            jumpPosCounter = Mathf.Clamp(jumpPosCounter, 0, 4);

            Vector3 newPos = new Vector3(this.transform.localPosition.x, jumpPositions[jumpPosCounter], this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, jumpSpeed * Time.deltaTime);
        }
        else {
            //Debug.Log("goingDown");
            //rb.MovePosition(this.transform.localPosition - new Vector3(0,jumpDistance,0));
            //float newY = this.transform.position.y - jumpDistance;
            //double roundedY = System.Math.Round(newY, 2);

            //Vector3 newPos = new Vector3(this.transform.localPosition.x, (float)roundedY, this.transform.position.z);
            //this.transform.position = Vector3.Lerp(this.transform.position, newPos, jumpSpeed * Time.deltaTime);

            jumpPosCounter++;
            jumpPosCounter = Mathf.Clamp(jumpPosCounter, 0, 4);

            Vector3 newPos = new Vector3(this.transform.localPosition.x, jumpPositions[jumpPosCounter], this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, jumpSpeed * Time.deltaTime);
        }
	}

	//Add horizontal force in proper direction
	IEnumerator MoveHorizontal ()
	{
		if (goingLeft) {
			rb.AddForce (-this.transform.right * force * Time.deltaTime);
		} else {
			rb.AddForce (this.transform.right * force * Time.deltaTime);
		}
		yield return null;

	}

	//Check if this position.y against high and low points to determine if we need to switch
	void CheckDirection ()
	{
		if (this.transform.position.y >= jumpPositions[0]) {
            //Debug.Log("Hit High");
			goingUp = false;
            jumpPosCounter = 0;
		} else if (this.transform.position.y <= jumpPositions[4]) {
            //Debug.Log("Hit Low");
			goingUp = true;
            jumpPosCounter = 4;
        }
	}

	void GetInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow)) 
		{
			Debug.Log ("YES");
			//this.transform.position += new Vector3(0,1,0);
			//rb.MovePosition = new Vector3(0,1,0);
			rb.MovePosition(this.transform.localPosition + new Vector3(0,1.5f,0));
		}

		if (Input.GetKeyDown(KeyCode.DownArrow)) 
		{
			//this.transform.localPosition -= new Vector3(0,1,0);
			//rb.MovePosition = new Vector3(0,1,0);
			rb.MovePosition(this.transform.localPosition - new Vector3(0,1.5f,0));
		}
	}

    public void SetSpawnSide(bool lr)
    {
        goingLeft = lr;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DragonflyDestroyer"))
        {
            Destroy(musicClef.gameObject);
            Destroy(this.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player!!");

            gameController.CrackJar();
        }
    }
}
