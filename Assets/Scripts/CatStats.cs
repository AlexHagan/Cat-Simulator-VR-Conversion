using System;
using UnityEngine;
using UnityEngine.UI;

// Defines the current stats of the cat, which are displayed as bars in the HUD and influence behavior
public class CatStats
{
	public CatStats(float energy = MAX, float fullness = MAX, float fun = MAX, float hygiene = MAX, float bladder = MAX, float bond = MAX)
	{
		this.energy = energy;
		this.fullness = fullness;
		this.fun = fun;
		this.hygiene = hygiene;
		this.bladder = bladder;
		this.bond = bond;

		if (AdoptionCenter.IsActive()) {
			return;
		}

		this.energy_slider = GameObject.Find("energy_slider").GetComponent <Slider> ();
		this.fullness_slider = GameObject.Find("food_slider").GetComponent <Slider> ();
		this.hygiene_slider = GameObject.Find("hygiene_slider").GetComponent <Slider> ();
		this.fun_slider = GameObject.Find("fun_slider").GetComponent <Slider> ();
		this.happy_indicator = GameObject.Find("happy_indicator").GetComponent<Renderer>();
		this.meow_sound = GameObject.Find("meow_sound").GetComponent<AudioSource>();
	}

	// Maximum value of each individual stat
	public const float MAX = 1.0F;
	// Minimimu value of each indiviual stat
	public const float MIN = 0.0F;
	public const float HAPPIENESS_INDICATOR_THRESHOLD = MAX * 0.75F;
	public const float MEOW_PERIOD = 10F;

	// How energetic the cat is, when maximized cat has no desire for sleep
	private float energy;
	public float Energy {get {return energy;} set {energy = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// Inverse of hunger, when maximized cat has no desire for food
	private float fullness;
	public float Fullness {get {return fullness;} set {fullness = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How much fun cat is having, when maximized cat is having, like, a lot of fun
	private float fun;
	public float Fun {get {return fun;} set {fun = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How clean the cat and its environment is. When maximized, cat is content with its hygiene
	private float hygiene;
	public float Hygiene {get {return hygiene;} set {hygiene = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How much the cat needs to use the bathroom
	private float bladder;
	public float Bladder {get {return bladder;} set {bladder = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How connected cat is with the owner
	private float bond;
	public float Bond {get {return bond;} set {bond = Math.Min(MAX, Math.Max(value, MIN));}}

	// UI sliders
	private Slider energy_slider;
	private Slider fullness_slider;
	private Slider fun_slider;
	private Slider hygiene_slider;
	// Happineness indicator
	private Renderer happy_indicator;
	private AudioSource meow_sound;
	private float last_meow;

	// The total happieness/contentness of the cat. When == MAX, all desires of cat are satisfied
	public float happieness()
	{
		return (energy + fullness + fun + hygiene + bladder) / (5.0F * MAX);
	}

	// Update the stat bars with the current stats
	public void UpdateUI()
	{
		this.energy_slider.value = energy;
		this.fullness_slider.value = fullness;
		this.fun_slider.value = fun;
		this.hygiene_slider.value = hygiene;
		if (happieness() > HAPPIENESS_INDICATOR_THRESHOLD) {
			this.happy_indicator.enabled = true;
			if (Time.time - last_meow > MEOW_PERIOD) {
				meow_sound.Play();
				last_meow = Time.time;
			}
		} else {
			this.happy_indicator.enabled = false;
		}
	}

	public override string ToString()
	{
		return string.Format("CatStats(Energy={0} Fullness={1} Fun={2} Hygiene={3} Bladder={4} Bond={5} Happieness={6})",
							 energy, fullness, fun, hygiene, bladder, bond, happieness());
	}
	
	public void Save()
	{
		PlayerPrefs.SetFloat("stats.energy", Energy);
		PlayerPrefs.SetFloat("stats.fullness", Fullness);
		PlayerPrefs.SetFloat("stats.fun", Fun);
		PlayerPrefs.SetFloat("stats.hygiene", Hygiene);
		PlayerPrefs.SetFloat("stats.bladder", Bladder);
		PlayerPrefs.SetFloat("stats.bond", Bond);
	}
	
	public static CatStats Load()
	{
		return new CatStats(PlayerPrefs.GetFloat("stats.energy"),
		                    PlayerPrefs.GetFloat("stats.fullness"),
							PlayerPrefs.GetFloat("stats.fun"),
							PlayerPrefs.GetFloat("stats.hygiene"),
							PlayerPrefs.GetFloat("stats.bladder"),
							PlayerPrefs.GetFloat("stats.bond"));
	}
}