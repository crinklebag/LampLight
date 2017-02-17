using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;
public class followhand : BodySourceView {


	public GameObject bodypart;
	public Kinect.JointType TrackedJoint;
	private BodySourceManager bodyManager;
	private Kinect.Body[] bodies;

	[SerializeField]public float speed;

	[SerializeField]public float distance;

	public Vector3 temp;

	// Use this for initialization
	void Start () {
		bodyManager = BodySourceManager.GetComponent<BodySourceManager> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (bodyManager == null)
		{
			return;
		}
		bodies = bodyManager.GetData ();
		if (bodies == null)
		{
			return;
		}
		foreach (var body in bodies) 
		{
			if (body == null)
			{
				continue;
			}
			if (body.IsTracked) 
			{
				if (body.Joints [TrackedJoint].Position.Z < distance) {
					var pos = body.Joints [TrackedJoint].Position;

					Debug.Log (body.Joints [TrackedJoint].Position.Z);
					temp = new Vector3 (pos.X, pos.Y);

					//var rot = body.Joints [TrackedJoint].Position;
					//gameObject.transform.position = new Vector3 (pos.X, pos.Y,0)*Time.time*speed;
					//gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.X,pos.Y,0), Time.deltaTime * speed * 1000);
					//gameObject.transform.rotation = new Quaternion (rot.X, rot.Y,0,0);

					SmoothMove ();
				}
			}
		}



	}

	void SmoothMove()
	{
		this.transform.position = Vector3.Lerp (this.transform.position, temp * speed, Time.smoothDeltaTime);

	}
}
