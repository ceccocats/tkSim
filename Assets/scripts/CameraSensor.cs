using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSensor : MonoBehaviour {

	public bool ok;
	public int FPS = 30;
	public RenderTexture rt;
	private int w, h;
	private Texture2D tex;

	// Use this for initialization
	void Start () {

		w = rt.width;
		h = rt.height;
		tex = new Texture2D (w, h, TextureFormat.RGB24, false);

		ok = tkBridge.tkbridge_camera_init ();
		
		InvokeRepeating("CamUpdate", 0.0f, 1.0f/FPS);
	}

	void CamUpdate() {
		RenderTexture prev = RenderTexture.active;
		RenderTexture.active = rt;

		tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
		tex.Apply();

		byte [] data = tex.EncodeToJPG ();
		ok = tkBridge.tkbridge_camera (data, data.Length);

		RenderTexture.active = prev;
	}

	void Update() {

	}
}
