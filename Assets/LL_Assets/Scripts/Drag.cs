using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
	float distance = 10;
	Rigidbody2D rb;
	[SerializeField]float speed = 10.0f;
	[SerializeField]float rotSpeed = 25.0f;


	//angles for look at 2d
	protected Vector3 normTarget;
	protected float angle;
	protected Quaternion rot;

	void Start()
	{
		rb = this.GetComponent<Rigidbody2D>();
	}

	void Update ()
	{

		if (Input.touchCount > 0) {
			
			Vector3 touchPosition = new Vector3 (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f);
			Vector3 objPosition = Camera.main.ScreenToWorldPoint (touchPosition);
			//transform.position = objPosition;
			//this.GetComponent<Rigidbody2D> ().transform.position = transform.position;

			//transform.LookAt(objPosition);
			//transform.rotation = Quaternion.Euler(0,0,this.transform.rotation.z);

			normTarget = (objPosition - this.transform.position).normalized;

			angle = Mathf.Atan2(normTarget.y, normTarget.x)*Mathf.Rad2Deg;

			rot = new Quaternion();
			rot.eulerAngles = new Vector3(0,0,angle-90);

			this.transform.rotation = Quaternion.Slerp(this.transform.localRotation, rot, Time.deltaTime * rotSpeed);

			rb.MovePosition(this.transform.localPosition + this.transform.up * Time.deltaTime * speed);


		}

	}
}