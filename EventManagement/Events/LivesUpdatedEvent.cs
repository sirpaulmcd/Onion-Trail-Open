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
	/// <summary>
	/// The LivesUpdatedEvent handler.
	/// </summary>
	public static event EventHandler<EventArgs> LivesUpdatedEventHandler;
	/// <summary>
	/// The getter for a LivesUpdatedEvent handler.
	/// </summary>
	/// <returns>The LivesUpdatedEvent handler.</returns>
	public static EventHandler<EventArgs> GetLivesUpdatedEventHandler()
	{
		return LivesUpdatedEventHandler;
	}
}

/// <summary>
/// The arguments for a LivesUpdatedEvent.
/// </summary>
public class LivesUpdatedEventArgs : EventArgs 
{
    public int newLives { get; set; }
	/// <summary>
	/// Constructor for LivesUpdatedEventArgs.
	/// </summary>
	/// <param name="newLives">
	/// The new value for lives.
	/// </param>
	public LivesUpdatedEventArgs(int newLives)
	{
		this.newLives = newLives;
	}
}
}