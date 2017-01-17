using UnityEngine;
using System.Collections;

public class Flicker : MonoBehaviour {

	public int _band;
	[SerializeField] GameObject bugParent;
	[SerializeField] float minAudioFreq;

	[SerializeField] float _startScale;
	[SerializeField] float _scaleMultiplier;

	[SerializeField] bool _useBuffer = false;

	void Start ()
	{
		
	}

    void Update ()
	{
		audioFlicker();
		audioScale();
	}

	//Adjusts the bug's glow sprite's alpha value to match its frequency band's levels
	//Tells the bug parent whether its bug sprite obj should be enabled on or not, depending on whether the band's frequency levels meet
	//	meet the minimum audio frequency
	//Glow sprite object is always enabled
    void audioFlicker ()
	{
		Color tempColor = this.GetComponent<SpriteRenderer> ().color;
		float bandFreq = AudioPeer._audioBandBuffer[_band];

		if (bandFreq < minAudioFreq) {
			tempColor.a = bandFreq;
			bugParent.GetComponent<FireFly>().isOn = false;
		}else{
			tempColor.a = bandFreq;
			bugParent.GetComponent<FireFly>().isOn = true;
		}

		this.GetComponent<SpriteRenderer>().color = tempColor;
		//Debug.Log(this.GetComponent<SpriteRenderer>().color.a);
	}

	//Scale bug glow by amplitude levels, takes bug start scale and a scale multiplier into consideration
	//Buffer provides smoother level drops and increases
	void audioScale ()
	{
		if (!_useBuffer) 
			this.transform.localScale = new Vector3((AudioPeer._amplitude * _scaleMultiplier) + _startScale, (AudioPeer._amplitude * _scaleMultiplier) + _startScale, 1.0f);
		if(_useBuffer)
			this.transform.localScale = new Vector3((AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, (AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, 1.0f);
	}
}
