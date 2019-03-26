﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum SelectedTool
{
	NONE,
	HAND,
	FOOD,
	BRUSH,
	LASER_POINTER
}

// Behavior script for the cat. Manages the cats behaviors, stats, and personality
public class Cat : BaseCat
{
	// The cat's current activity/behavior/goal
	CatActivity activity;
	
	NavMeshAgent agent;
	
	// Cat AI
	BehaviorTree autonomousCatBehaviorTree;
	BehaviorTree userInteractionBehaviorTree;
	Context contextObject;
	
	// Petting / brushing / summoning related variables
	Vector3 inFrontOfUserPosition;
	bool is_drag;
	double drag_start_time;
	public float time_of_last_user_interaction {get; private set;}
	
	// UI Buttons
	private Button hand_button, brush_button, food_button, laser_button, liter_button;
	public SelectedTool selected_tool {get; private set;}
	public Texture2D hand_cursor;
	public Texture2D brush_cursor;
	public Texture2D food_cursor;
	public Texture2D laser_cursor;

	// Laser pointer GameObject
	GameObject laserPointer;
	// Laser pointer GameObject's script
	LaserPointer laserPointerScript;
	
	float last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		// Initialize agent
		agent = GetComponent<NavMeshAgent>();
		
		// Cat position for petting / brushing
		inFrontOfUserPosition = new Vector3(0F, -0.5F, -5F);
		
		// Get the laser pointer GameObject
		laserPointer = GameObject.Find("Laser Pointer");
		// Get the laser pointer GameObject's attached script
		laserPointerScript = laserPointer.GetComponent<LaserPointer>();
		// Deactivate laser pointer game object to turn it off
		laserPointer.SetActive(false);
		
		// Initialize Buttons
		hand_button = GameObject.Find("hand_button").GetComponent<Button>();
		brush_button = GameObject.Find("brush_button").GetComponent<Button>();
		food_button = GameObject.Find("food_button").GetComponent<Button>();
		laser_button = GameObject.Find("laser_button").GetComponent<Button>();
		hand_button.onClick.AddListener(delegate {SelectTool(SelectedTool.HAND);});
		brush_button.onClick.AddListener(delegate {SelectTool(SelectedTool.BRUSH);});
		food_button.onClick.AddListener(delegate {SelectTool(SelectedTool.FOOD);});
		laser_button.onClick.AddListener(delegate {SelectTool(SelectedTool.LASER_POINTER);});
		SelectTool(SelectedTool.HAND);

		// Previous save should always exist (otherwise adoption center would be loaded)
		// If for some reason we got here, crash.
		if (!GameSave.Valid()) {
			Debug.Log("LivingRoom loaded without a save! Quiting.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
			return;
#endif
		}

		// Load previous save / adopted cat
		Load();

		// Start off idle
		activity = new CatActivity( CatActivityEnum.Idle );
		
		// Initialize variables needed by behavior tree nodes
		contextObject = new Context( gameObject, ref personality, ref stats, ref activity );
		getPointDelegate getPointDel = laserPointerScript.getLaserIntersectionPoint;
		
		// Construct the cat's behavior tree
        autonomousCatBehaviorTree = new BehaviorTree(	new SelectorNode	( 	contextObject,
		
																				/* Energy Sequence */				new SequenceNode 	(	contextObject, 
																																			new CheckEnergyNode ( contextObject ),
																																			new SleepNode ( contextObject )
																																		),
																				/* Hunger Sequence */				new SequenceNode 	( 	contextObject,
																																			new CheckFullnessNode ( contextObject ),
																																			new CheckObjectStatusNode ( contextObject, GameObject.Find("food_in_bowl") ),
																																			new GoToObjectNode ( contextObject, GameObject.Find("Food Bowl") ),
																																			new EatNode ( contextObject, GameObject.Find("food_in_bowl") )
																																		),
																				/* Chase Laser Pointer Sequence */	new SequenceNode 	( 	contextObject,
																																			new CheckObjectStatusNode ( contextObject, laserPointer ),
																																			new GoToDynamicPointNode ( contextObject, getPointDel ),
																																			new ChaseLaserNode ( contextObject )
																																		),
																				/* Wandering Sequence */			new SequenceNode 	(	contextObject,
																																			new WaitNode ( contextObject, 5F),
																																			new GoToRandomPointNode ( contextObject )
																																		)
																			)
													);
		autonomousCatBehaviorTree.paused = false;
										
		userInteractionBehaviorTree = new BehaviorTree ( new SequenceNode 	( 	contextObject,
																				new GoToPointNode ( contextObject, inFrontOfUserPosition ),
																				new FocusOnUserNode ( contextObject, 7F )
																			)
														);
		userInteractionBehaviorTree.paused = true;
		
		
		// Initialize last update time to now
		last_update_time = Time.time;
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
		if (Input.GetKeyDown (KeyCode.R)) {
			CreateNew();
			GameSave.Clear();
		}

		// Calcuate time delta since last update, in seconds, fps-independent calculations
		float dt = Time.time - last_update_time;
		last_update_time = Time.time;

		// Update cat stats with current state
		personality.UpdateStats(ref stats, activity, dt);

		// Update UI
		stats.UpdateUI();

		// Run behavior tree
		autonomousCatBehaviorTree.run(Time.time);
		userInteractionBehaviorTree.run(Time.time);
		
		// If cat is currently interacting with user, the camera should follow the cat
		if (userInteractionBehaviorTree.paused == false)
		{
			Camera.main.transform.LookAt(gameObject.transform); // Main camera look at cat
		}

		// Log current state
		Debug.Log(activity);
        //Debug.Log(stats);
    }

