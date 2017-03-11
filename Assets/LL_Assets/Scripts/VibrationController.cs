using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationController : MonoBehaviour {

	public void Vibrate ()
	{
		if (Application.platform == RuntimePlatform.Android) 
		{
			Handheld.Vibrate();
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Handheld.Vibrate();
		}
	}
}
