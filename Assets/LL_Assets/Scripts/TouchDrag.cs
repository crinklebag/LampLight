using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDrag : TouchManager {

	// Use this for initialization

	Vector2 direction;

	float speed; 

	Rigidbody2D rb;
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
		//transform.position = pos;
		transform.LookAt (pos);
		rb.MovePosition (this.transform.position + Vector3.forward + pos * Time.deltaTime);
		//RotateJar ();
		Debug.Log ("DRAG");
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

}
