
// Template controller handler for enabling Vive controller interactions with game objects. 
// Attach this script to the object you want to be interactable with the raycast pointer, 
// and edit the functions below as needed.

// Template code found in the Vive Input Utility Developer's Manual: https://usermanual.wiki/Pdf/guide.2092279436.pdf

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;



public class Template_ControllerHandler : MonoBehaviour
										, IPointerEnterHandler
 										, IPointerExitHandler
 										, IPointerClickHandler
{
	private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();

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
		}
		else if (eventData.button == PointerEventData.InputButton.Left)
		{
			// Standalone button pressed
		}
	}

}