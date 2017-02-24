using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FernMovement : MonoBehaviour {

	private bool isRotating = true;
	private bool goingRight = true;

	[SerializeField]
	private float AngleToRotate = 15.0f;
	[SerializeField]
	private float rotSpeed = 2.5f;
	[SerializeField]
	private float wiggleAngle = 3.0f;
	[SerializeField]
	private float wiggleSpeed = 5.0f;

	private Quaternion targetRot;
	private Transform wiggleTransform;
	private Quaternion wiggleRot;

	void Start()
	{
		wiggleTransform = this.transform.GetChild(0).GetComponent<Transform>();
	}

	void Update ()
	{
		//Swap direction bool on beat, swap target rotation
		if (AudioManager.beatCheckQuarter) 
		{
			goingRight = !goingRight;
			if (goingRight) 
			{
				targetRot = Quaternion.Euler (0.0f, 0.0f, 180.0f - AngleToRotate);
			} 
			else 
			{
				targetRot = Quaternion.Euler (0.0f, 0.0f, 180.0f + AngleToRotate);
			}
		}
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRot, Time.deltaTime * rotSpeed);

		wiggleTransform.localRotation =  Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.timeSinceLevelLoad * wiggleSpeed) * wiggleAngle);
	}
}
