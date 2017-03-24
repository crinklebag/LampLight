using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour {

	[SerializeField] float destroyTime = 5.0f;

	void Start()
	{
		DestroyMe();
	}

	void DestroyMe()
	{
		Destroy(this.gameObject, destroyTime);
	}
}
