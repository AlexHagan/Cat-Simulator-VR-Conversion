// Template code found in the Vive Input Utility Developer's Manual: https://usermanual.wiki/Pdf/guide.2092279436.pdf

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;



public class Litterbox_ControllerHandler : MonoBehaviour
, IPointerEnterHandler
, IPointerExitHandler
, IPointerClickHandler
{
	private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
	GameObject[] poops;
	Cat catScript;

	public void Awake()
	{
		 poops = GameObject.FindGameObjectsWithTag("poop");
		 catScript = GameObject.Find("Cat").GetComponent<Cat>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		 if (hovers.Add(eventData) && hovers.Count == 1)
		 {
			  //Debug.Log("Hovering over object.");
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
			  if (catScript.selected_tool == SelectedTool.LITTER_SCOOPER)
			  {
					// Find first active poop
					foreach (GameObject poop in poops)
					{
						 if (poop.activeSelf == true)
						 {
							  catScript.achievements.litter_box_cleaned += 1;
							  poop.SetActive(false);
							  return;
						 }
					}
			  }
		 }
		 else if (eventData.button == PointerEventData.InputButton.Left)
		 {
			  // Standalone button triggered!
		 }
	}

}