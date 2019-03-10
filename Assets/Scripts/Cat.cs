﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Behavior script for the cat. Manages the cats behaviors, stats, and personality
public class Cat : MonoBehaviour
{
	// The cat's current stats, which appear on the HUD bars
	CatStats stats;
	// The cats permenant personality, initialized randomly
	CatPersonality personality;
	// The cat's current activity/behavior/goal
	CatActivity activity;
	// Tracks last time Update() was called for dt calculation
	BehaviorTree behaviorTree;
	Context contextObject;
	
	// Variales for waundering functionality
	// Control's cats movement
	NavMeshAgent agent;
	// Last time cat has waundered
	float last_waunder_time;
	const float WAUNDER_PERIOD_SECONDS = 5F;
	
	float last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		// Initialize agent
		agent = GetComponent<NavMeshAgent>();

		// If a previous save exists, load it
		if (PlayerPrefs.HasKey("savetime")) {
			Debug.Log("Previous save found, loading");
			Load();
		// Otherwise create a new random cat and save it
		} else {
			Debug.Log("No previous save found, creating a cat");
			CreateNew();
			Save();
		}

		// Start off idle
		activity = new CatActivity( CatActivityEnum.Idle );
		
		contextObject = new Context( gameObject, ref personality, ref stats, ref activity );
		// Construct the cat's behavior tree
        behaviorTree = new BehaviorTree( new SequenceNode ( contextObject, 
																		new CheckEnergyNode ( contextObject ),
																		new SleepNode ( contextObject )
														)
										
										);
		
		
		// Initialize last update time to now
		last_update_time = Time.time;
		
		Debug.Log(personality);
    }
	
	// Called when there is no save to generate a new random cat
	void CreateNew()
	{	// Initialize stats for a completely content cat
		stats = new CatStats();
		// Initialize personality to random values
		personality = CatPersonality.RandomPersonality();
	}

    // Update is called once per frame
    void Update()
    {
		// TODO: autosave
		//  Save game when S is pressed
        if(Input.GetKeyDown (KeyCode.S))
            Save();
		// Load game when L is pressed
        if (Input.GetKeyDown (KeyCode.L))
            Load();
		// If R is pressed, reset
		if (Input.GetKeyDown (KeyCode.R))
			CreateNew();

		// Calcuate time delta since last update, in seconds, fps-independent calculations
		float dt = Time.time - last_update_time;
		last_update_time = Time.time;

		// Update cat stats with current state
		personality.UpdateStats(ref stats, activity, dt);

		// Update UI
		stats.UpdateUI();
		
		// TODO: change activity
		behaviorTree.run(Time.time);
		
		// Carry out behavior based on current behavior
		if (CatActivityEnum.Idle == activity.current) {
			if (Time.time - last_waunder_time > WAUNDER_PERIOD_SECONDS) {
				agent.destination = RandomWaypoint();
				last_waunder_time = Time.time;
			}
		}

		// Log current state
		Debug.Log(activity);
        Debug.Log(stats);
    }
	
	Vector3 RandomWaypoint()
	{
		return new Vector3(Random.Range(-20F, 20F),
		                   Random.Range(-20F, 20F),
						   0);
	}

	// Load the cat from a previous save
	public void Load()
	{
		// Load personality and stats
		personality = CatPersonality.Load();
		stats = CatStats.Load();
		// Load cat pose
		Quaternion r = new Quaternion(PlayerPrefs.GetFloat("pose.r.x"),
			PlayerPrefs.GetFloat("pose.r.y"),
			PlayerPrefs.GetFloat("pose.r.z"),
			PlayerPrefs.GetFloat("pose.r.w"));
		Vector3 p = new Vector3(PlayerPrefs.GetFloat("pose.p.x"),
			PlayerPrefs.GetFloat("pose.p.y"),
			PlayerPrefs.GetFloat("pose.p.z"));
		gameObject.transform.SetPositionAndRotation(p, r);
		// TODO: color, other info
		
		Debug.Log("--- LOADED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log("-------------");
	}

	// Save the current cat to a file for later resuming play
	public void Save()
	{
		// Save personality and stats
		personality.Save();
		stats.Save();
		// Save pose
		PlayerPrefs.SetFloat("pose.p.x",gameObject.transform.position.x);
		PlayerPrefs.SetFloat("pose.p.y",gameObject.transform.position.y);
		PlayerPrefs.SetFloat("pose.p.z",gameObject.transform.position.z);
		PlayerPrefs.SetFloat("pose.r.x",gameObject.transform.rotation.x);
		PlayerPrefs.SetFloat("pose.r.y",gameObject.transform.rotation.y);
		PlayerPrefs.SetFloat("pose.r.z",gameObject.transform.rotation.z);
		PlayerPrefs.SetFloat("pose.r.w",gameObject.transform.rotation.w);

		// TODO: save pose, color, other info
		PlayerPrefs.SetFloat("savetime", Time.time);
		PlayerPrefs.Save();

		Debug.Log("--- SAVED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log("-------------");
	}
}
