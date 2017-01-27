using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragonfly : MonoBehaviour {

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
		rb = GetComponent<Rigidbody2D> ();

		//Check which side of the screen were starting on
		if (this.transform.position.x > 0) {
			goingLeft = true;
		} else {
			goingLeft = false;
		}

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

}