	// Change the currently selected tool
	void SelectTool(SelectedTool tool)
	{
		// If no change, nothing to do
		if (tool == selected_tool) return;
		
		// If selected tool is not the laser pointer, ensure the laser pointer GameObject is turned off
		if (tool != SelectedTool.LASER_POINTER)
		{
			laserPointer.SetActive(false);
		}
		
		// Log the change in tool
		Debug.Log(string.Format("Selected Tool {0}", tool));
		
		Vector2 offset = new Vector2(0, 32);
		
		if (SelectedTool.HAND == tool)
		{
			Cursor.SetCursor(hand_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.BRUSH == tool) 
		{
			Cursor.SetCursor(brush_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.FOOD == tool) 
		{
			Cursor.SetCursor(food_cursor, offset, CursorMode.Auto);
		} 
		else if (SelectedTool.LASER_POINTER == tool) 
		{
			Cursor.SetCursor(laser_cursor, offset, CursorMode.Auto);
			laserPointer.SetActive(true);
		}

		selected_tool = tool;
	}
	
	void OnMouseDown ()
	{
		Debug.Log("Clicked on cat.");
		
		// If mouse just went down, and the user is currently petting or brushing the cat, start counting drag time
		if ((!is_drag) && (SelectedTool.HAND == selected_tool || SelectedTool.BRUSH == selected_tool)) {
			is_drag = true;
			drag_start_time = Time.time;
		}

 	}

 	void OnMouseUp ()
	{
		// When mouse released, act based on accumulated drag
		is_drag = false;
		double drag_time = Time.time - drag_start_time;
		time_of_last_user_interaction = Time.time;

 		// A short drag is registered as a click, causing cat to begin user interaction behaviors
		if (drag_time < 0.1) 
		{
			// Switch behavior trees
			turnOnUserInteractionCatBehavior();

 		}
		// Else if cat is in front of user...
		else if ( (GetComponent<Transform>().position - inFrontOfUserPosition).magnitude <= agent.stoppingDistance )
		{
			// If using hand tool, register as petting
			if (selected_tool == SelectedTool.HAND)
			{
				activity.current = CatActivityEnum.BeingPet;
			}
			// If using brush tool, register as brushing
			else if (selected_tool == SelectedTool.BRUSH)
			{
				activity.current = CatActivityEnum.BeingBrushed;
			}
		}


 	}

 	// Pause one behavior tree and activate the other
	public void turnOnAutonomousCatBehavior()
	{
		autonomousCatBehaviorTree.paused = false;
		userInteractionBehaviorTree.paused = true;
		
		Camera.main.GetComponent<CameraScript>().Reset();
	}

 	public void turnOnUserInteractionCatBehavior()
	{
		autonomousCatBehaviorTree.paused = true;
		userInteractionBehaviorTree.paused = false;
		
		Camera.main.transform.LookAt(gameObject.transform); // Main camera look at cat
	}
}
