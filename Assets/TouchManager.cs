using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TouchManager : MonoBehaviour
{

	public static bool screenTouch = false;


	public void TouchInput(BoxCollider2D col)
	{
		Debug.Log (col);
		if (Input.touchCount > 0)
		{
			Debug.Log ("touch");
			//Debug.Log (Input.GetTouch (0).position);
			if(col == Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position)))
			{
				switch (Input.GetTouch (0).phase)
				{
				case TouchPhase.Began:
					Debug.Log ("YEs");
					SendMessage ("OnFirstTouchBegan", SendMessageOptions.DontRequireReceiver);
					SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Stationary:
					SendMessage ("OnFirstTouchStayed", SendMessageOptions.DontRequireReceiver);
					SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Moved:
					SendMessage ("OnFirstTouchMoved", SendMessageOptions.DontRequireReceiver);
					SendMessage ("OnFirstTouch", SendMessageOptions.DontRequireReceiver);
					screenTouch = true;
					break;
				case TouchPhase.Ended:
					Debug.Log ("Ended");
					SendMessage ("OnFirstTouchEnded", SendMessageOptions.DontRequireReceiver);
					screenTouch = false;
					break;

				}

			}

		}

	}

}