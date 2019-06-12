
// Template controller handler for enabling Vive controller interactions with game objects. 
// Attach this script to the object you want to be interactable with the raycast pointer, 
// and edit the functions below as needed.

// Template code found in the Vive Input Utility Developer's Manual: https://usermanual.wiki/Pdf/guide.2092279436.pdf

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;


public class Cat_ControllerHandler : MonoBehaviour
 										, IPointerClickHandler
{
	private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
	Cat catScript; 	// Reference to cat class attached to Cat gameobject
	NavMeshAgent agent;

	// Variables to detect if the cat is being petted / summoned / brushed
	Vector3 inFrontOfUserPosition;
	bool is_drag;
	double drag_start_time;

	void Start()
	{
		catScript = GameObject.Find("Cat").GetComponent<Cat>();
		agent = GetComponent<NavMeshAgent>();
		is_drag = false;
	}

	void Update()
	{


	}

	// Click on the cat to summon it
	public void OnPointerClick(PointerEventData eventData)
	{
		if ((eventData.IsViveButton(ControllerButton.Trigger)) && (SelectedTool.HAND == catScript.selected_tool || SelectedTool.BRUSH == catScript.selected_tool))
		{
			Debug.Log("Controller clicked on cat.");
			catScript.turnOnUserInteractionCatBehavior();
		}
	}

	void OnCollisionExit(Collision collisionInfo)
	{
		Debug.Log(string.Format("Collision with {0} exited.", collisionInfo.gameObject.name));
		
		if (collisionInfo.gameObject.CompareTag("GameController"))
		{
			petOrBrushCat();
		}
	}

	public void petOrBrushCat()
	{
		// If using hand tool, register as petting
		if (catScript.selected_tool == SelectedTool.HAND)
		{
			catScript.activity.current = CatActivityEnum.BeingPet;
			catScript.achievements.num_pets++;
			Debug.Log("Petted cat with VR controller.");
		}
		// If using brush tool, register as brushing
		else if (catScript.selected_tool == SelectedTool.BRUSH)
		{
			catScript.activity.current = CatActivityEnum.BeingBrushed;
			catScript.achievements.num_brushes++;
			Debug.Log("Brushed cat with VR controller.");
		}
	}


}