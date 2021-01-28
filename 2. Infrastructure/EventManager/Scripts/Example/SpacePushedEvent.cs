using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when space bar is pressed.
/// </summary>
public static class SpacePressedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> SpacePressedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return SpacePressedEventHandler;
	}
}

/// <summary>
/// The arguments for a SpacePressedEvent.
/// </summary>
public class SpacePressedEventArgs : EventArgs 
{
	/// <summary>
	/// The number of times the space bar has been pressed
	/// </summary>
    public int spaceCount { get; set; }

	// Constructor
	public SpacePressedEventArgs(int spaceCount)
	{
		this.spaceCount = spaceCount;
	}
}
}