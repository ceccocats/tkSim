using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class GPSSensor : MonoBehaviour {

	public bool ok;
	public int HZ = 5;
	private StreamWriter sw;

	private const double R = 6371000.0;
	private const double lat0 = 44.629046;
	private const double lon0 = 10.950265;
	private const double h0 = 80;
	private double x0, y0, cosLat0;

	// Use this for initialization
	void Start () {
		// origin coordinates
		cosLat0 = (double) Mathf.Cos( (float) lat0);
		x0 = R*lon0*cosLat0;
		y0 = R*lat0;

		FileStream fs = File.OpenWrite( "/tmp/ttyGPSin" );
		sw = new StreamWriter (fs);
		InvokeRepeating("GpsUpdate", 0.0f, 1.0f/HZ);
	}

	void GpsUpdate() {
		double lat, lon, h;
		double x, y, z;
		GpsUtils.EnuToEcef (-transform.position.x, -transform.position.z, transform.position.y,
			lat0, lon0, h0, out x, out y, out z);
		GpsUtils.EcefToGeodetic (x, y, z, out lat, out lon, out h);
		int    quality     = 1;
		int    nsats       = 10;
		double hdop        = 1.0;
		double geodial_sep = 0;
		double age         = 1.0;

		//Debug.Log ("lat/lon: "+ lat +" "+ lon);
			
		// build string
		string gngga = "$GNGGA,";
		gngga += System.DateTime.Now.ToString("HHmmss.ff,"); // timestamp
		gngga += string.Format ("{0},N,", lat*100);       // lat
		gngga += string.Format ("{0},W,", lon*100);       // lon
		gngga += quality + ",";   					         // quality
		gngga += nsats + ",";   					         // n satellites
		gngga += string.Format ("{0},", hdop);            // HDOP
		gngga += string.Format ("{0},M,", h);             // height
		gngga += string.Format ("{0},M,", geodial_sep);   // height
		gngga += string.Format ("{0},", age);             // age of correction
		gngga += "0000";						             // id of correction

		int checksum = 0; 
		for (int i = 0; i < gngga.Length; i++) { 
			checksum ^= (byte)(gngga[i]);
		}
		gngga += "*" + checksum;

		try {
			sw.WriteLine (gngga);
			sw.Flush ();
			ok = true;
		} catch (IOException ex) {
			//Debug.LogError (ex.Message);
			ok = false;
		}
	}
		

	void Update() {

	}
}


/*
NMEA spec

$GPGGA,181908.00,3404.7041778,N,07044.3966270,W,4,13,1.00,495.144,M,29.200,M,0.10,0000*40

All NMEA messages start with the $ character, and each data field is separated by a comma.
 0) - GP represent that it is a GPS position (GL would denote GLONASS).
 1) - 181908.00 is the time stamp: UTC time in hours, minutes and seconds.
 2) - 3404.7041778 is the latitude in the DDMM.MMMMM format. Decimal places are variable.
 3) - N denotes north latitude.
 4) - 07044.3966270 is the longitude in the DDDMM.MMMMM format. Decimal places are variable.
 5) - W denotes west longitude.
 6) - 4 denotes the Quality Indicator:
    1 = Uncorrected coordinate
    2 = Differentially correct coordinate (e.g., WAAS, DGPS)
    4 = RTK Fix coordinate (centimeter precision)
    5 = RTK Float (decimeter precision.
 7) - 13 denotes number of satellites used in the coordinate.
 8) - 1.0 denotes the HDOP (horizontal dilution of precision).
09) - 495.144 denotes altitude of the antenna.
10) - M denotes units of altitude (eg. Meters or Feet)
11) - 29.200 denotes the geoidal separation (subtract this from the altitude of the antenna to arrive at the Height Above Ellipsoid (HAE).
12) - M denotes the units used by the geoidal separation.
13) - 1.0 denotes the age of the correction (if any).
14) - 0000 denotes the correction station ID (if any).
15) - *40 denotes the checksum.

ref:
https://www.gpsworld.com/what-exactly-is-gps-nmea-data/
*/