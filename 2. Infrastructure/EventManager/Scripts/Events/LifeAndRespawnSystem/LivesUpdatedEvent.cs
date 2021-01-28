using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when squad lives are updated.
/// </summary>
public static class LivesUpdatedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> LivesUpdatedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return LivesUpdatedEventHandler;
	}
}

/// <summary>
/// The arguments for a LivesUpdatedEvent.
/// </summary>
public class LivesUpdatedEventArgs : EventArgs 
{
	/// <summary>
	/// The new value for lives.
	/// </summary>
    public int newLives { get; set; }

	// Constructor
	public LivesUpdatedEventArgs(int newLives)
	{
		this.newLives = newLives;
	}
}
}