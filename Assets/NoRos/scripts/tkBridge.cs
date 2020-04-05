using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

public class tkBridge : MonoBehaviour {

	/*
	bool tkbridge_can_init (char*port);
	bool tkbridge_can_close ();
	bool tkbridge_can_write_fd(struct can_frame frame);
	bool tkbridge_can_write (int id, int dlc, uint64_t data);
	bool tkbridge_can_read(struct can_frame *frame);
	bool tkbridge_can_write_vals (float*vals, int n_vals);
	*/
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_can_init (string port);
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_can_close();
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_can_write_vals (float [] vals, int n_vals);
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_can_read_vals (float [] vals, int n_vals);
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_camera_init ();
	[DllImport ("libtkbridge")]
	public static extern bool tkbridge_camera(byte [] data, int len);

	// Use this for initialization
	void Start () {
		bool error = tkbridge_can_init ("vcan0");
		Debug.Log ("can init: " + error);
	}
	
	// Update is called once per frame
	void Update () {

		float [] vals = new float[2];
		vals [0] = 10;
		vals [1] = 20;
		tkbridge_can_write_vals (vals, 2);
	}


}
