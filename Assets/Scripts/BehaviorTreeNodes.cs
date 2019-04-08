using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;




// Primitives:
// Nodes that perform an action or check a condition
public class PrimitiveNode : Node
{
	public PrimitiveNode (Context _context) : base (_context)
	{
		
	}
	
	// Primitive Nodes will not have children
	public override List<Node> getChildren()
	{
		return null;
	}
}

// TODO: test this
public class CustomCheckNode : PrimitiveNode
{
	public delegate NodeStatus customCheckFunction();
	customCheckFunction function;
	
	public CustomCheckNode (Context _context, customCheckFunction _function) : base (_context)
	{
		function = _function;
	}
	
	public override NodeStatus run (float _time)
	{
		return function();
	}
}

public class CheckTimerNode : PrimitiveNode
{
	public delegate void TimesUpFunction();
	TimesUpFunction endTimerFunction;
	
	bool firstIteration;
	float startTime;
	float timerDuration;
	
	public CheckTimerNode (Context _context, float _duration, TimesUpFunction _function) : base (_context)
	{
		firstIteration = true;
		timerDuration = _duration;
		endTimerFunction = _function;
	}
	
	public override NodeStatus run (float _time)
	{	
		// If node is being run for the first time, start the timer
		if (firstIteration)
		{
			firstIteration = false;
			startTime = _time;
		}
		
		if (_time >= startTime + timerDuration)
		{
			endTimerFunction();
			
			return NodeStatus.Failure;
		}
		
		return NodeStatus.Success;
	}
}


// Decorators
public class LoopNode : Node
{
	// I'm guessing a loop node is supposed to repeatedly run its child until it succeeds.
	// LoopNodes will only ever have one child.
	
	Node child;
	
	public LoopNode (Context _context) : base (_context)
	{
		child = null;
	}
	
	public LoopNode (Context _context, Node _child) : base (_context)
	{
		child = _child;
	}
	
	public override NodeStatus run(float _time)
	{
		if (child.run(Time.time) != NodeStatus.Success) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	public void addChild(Node _child)
	{
		child = _child;
	}
	
	public override List<Node> getChildren()
	{
		List<Node> childrenList = new List<Node>();
		childrenList.Add(child);
		
		return childrenList;
	}
}

public class WaitNode : Node
{ 
	public float waitTime {get; private set;} // The amount of time (in seconds) that the node should wait
	float startTime; // The time at which the node started waiting
	bool startTimeSet;
	
	public WaitNode (Context _context, float _waitTime) : base (_context)
	{
		waitTime = _waitTime;
		startTime = 0F;
		startTimeSet = false;
	}
	
	// WaitNode will return Success after waiting x number of seconds.
	public override NodeStatus run(float _time)
	{
		if (startTimeSet == false)
		{
			startTime = Time.time;
			startTimeSet = true;
		}
		
		if (startTime + waitTime > Time.time) 
		{
			return NodeStatus.Running;
		}
		// else...
		
		// wait time has elapsed, so reset variables and return Success
		startTimeSet = false;
		return NodeStatus.Success;
	}
	
}

public class InverterNode : Node
{
	Node child;
	
	public InverterNode( Context _context ) : base (_context)
	{
		child = null;
	}
	
	public InverterNode ( Context _context, Node _child ) : base (_context)
	{
		child = _child;
	}
	
	public override NodeStatus run(float _time)
	{
		NodeStatus result;
		result = child.run(Time.time);
			
		if (result == NodeStatus.Success) {
			return NodeStatus.Failure;
		}	
		if (result == NodeStatus.Failure) {
			return NodeStatus.Success;
		}
		// If result == NodeStatus.Running...
		return result;
		
	}
	
	public void addChild(Node _child) 
	{
		child = _child;
	}
	
	public override List<Node> getChildren()
	{
		List<Node> childrenList = new List<Node>();
		childrenList.Add(child);
		
		return childrenList;
	}
}

// Composites

// Runs child nodes in sequence, one after the other, until either a.) all of them have run successfully, in which case the SequenceNode returns Success, or b.) one of the child nodes runs and returns Failure, in which case the SequenceNode also returns failure.
public class SequenceNode : Node
{
	List<Node> children;
	NodeStatus result;
	
	public SequenceNode( Context _context ) : base (_context)
	{
		children = new List<Node>();
	}
	
	// "params" means that this function accepts a variable number of Node objects as its argument. When using this constructor, pass Nodes in a comma separated list.
	public SequenceNode ( Context _context, params Node[] _children ) : base (_context)
	{
		children = new List<Node>(_children);
	}
	
	
	public override NodeStatus run(float time) 
	{
		// If no children, return Failure
		if (children.Count == 0) 
		{
			return NodeStatus.Failure;
		}
		
		foreach (Node child in children)
		{
			result = child.run(Time.time);
			
			if (result == NodeStatus.Failure)
			{
				return NodeStatus.Failure;
			}
			if (result == NodeStatus.Running)
			{
				return NodeStatus.Running;
			}
			
		}
		
		return NodeStatus.Success;
	}
	
	public override List<Node> getChildren()
	{
		return children;
	}
	
	public void addChild(Node _child)
	{
		children.Add(_child);
	}
	
	
}

// Selectors will run each of its children until one of them succeeds, in which case the Selector will return Success. If no children succeed, the Selector will return Failure
public class SelectorNode : Node
{
	List<Node> children;
	NodeStatus result;
	
	public SelectorNode( Context _context ) : base (_context)
	{
		children = new List<Node>();
	}
	
	// "params" means that this function accepts a variable number of Node objects as its argument. When using this constructor, pass Nodes in a comma separated list.
	public SelectorNode ( Context _context, params Node[] _children ) : base (_context)
	{
		children = new List<Node>(_children);
	}
	
	public override NodeStatus run(float _time)
	{
		// If no children, return Failure
		if (children.Count == 0) 
		{
			return NodeStatus.Failure;
		}
		
		foreach (Node child in children)
		{
			result = child.run(Time.time);
			
			if (result == NodeStatus.Success)
			{
				return NodeStatus.Success;
			}
			if (result == NodeStatus.Running)
			{
				return NodeStatus.Running;
			}
		}
		
		return NodeStatus.Failure;
		
	}
	
	public override List<Node> getChildren()
	{
		return children;
	}
	
	public void addChild(Node _child)
	{
		children.Add(_child);
	}
}
/*
// TODO:
// Implement this. Requires the use of threading. 

public class ParallelNode : Node
{
	
}
*/

