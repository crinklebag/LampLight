using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrag : TouchManager {

	// Use this for initialization

	Vector2 direction;
	public Rigidbody2D rb;

	float speed; 
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		TouchInput(GetComponent<BoxCollider2D>());

	}

	void OnFirstTouch()
	{
		Vector3 pos;

		pos = new Vector3 (Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y,0);

		transform.position = pos;
		//rb.AddForce(pos);
		Debug.Log (pos);
		//rb.transform.position = pos;
		//if (TouchManager.screenTouch == false) {
		//	rb.AddForce(Vector3.zero);
		//	rb.velocity = Vector3.zero;
			//rb.angularVelocity = 0f;
		//}

		//RotateJar ();
		Debug.Log ("DRAG");
	}

	void OnTouchEnded()
	{
		//rb.velocity = Vector3.zero;
		//rb.angularVelocity = 0f;
	}

	void RotateJar()
	{
		if (Input.GetTouch (0).phase == TouchPhase.Moved)
		{
			//direction = Input.GetTouch (0).deltaPosition.normalized;
			speed = Input.GetTouch (0).deltaPosition.magnitude;
			transform.rotation = Quaternion.LookRotation (direction);
		}
	}

	void Stop()
	{
		this.transform.position = this.transform.position;
	}

}
