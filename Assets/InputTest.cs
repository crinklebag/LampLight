using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour {


	public GameObject particle;
	// Use this for initialization

	Vector2 touchspot;
	Vector2 stopSpot;

	public Rigidbody2D rb;

	Vector2 newPos;

	bool stopped;

	public float moveForce = 1;
	void Start () {

		rb = GetComponent<Rigidbody2D> ();

		
	}
	
	// Update is called once per frame
	void Update () {

		Debug.Log (Input.touchCount);

		if (Input.touchCount > 0) 
		{
			Touch touch = Input.GetTouch (0);
			touchspot = touch.deltaPosition;
			Debug.Log(Input.GetTouch(0).position);

			if (Input.GetTouch (0).phase == TouchPhase.Began)
			{
				Debug.Log ("Touch Begin");
				NewPosition ();
				Debug.Log(Input.GetTouch(0).position);
				newPos = touch.deltaPosition;
			}
			if (Input.GetTouch (0).phase == TouchPhase.Moved)
			{
				Debug.Log ("Touch Moved");
				Move ();
				Debug.Log (Input.GetTouch (0).position);
				touchspot = touch.deltaPosition;
			}
			if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				//Stop ();
				Debug.Log ("Touch End");
				stopSpot = touch.deltaPosition;
			}
		}
	}

	void NewPosition()
	{
		particle.transform.position = newPos;

	}

	void Move()
	{
		
			particle.transform.localPosition = Vector3.MoveTowards(particle.transform.position, touchspot, 10 *   Time.deltaTime);

		rb.AddForce (particle.transform.position * moveForce, ForceMode2D.Impulse);
	}


	void Stop()
	{
		particle.transform.position = Vector3.MoveTowards(touchspot, stopSpot ,  Time.deltaTime);
	}
}
