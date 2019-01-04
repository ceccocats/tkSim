using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSensor : MonoBehaviour {

	public MSVehicleControllerFree vehicle;
	private float [] vals = new float[2];

	// Use this for initialization
	void Start () {
		bool error = tkBridge.tkbridge_can_init ("vcan0");
		Debug.Log ("can init: " + error);
		InvokeRepeating("canUpdate", 0.0f, 1.0f/100.0f);
	}
	
	// Update is called once per frame
	void Update () {
		vals [0] = vehicle._wheels.leftFrontWheel.wheelCollider.steerAngle;
		vals [1] = vehicle.KMh;

		Debug.Log ("steer: " + vals [0]);
	}
		
	void canUpdate () {
		tkBridge.tkbridge_can_write_vals (vals, 2);
	}
}
