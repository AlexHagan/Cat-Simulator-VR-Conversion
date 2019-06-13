using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
	LineRenderer laserRenderer;
	Vector3 laserIntersectionPoint;
	GameObject player;
	public GameObject rightLaserReticle;
	public GameObject rightHandGameObject;
	public GameObject leftHandGuideline;
	public GameObject rightHandGuideline;
	
    // Start is called before the first frame update
    void Start()
    {
		laserRenderer = GetComponent<LineRenderer>();
		player = GameObject.Find("Player");
    }

    // LateUpdate is called after all Update functions are called
    void LateUpdate()
    {
		//RaycastHit hit;
		//Ray ray = new Ray(player.transform.position, (rightLaserReticle.transform.position - player.transform.position).normalized);

		// Update laser visualization
		// First point is laser origin, second point is laser destination
		Vector3[] linePoints = {rightHandGameObject.transform.position,	rightLaserReticle.transform.position};
			
		laserIntersectionPoint = rightLaserReticle.transform.position;
	 
		laserRenderer.positionCount = linePoints.Length;
		laserRenderer.SetPositions(linePoints);
    }
	
	public Vector3 getLaserIntersectionPoint ()
	{
		return new Vector3(laserIntersectionPoint.x, laserIntersectionPoint.y, laserIntersectionPoint.z);
	}
	
}

