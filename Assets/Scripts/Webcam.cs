using UnityEngine;
using System.Collections;

public class Webcam : MonoBehaviour 
{
	public string uniformName = "_WebcamTexture";
	
	private WebCamTexture _texture;
	public WebCamTexture texture { get { return _texture; } }
	private int current;

	void Awake () 
	{
		if (WebCamTexture.devices.Length > 0)
		{
			current = 0;
			_texture = new WebCamTexture(WebCamTexture.devices[0].name);
			_texture.Play();
		}
	}

	void Update ()
	{
		Shader.SetGlobalTexture(uniformName, texture);

		if (Input.GetKeyDown(KeyCode.N)) {
			ChangeCamera();
		}
	}

	void OnEnable ()
	{
		if (_texture != null) {
			_texture.Play();
		}
	}

	void OnDisable ()
	{
		if (_texture != null) {
			_texture.Stop();
		}
	}

	public void ChangeCamera ()
	{
		if (WebCamTexture.devices.Length > 1)
		{
			current = (current + 1) % WebCamTexture.devices.Length;
			_texture.Stop();
			_texture = new WebCamTexture(WebCamTexture.devices[current].name);
			_texture.Play();
		}
	}
}