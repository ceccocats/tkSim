﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSensor : MonoBehaviour {

	public string port = "vcan0";

	public bool ok;
	public bool ok_read;

	public MSVehicleControllerFree vehicle;
	private float [] vals = new float[5];
	private float [] read_vals = new float[2];

	public float steer_rq = 0;
	public float accel_rq = 0;
	public bool manual;

	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		ok = tkBridge.tkbridge_can_init (port);
		InvokeRepeating("canUpdate", 0.0f, 1.0f/50.0f);
		InvokeRepeating("canRead", 0.0f, 1.0f/20.0f);
	}
	
	// Update is called once per frame
	void Update () {
		// current steer in degs
		float cur_steer = (vehicle._wheels.leftFrontWheel.wheelCollider.steerAngle+vehicle._wheels.rightFrontWheel.wheelCollider.steerAngle)/2;

		// compute acceleration
		Vector3 acc;
		Math3d.LinearAcceleration (out acc, transform.position, 2);

		// transform to local coords
		Vector3 locVel = transform.InverseTransformDirection(rb.velocity);
		Vector3 locAcc = transform.InverseTransformDirection(acc);

		// update vals
		vals [0] = cur_steer*Mathf.Deg2Rad;	// steer    rad
		vals [1] = vehicle.KMh / 3.6f;		// speed    m/s
		vals [2] = -rb.angularVelocity.y;	// yawrate  rad/s     axis are inverted
		vals [3] = locAcc.z;			    // accX     m/s
		vals [4] = -locAcc.x;				// accY     m/s       axis are inverted

		//Debug.Log ("yaw: " + vals[2]);

		steer_rq = -read_vals [0]/0.6f;
		accel_rq = read_vals [1];
		if(accel_rq > 0.2f)
			accel_rq = 0.2f;
		Debug.Log ("steer: " + steer_rq + "  Accel: " + accel_rq);
	}
		
	void canUpdate () {
		ok = tkBridge.tkbridge_can_write_vals (vals, 5);
	}
	void canRead () {
		ok_read = tkBridge.tkbridge_can_read_vals (read_vals, 2);
		if(!ok_read) {
			read_vals[0] = 0f;
			read_vals[1] = -0.5f;		
		}
	}
}
