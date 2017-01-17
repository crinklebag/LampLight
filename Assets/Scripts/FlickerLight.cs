using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickerLight : MonoBehaviour {
	public int _band;
	public float _minIntensity;
	public float _maxIntensity;
	private Light _light;

	[SerializeField] float minAudioFreq;
	[SerializeField] GameObject bugParent;
	
	void Start () 
	{
		_light = this.gameObject.GetComponent<Light>();
	}
	

	void Update () 
	{
		_light.intensity = (AudioPeer._audioBandBuffer[_band] * (_maxIntensity - _minIntensity)) +  _minIntensity;

		if (_light.intensity < minAudioFreq){
			bugParent.GetComponent<FireFly>().isOn = false;
		}
		else{
			bugParent.GetComponent<FireFly>().isOn = true;
		}
	}
}
