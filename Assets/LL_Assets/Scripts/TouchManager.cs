using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchManager : MonoBehaviour
{

	public static bool screenTouch = false;


	public void TouchInput(BoxCollider2D col)
	{
		if (Input.touchCount > 0)
		{
			Debug.Log ("touch");
			if(col == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
			{
				Debug.Log ("touch" + Input.GetTouch (0).phase);
				switch (Input.GetTouch (0).phase)
				{

				case TouchPhase.Began:
					SendMessage ("OnFirstTouchBegan", SendMessageOptions.DontRequireReceiver);
					SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Stationary:
					SendMessage ("OnFirstTouchStayed", SendMessageOptions.DontRequireReceiver);
					//SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Moved:
					SendMessage ("OnFirstTouchMoved", SendMessageOptions.DontRequireReceiver);
					SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Ended:
					SendMessage ("OnFirstTouchEnded", SendMessageOptions.DontRequireReceiver);
					screenTouch = false;
					break;
				}

			}

		}

	}

}