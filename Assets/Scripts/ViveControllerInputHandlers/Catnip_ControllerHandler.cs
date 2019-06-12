// Template code found in the Vive Input Utility Developer's Manual: https://usermanual.wiki/Pdf/guide.2092279436.pdf

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;



public class Catnip_ControllerHandler : MonoBehaviour
										, IPointerEnterHandler
 										, IPointerExitHandler
 										, IPointerClickHandler
{
	private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
	Cat catScript; 									// Reference to cat class attached to Cat gameobject
	Text tooltip_text;
	public const float CATNIP_TIME_DURATION = 60F; 	// How long catnip effects will last, in seconds.
	public GameObject UI_Effects;					// Visual effects to call user's attention to catnip

	public void Start()
	{
		tooltip_text = GameObject.Find("CatnipToolTipText").GetComponent<Text>();
        catScript = GameObject.Find("Cat").GetComponent<Cat>();
		UI_Effects = GameObject.Find("UIEffects");
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hovers.Add(eventData) && hovers.Count == 1)
		{
			tooltip_text.text = "Feed Catnip";
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (hovers.Remove(eventData) && hovers.Count == 0)
		{
			tooltip_text.text = "";
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.IsViveButton(ControllerButton.Trigger))
		{
			// Trigger button pressed
			Debug.Log("Clicked on catnip.");
			UI_Effects.SetActive(false);		// Turn off visual effects
			
			// If not currently on catnip, use catnip
			if (!catScript.on_catnip)
			{
				StartCoroutine(catScript.useCatnip());
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Left)
		{
			// Standalone button triggered!
		}
	}

}