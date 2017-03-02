using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

	public RawImage image;
	public bool isRecording = true;

	private Color color;

	void Start ()
	{
		color = image.color;
		color.a = 0f;
		image.color = color;
	}
	
	void Update ()
	{
		if (isRecording) {
			color.a = Mathf.Cos(Time.time * 10f) * 0.5f + 0.5f;
		} else {
			color.a = 0f;
		}

		image.color = color;
	}
}
