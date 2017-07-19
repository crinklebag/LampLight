using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageScaler : MonoBehaviour {

	public GameObject reference;

	public GameObject[] objects2Scale;

	[SerializeField] float width;
    [SerializeField] float height;

    float baseY = 0.0f;

	// Use this for initialization
	void Awake () {
		baseY = reference.transform.position.y;

        height = Camera.main.orthographicSize * 2.0f;

        width = height * (Screen.width / Screen.height);

        height = 2.0f * Mathf.Tan(0.5f * Camera.main.fieldOfView * Mathf.Deg2Rad) * -10.0f;
        width = height * Camera.main.aspect;

		float aspectHeight = 1;

		// 5:4
		if (Camera.main.aspect <= 1.3f)
		{
			aspectHeight = 4.0f;
		}
		// 4:3
		else if (Camera.main.aspect <= 1.4f)
		{
			aspectHeight = 3.0f;
		}
        // 3:2
		else if (Camera.main.aspect == 1.5f)
		{
			aspectHeight = 2.0f;
		}
		// 16:10
		else if (Camera.main.aspect == 1.6f)
		{
			//aspectHeight = 10.0f;
		}
		// 16:9
		else if (Camera.main.aspect <= 1.8f)
		{
			//aspectHeight = 9.0f;
		}

		for (int i = 0; i < objects2Scale.Length; i++)
		{
			objects2Scale[i].transform.localScale = new Vector3(Mathf.Abs(width / 11.5f), Mathf.Abs(width / 11.5f), objects2Scale[i].transform.localScale.z);

			objects2Scale[i].transform.position = new Vector3(objects2Scale[i].transform.position.x, baseY / aspectHeight, objects2Scale[i].transform.position.z);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
