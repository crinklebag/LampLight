using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotate : MonoBehaviour {

	[SerializeField] private float rotSpeed = 5.0f;

	void Update () 
	{
		this.transform.Rotate(-Vector3.forward * rotSpeed, Space.World);
	}
}
