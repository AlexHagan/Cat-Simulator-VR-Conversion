using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementForVR : MonoBehaviour
{
	Vector3 startPosition;
	Vector3 catInteractionPosition;
	public Vector3 goalPosition;

	float speed = 1.0F;
	Cat catScript; 	// Reference to cat class attached to Cat gameobject
	public bool currentlyMoving {get; private set;}

    // Start is called before the first frame update
    void Start()
    {	
    	startPosition = transform.position;
    	catInteractionPosition = new Vector3(-0.24F, 0.5F, -10.2F);
    	catScript = GameObject.Find("Cat").GetComponent<Cat>();

    	currentlyMoving = false;
    }

    /*
    public IEnumerator moveCameraToNormalPosition()
    {
    	// If camera + controllers are not at normal position...
		while (Vector3.Distance(transform.position, startPosition) > 0.001F)
	    {
	    	currentlyMoving = true;
	        Debug.Log(string.Format("Moving camera to original position, {0}...", startPosition));
			// Move camera + controller gameobjects closer to starting position.
			float step =  speed * Time.deltaTime; // calculate distance to move
    		transform.position = Vector3.MoveTowards(transform.position, catInteractionPosition, step);
	        yield return null;
	    }

	    currentlyMoving = false;
    }

    public IEnumerator moveCameraToUserInteractionPosition()
    {
    	while (Vector3.Distance(transform.position, catInteractionPosition) > 0.001F)
		{
			currentlyMoving = true;
			Debug.Log(string.Format("Moving camera to cat interaction position, {0}...", catInteractionPosition));
			// Move camera + controller gameobjects closer to the target.
    		float step =  speed * Time.deltaTime; // calculate distance to move
    		transform.position = Vector3.MoveTowards(transform.position, catInteractionPosition, step);
    		yield return null;
		}

		currentlyMoving = false;
    }

    public IEnumerator moveCameraToGoalPosition()
    {

    }
    */
}
