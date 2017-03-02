using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using HTC.UnityPlugin.Multimedia;

[RequireComponent(typeof(MediaDecoder))]
public class Video : MonoBehaviour 
{
	public Material videoMaterial;
	public string folderName = "Videos";
	public string uniformName = "_VideoTexture";
	public string uniformNamePrevious = "_VideoTextureLast";
	public float frameRate = 25;
	public float jumpScale = 10f;
	public KeyCode keyNext = KeyCode.N;
	public KeyCode keyPause = KeyCode.P;
	public KeyCode keyForward = KeyCode.RightArrow;
	public KeyCode keyBackward = KeyCode.LeftArrow;

	private MediaDecoder mediaDecoder;
	private List<string> videoNames;
	private int current = 0;
	private RenderTexture videoTexture;
	private RenderTexture videoTexturePrevious;
	public Texture texture { get { return videoTexture; } }
	public Texture texturePrevious { get { return videoTexturePrevious; } }
	private bool isPlaying = true;
	private float firedAt = 0f;

	void Start ()
	{
		videoNames = new List<string>();
		string path = GetPath(folderName);
		var info = new DirectoryInfo(path);
		FileInfo[] fileInfos = info.GetFiles();
		for (int i = 0; i < fileInfos.Length; ++i) {
			FileInfo fileInfo = fileInfos[i];
			string[] infos = fileInfo.Name.Split('.');
			if (infos[infos.Length - 1] != "meta") {
				videoNames.Add(fileInfo.Name);
			}
		}

		mediaDecoder = GetComponent<MediaDecoder>();
		mediaDecoder.initDecoder(System.IO.Path.Combine(GetPath(folderName), videoNames[current]));
		mediaDecoder.onInitComplete.AddListener(mediaDecoder.startDecoding);
		mediaDecoder.onVideoEnd.AddListener(Rewind);
		
		videoTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		videoTexturePrevious = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
	}

	void Update ()
	{
		if (Input.GetKeyDown(keyPause)) {
			Toggle();
		}
		if (Input.GetKeyDown(keyNext)) {
			Next();
		}
		if (Input.GetKeyDown(keyBackward)) {
			SeekRelative(-jumpScale);
		}
		if (Input.GetKeyDown(keyForward)) {
			SeekRelative(jumpScale);
		}

		if (firedAt + (1f / frameRate) < Time.time) {
			firedAt = Time.time;
			UpdateFrame();
		}

		Shader.SetGlobalTexture(uniformName, videoTexture);
		Shader.SetGlobalTexture(uniformNamePrevious, videoTexturePrevious);
	}

	public string GetPath (string folderName)
	{
		string path = Application.dataPath;
		List<string> paths = new List<string>(path.Split('/'));
		paths.RemoveAt(paths.Count-1);
		path = String.Join("/", paths.ToArray());
		return System.IO.Path.Combine(path, folderName);
	}

	public void UpdateFrame ()
	{
		if (isPlaying && videoMaterial != null) {
			SaveLastFrame();
			Graphics.Blit(null, videoTexture, videoMaterial);
		}
	}

	public void Toggle ()
	{
		isPlaying = !isPlaying;
		if (isPlaying) {
			mediaDecoder.setResume();
		} else {
			mediaDecoder.setPause();
		}
		// Debug.Log(isPlaying);
	}

	public void Toggle (bool should)
	{
		isPlaying = should;
		if (isPlaying) {
			mediaDecoder.setResume();
		} else {
			mediaDecoder.setPause();
		}
	}

	public void Seek (float time)
	{
		SaveLastFrame();
		mediaDecoder.setSeekTime(Mathf.Clamp(time, 0f, mediaDecoder.videoTotalTime));
		Graphics.Blit(null, videoTexture, videoMaterial);
	}

	public void SeekRelative (float time)
	{
		float total = mediaDecoder.videoTotalTime;
		Seek(Mathf.Clamp((mediaDecoder.getVideoCurrentTime() + time) % total, 0f, total));
	}

	public void Next ()
	{
		current = (current + 1) % videoNames.Count;
		mediaDecoder.stopDecoding();
		mediaDecoder.initDecoder(System.IO.Path.Combine(GetPath(folderName), videoNames[current]));
	}

	public void Rewind ()
	{
		Seek(0f);
	}

	public void RandomSeek ()
	{
		Seek(UnityEngine.Random.Range(0f, mediaDecoder.getVideoCurrentTime()));
	}

	private void SaveLastFrame ()
	{
		Graphics.Blit(videoTexture, videoTexturePrevious);
	}
}
