using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSensor : MonoBehaviour {

	public string port = "vcan0";

	public bool ok;
	public MSVehicleControllerFree vehicle;
	private float [] vals = new float[5];

	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		ok = tkBridge.tkbridge_can_init (port);
		InvokeRepeating("canUpdate", 0.0f, 1.0f/50.0f);
	}
	
	// Update is called once per frame
	void Update () {
		// current steer in degs
		float cur_steer = -(vehicle._wheels.leftFrontWheel.wheelCollider.steerAngle+vehicle._wheels.rightFrontWheel.wheelCollider.steerAngle)/2;

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
	}
		
	void canUpdate () {
		ok = tkBridge.tkbridge_can_write_vals (vals, 5);
	}
}
