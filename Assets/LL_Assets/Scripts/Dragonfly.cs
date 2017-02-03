﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonfly : MonoBehaviour {

    public GameObject sprite;

    GameController gameController;

    [SerializeField] float force;
	[SerializeField] float jumpDistance;
	[SerializeField] float jumpSpeed;
	[SerializeField] float lowPoint;
	[SerializeField] float highPoint;

	private bool goingUp = false;
	private bool goingLeft = false;

	private Rigidbody2D rb;

	void Start ()
	{
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        rb = GetComponent<Rigidbody2D> ();

        //Debug.Log("Before: " + sprite.gameObject.transform.rotation);

        if (goingLeft)
        {
            sprite.gameObject.transform.rotation = new Quaternion(sprite.gameObject.transform.rotation.x, sprite.gameObject.transform.rotation.y, 1.0f, sprite.gameObject.transform.rotation.w);
            GetComponent<BoxCollider2D>().offset = new Vector2(-1.2f, 0);
        }
        else
        {
            sprite.gameObject.transform.rotation = new Quaternion(sprite.gameObject.transform.rotation.x, sprite.gameObject.transform.rotation.y, -1.0f, sprite.gameObject.transform.rotation.w);
            GetComponent<BoxCollider2D>().offset = new Vector2(1.2f, 0);
        }

        //Debug.Log("After: " + sprite.gameObject.transform.rotation);

        highPoint = GameObject.Find("Top").gameObject.transform.position.y - 2.5f;
        lowPoint = GameObject.Find("Bottom").gameObject.transform.position.y + 2.5f;

        //Add Force to move across screen
        StartCoroutine(MoveHorizontal()); 
	}

	void Update ()
	{
		CheckDirection();//check if we need to switch vertical direction

		if (AudioPeer.GetBeat ()) { //Move vertically on beat
			MoveVertical();
		}
	}

	//Add/Subtract the jump distance and this position based off direction
	void MoveVertical ()
	{
		if (goingUp) {
			//rb.MovePosition(this.transform.localPosition + new Vector3(0,jumpDistance,0));
			Vector3 newPos = new Vector3(this.transform.localPosition.x, this.transform.position.y + jumpDistance, this.transform.position.z);
			this.transform.position = Vector3.Lerp(this.transform.position, newPos, jumpSpeed * Time.deltaTime);
		} else {
			//rb.MovePosition(this.transform.localPosition - new Vector3(0,jumpDistance,0));
			Vector3 newPos = new Vector3(this.transform.localPosition.x, this.transform.position.y - jumpDistance, this.transform.position.z);
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
		if (this.transform.position.y >= highPoint) {
			goingUp = false;
		} else if (this.transform.position.y <= lowPoint) {
			goingUp = true;
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
