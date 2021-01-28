using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when a player is removed.
/// </summary>
public static class PlayerRemovedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> PlayerRemovedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return PlayerRemovedEventHandler;
	}
}

/// <summary>
/// The arguments for a PlayerRemovedEvent.
/// </summary>
public class PlayerRemovedEventArgs : EventArgs 
{
    // No arguments
}
}