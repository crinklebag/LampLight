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
				var pos = body.Joints [TrackedJoint].Position;

				//var rot = body.Joints [TrackedJoint].Position;
				gameObject.transform.position = new Vector3 (pos.X, pos.Y,0)*Time.time*speed;
				//gameObject.transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.X,pos.Y,0), Time.deltaTime * speed * 1000);
				//gameObject.transform.rotation = new Quaternion (rot.X, rot.Y,0,0);
			}
		}



	}
}
