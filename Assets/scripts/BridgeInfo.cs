using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BridgeInfo : MonoBehaviour {

	Velodyne_UDP_Send lidar_sens;
	CanSensor can_sens;
	CameraSensor cam_sens;

	public GameObject vehicle;
	public Text text;

	// Use this for initialization
	void Start () {
		lidar_sens = vehicle.GetComponentInChildren<Velodyne_UDP_Send> ();
		can_sens = vehicle.GetComponentInChildren<CanSensor> ();
		cam_sens = vehicle.GetComponentInChildren<CameraSensor> ();

	}
		
	string getColorStr(bool status) {
		if (!status)
			return "<color=#ff0000ff> x </color>";
		else
			return "<color=#00ff00ff> ✓ </color>";
	}
	
	// Update is called once per frame
	void Update () {


		text.text = "Sensors:\n" + 
			getColorStr(lidar_sens.ok) + " lidar  \n" + 
			getColorStr(can_sens.ok)   + " CAN    \n" + 
			getColorStr(cam_sens.ok)   + " camera \n";
	}
}
