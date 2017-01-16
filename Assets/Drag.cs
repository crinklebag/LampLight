using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
	float distance = 10;

	void Update(){
		Vector3 touchPosition = new Vector3 (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, distance);
		Vector3 objPosition = Camera.main.ScreenToWorldPoint (touchPosition);
		transform.position = objPosition;
		//this.GetComponent<Rigidbody2D> ().transform.position = transform.position;
	}
}