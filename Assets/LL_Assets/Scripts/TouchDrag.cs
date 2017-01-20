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

		pos = new Vector3 (Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x, Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y, 0.0f);
		//transform.position = pos;
		//transform.LookAt (pos);

		//transform.rotation = Quaternion.Euler (0, 0, this.transform.rotation.z);


		/// 2d LOOKAT
		Vector3 normTarget = (pos - this.transform.position).normalized;

		//Vector3 norm = normTarget.normalized;

		float angle = Mathf.Atan2 (normTarget.y, normTarget.x) * Mathf.Rad2Deg;

		Quaternion rot = new Quaternion ();

		rot.eulerAngles = new Vector3 (0, 0, angle);

		Debug.Log(angle);

		this.transform.rotation = rot;


		rb.MovePosition (this.transform.position + Vector3.up + pos * Time.deltaTime);
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
