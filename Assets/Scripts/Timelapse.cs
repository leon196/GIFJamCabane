using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timelapse : MonoBehaviour {

	public Shader shader;
	public Shader shaderSlicing;
	public float frameRate = 30f;
	public int count = 200;
	private int lod = 1; 

	private Webcam webcam;
	private RenderTexture[] textures;
	private Material materialSlicing;
	private FrameBuffer frameBuffer;
	private float lastFire = 0f;
	private bool isReady = false;
	private float lineSize = 0.01f;
	private int width, height;

	void Start ()
	{
		webcam = GameObject.FindObjectOfType<Webcam>();
		textures = new RenderTexture[count];

		materialSlicing = new Material(shaderSlicing);

		width = webcam.texture.width;
		height = webcam.texture.height;

		frameBuffer = new FrameBuffer(width, height, 2);

		for (int i = 0; i < count; ++i) {
			textures[i] = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			textures[i].Create();
			textures[i].filterMode = FilterMode.Bilinear;
			textures[i].wrapMode = TextureWrapMode.Clamp;
		}

		lineSize = 2f / (float)count;

		isReady = true;
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			ChangeLOD(lod - 1);
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			ChangeLOD(lod + 1);
		}
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			ChangeCount(count + 50);
		} else if (Input.GetKeyDown(KeyCode.DownArrow)) {
			ChangeCount(count - 50);
		}

		if (isReady) {
			materialSlicing.SetFloat("_LineSize", lineSize);

			if (lastFire + 1f / frameRate < Time.time) {
				lastFire = Time.time;

				for (int i = count - 1; i > 0; --i) {
					Graphics.Blit(textures[(i-1)%count], textures[i%count]);
					materialSlicing.SetFloat("_LinePosition", i / (float)count);
					materialSlicing.SetTexture("_NewFrame", textures[(i-1)%count]);
					frameBuffer.Apply(materialSlicing);
				}
				Graphics.Blit(webcam.texture, textures[0]);
			}

			Shader.SetGlobalTexture("_TimeTexture", frameBuffer.Get());
		}
	}

	void ChangeLOD (int newLOD)
	{
		isReady = false;

		lod = (int)Mathf.Clamp(newLOD, 1f, 10f);

		width = webcam.texture.width / lod;
		height = webcam.texture.height / lod;

		frameBuffer = new FrameBuffer(width, height, 2);

		for (int i = 0; i < count; ++i) {
			if (textures[i] != null) {
				textures[i].Release();
			}
			textures[i] = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			textures[i].Create();
			textures[i].filterMode = FilterMode.Bilinear;
			textures[i].wrapMode = TextureWrapMode.Clamp;
		}

		isReady = true;
	}

	void ChangeCount (int newCount)
	{
		isReady = false;

		for (int i = 0; i < count; ++i) {
			if (textures[i] != null) {
				textures[i].Release();
			}
		}

		count = (int)Mathf.Clamp(newCount, 50, 1000);

		lineSize = 2f / (float)count;

		textures = new RenderTexture[count];
		for (int i = 0; i < count; ++i) {
			textures[i] = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
			textures[i].Create();
			textures[i].filterMode = FilterMode.Bilinear;
			textures[i].wrapMode = TextureWrapMode.Clamp;
		}

		isReady = true;
	}
}
