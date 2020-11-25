using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This is an example Event class. It is to be called when the space bar has
/// been pressed. Avoid the use of the word "On" in the event name
/// (i.e. OnSpacePressedEvent).
/// </summary>
public static class SpacePressedEvent
{
	/// <summary>
	/// The SpacePressedEvent handler. It has the same name as the event
    /// class with the addition of "Handler".   
	/// </summary>
	public static event EventHandler<EventArgs> SpacePressedEventHandler;
	/// <summary>
	/// The getter for a SpacePressedEvent handler. This is necessary because
    /// the handler cannot be referenced by other scripts directly. It has the
    /// same name as the event handler with the addition of the word "Get".
	/// </summary>
	/// <returns>The SpacePressedEvent handler.</returns>
	public static EventHandler<EventArgs> GetSpacePressedEventHandler()
	{
		return SpacePressedEventHandler;
	}
}

/// <summary>
/// The arguments for a SpacePressedEvent. It has the same name as the
/// associated event with the addition of "Args" at the end.
/// </summary>
public class SpacePressedEventArgs : EventArgs 
{
    public int spaceCount { get; set; }
	/// <summary>
	/// Constructor for SpacePressedEventArgs.
	/// </summary>
	/// <param name="spaceCount">
	/// The number of times the sapce bar has been pressed.
	/// </param>
	public SpacePressedEventArgs(int spaceCount)
	{
		this.spaceCount = spaceCount;
	}
}
}