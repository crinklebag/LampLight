﻿using System.Collections;
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
<<<<<<< HEAD
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._bandBuffer[_band] * _scaleMultiplier) + _startScale, this.transform.localScale.z);
=======
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._audioBandBuffer[_band] * _scaleMultiplier) + _startScale, this.transform.localScale.z);
>>>>>>> feature/Audio
			Color _colour = new Color(AudioPeer._audioBandBuffer[_band], AudioPeer._audioBandBuffer[_band], AudioPeer._audioBandBuffer[_band]);
			_material.SetColor("_EmissionColor", _colour);

		} 
		if(!_useBuffer)
		{
<<<<<<< HEAD
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._freqBands[_band] * _scaleMultiplier) + _startScale, this.transform.localScale.z);
=======
			this.transform.localScale = new Vector3(this.transform.localScale.x, (AudioPeer._audioBand[_band] * _scaleMultiplier) + _startScale, this.transform.localScale.z);
>>>>>>> feature/Audio
			Color _colour = new Color(AudioPeer._audioBand[_band], AudioPeer._audioBand[_band], AudioPeer._audioBand[_band]);
			_material.SetColor("_EmissionColor", _colour);
		}
	}
}