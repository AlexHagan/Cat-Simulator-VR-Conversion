// Template code found in the Vive Input Utility Developer's Manual: https://usermanual.wiki/Pdf/guide.2092279436.pdf

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using HTC.UnityPlugin.Vive;



public class RecordPlayer_ControllerHandler : MonoBehaviour
										, IPointerEnterHandler
 										, IPointerExitHandler
 										, IPointerClickHandler
{
	private HashSet<PointerEventData> hovers = new HashSet<PointerEventData>();
	public AudioSource backgroundMusic;
	GameObject particleEffects;
	Text tooltip_text;

	void Start()
    {
		tooltip_text = GameObject.Find("RecordPlayerToolTipText").GetComponent<Text>();
		backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
		particleEffects = GameObject.Find("MusicParticles");
    }

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hovers.Add(eventData) && hovers.Count == 1)
		{
			tooltip_text.text = "Play/Pause Music";
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
			Debug.Log("Music toggled on/off.");
	        if (!backgroundMusic.isPlaying)
	        {
	           backgroundMusic.Play();
			   particleEffects.SetActive(true);
	        } 
	        else 
	        {
				backgroundMusic.Stop();
				particleEffects.SetActive(false);
			}
		}
		else if (eventData.button == PointerEventData.InputButton.Left)
		{
			// Standalone button pressed
		}
	}

}