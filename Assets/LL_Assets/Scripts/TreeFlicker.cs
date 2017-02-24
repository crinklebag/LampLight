using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFlicker : MonoBehaviour {

	[SerializeField]
	private Sprite frame_01;
	[SerializeField]
	private Sprite frame_02;

	private SpriteRenderer SR;

	void Start()
	{
		SR = this.GetComponent<SpriteRenderer>();
	}

	void Update () 
	{
		//swap sprite on beat
		if(AudioManager.beatCheck)
			ChangeSprite();
	}

	void ChangeSprite ()
	{
		if (SR.sprite.name == frame_01.name) 
		{
			SR.sprite = frame_02;
		} 
		else 
		{
			SR.sprite = frame_01;
		}
	}
}
