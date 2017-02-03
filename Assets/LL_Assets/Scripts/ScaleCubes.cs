using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleCubes : MonoBehaviour {

	public int _band;
	public float _startScale;
	public float _scaleMultiplier;

	public bool _useBuffer = false;
	Material _material;

	void Start()
	{
		_material = this.gameObject.GetComponent<MeshRenderer>().materials[0];
	}

	void Update ()
	{
		if (_useBuffer) 
		{
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioTxtReader._currAudioSamples[_band]  * _scaleMultiplier) + _startScale, this.transform.localScale.z);

			Color _colour = new Color(AudioTxtReader._currAudioSamples[_band], AudioTxtReader._currAudioSamples[_band], AudioTxtReader._currAudioSamples[_band]);
			_material.SetColor("_EmissionColor", _colour);

		} 
		if(!_useBuffer)
		{
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioTxtReader._currAudioSamples[_band]  * _scaleMultiplier) + _startScale, this.transform.localScale.z);

			Color _colour = new Color(AudioTxtReader._currAudioSamples[_band], AudioTxtReader._currAudioSamples[_band], AudioTxtReader._currAudioSamples[_band]);
			_material.SetColor("_EmissionColor", _colour);
		}
	}
}