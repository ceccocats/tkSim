using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeloLidar : MonoBehaviour {

	public bool debugRay = false;
    public float maxAngle = 10;
    public float minAngle = -10;
    public int numberOfLayers = 16;
    public int numberOfIncrements = 360;
    public float maxRange = 100f;
    public float period = 0.1f;

    float vertIncrement;
    float azimutIncrAngle;

    [HideInInspector]
    public Vector3[] points;

    // Use this for initialization
    void Start () {
        points = new Vector3[numberOfLayers* numberOfIncrements];
        vertIncrement = (float)(maxAngle - minAngle) / (float)(numberOfLayers - 1);
        azimutIncrAngle = (float)(360.0f / numberOfIncrements);
    }


	public void scan() {
        Vector3 nopoint = new Vector3(0, 0, 0);
        Vector3 fwd = new Vector3(0, 0, 1);
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;
        float angle;

        //azimut angles
        for (int incr = 0; incr < numberOfIncrements; incr++)
        {
            for (int layer = 0; layer < numberOfLayers; layer++)
            {
                //print("incr "+ incr +" layer "+layer+"\n");
                indx = layer + incr * numberOfLayers;
                angle = minAngle + (float)layer * vertIncrement;
                float azimut = incr * azimutIncrAngle;
                dir = transform.rotation * Quaternion.Euler(-angle, azimut, 0)*fwd;

                if (Physics.Raycast(transform.position, dir, out hit, maxRange))
                {
                    points[indx] = transform.InverseTransformPoint(hit.point);
                }
                else
                {
                    points[indx] = nopoint;
                }
				if(debugRay)
					Debug.DrawRay(transform.position, dir * hit.distance, Color.green);

            }
        }

    }
}
