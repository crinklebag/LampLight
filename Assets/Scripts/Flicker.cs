using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	public int _band;
	[SerializeField] GameObject bugParent;
	[SerializeField] float minIntensity;
	[SerializeField] float maxIntensity;
	[SerializeField] float minAudioFreq;

	public float _startScale;
	public float _scaleMultiplier;

	public bool _useBuffer = false;

	void Start ()
	{
		
	}

    void Update ()
	{
		audioFlicker();
		audioScale();
	}

    void audioFlicker ()
	{
		Color tempColor = this.GetComponent<SpriteRenderer> ().color;
		//float Amp = AudioPeer._amplitudeBuffer;
		float bandFreq = AudioPeer._audioBandBuffer[_band];
		//float newAlpha = AudioPeer._amplitudeBuffer;
		float newAlpha = AudioPeer._audioBandBuffer[_band];
		//float newAlpha = (AudioPeer._audioBandBuffer[_band] * (maxIntensity - minIntensity)) +  minIntensity;

		if (bandFreq < minAudioFreq) {
			//tempColor.a = 0.0f;
			tempColor.a = newAlpha;
			bugParent.GetComponent<FireFly>().isOn = false;
		}
		else{
			tempColor.a = newAlpha;
			bugParent.GetComponent<FireFly>().isOn = true;
		}

		this.GetComponent<SpriteRenderer>().color = tempColor;
		//Debug.Log(this.GetComponent<SpriteRenderer>().color.a);
	}

	void audioScale ()
	{
		if (!_useBuffer) 
			this.transform.localScale = new Vector3((AudioPeer._amplitude * _scaleMultiplier) + _startScale, (AudioPeer._amplitude * _scaleMultiplier) + _startScale, (AudioPeer._amplitude * _scaleMultiplier) + _startScale);
		if(_useBuffer)
			this.transform.localScale = new Vector3((AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, (AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, (AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale);
	}
}
