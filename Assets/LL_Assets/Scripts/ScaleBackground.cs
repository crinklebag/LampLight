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
<<<<<<< HEAD
	Material _material;

	void Start()
	{
		_material = this.gameObject.GetComponent<MeshRenderer>().materials[0];
	}
=======
>>>>>>> feature/Audio

	void Update ()
	{
		if (!_useBuffer) 
		{
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._amplitude * _scaleMultiplier) + _startScale, this.transform.localScale.z);
<<<<<<< HEAD
			Color _colour = new Color(_red * AudioPeer._amplitude, _green * AudioPeer._amplitude, _blue * AudioPeer._amplitude);
			_material.SetColor("_EmissionColor", _colour);
=======
>>>>>>> feature/Audio

		} 
		if(_useBuffer)
		{
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._amplitudeBuffer * _scaleMultiplier) + _startScale, this.transform.localScale.z);
<<<<<<< HEAD
			Color _colour = new Color(_red * AudioPeer._amplitudeBuffer, _green * AudioPeer._amplitudeBuffer, _blue * AudioPeer._amplitudeBuffer);
			_material.SetColor("_EmissionColor", _colour);
=======
>>>>>>> feature/Audio
		}
	}
}