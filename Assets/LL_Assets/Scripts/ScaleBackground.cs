using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBackground: MonoBehaviour {

	public float _startScale;
	public float _scaleMultiplier;

	public float _red;
	public float _green;
	public float _blue;

	public bool _useBuffer = false;

	void Update ()
	{
		if (!_useBuffer) {
			this.transform.localScale = new Vector3 (this.transform.localScale.x, (AudioPeer._amplitude * _scaleMultiplier) + _startScale, this.transform.localScale.z);
		}
		if(_useBuffer)
		{
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, this.transform.localScale.z);
		}
	}
}