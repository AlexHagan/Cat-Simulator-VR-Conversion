
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
										, IPointerEnterHandler
 										, IPointerExitHandler
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
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hovers.Add(eventData) && hovers.Count == 1)
		{
			// turn to highlight state
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (hovers.Remove(eventData) && hovers.Count == 0)
		{
			// turn to normal state
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.IsViveButton(ControllerButton.Trigger))
		{
			// Trigger button pressed
			
			Debug.Log("Controller clicked on cat.");
		
			// If trigger just went down, and the user is currently petting or brushing the cat, start counting drag time
			if ((!is_drag) && (SelectedTool.HAND == catScript.selected_tool || SelectedTool.BRUSH == catScript.selected_tool)) {
				is_drag = true;
				drag_start_time = Time.time;
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Left)
		{
			// Standalone button pressed
		}
	}

	public void OnPointerRelease(PointerEventData eventData)
	{
		// When mouse released, act based on accumulated drag
		is_drag = false;
		double drag_time = Time.time - drag_start_time;
		catScript.time_of_last_user_interaction = Time.time;

 		// A short drag is registered as a click, causing cat to begin user interaction behaviors
		if (drag_time < 0.1) 
		{
			// Switch behavior trees
			catScript.turnOnUserInteractionCatBehavior();

 		}
		// Else if cat is in front of user...
		else if ( (GetComponent<Transform>().position - catScript.inFrontOfUserPosition).magnitude <= agent.stoppingDistance )
		{
			// If using hand tool, register as petting
			if (catScript.selected_tool == SelectedTool.HAND)
			{
				catScript.activity.current = CatActivityEnum.BeingPet;
				catScript.achievements.num_pets++;
			}
			// If using brush tool, register as brushing
			else if (catScript.selected_tool == SelectedTool.BRUSH)
			{
				catScript.activity.current = CatActivityEnum.BeingBrushed;
				catScript.achievements.num_brushes++;
			}
		}


	}

}