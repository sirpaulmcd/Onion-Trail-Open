using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when a player is added.
/// </summary>
public static class PlayerAddedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> PlayerAddedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return PlayerAddedEventHandler;
	}
}

/// <summary>
/// The arguments for a PlayerAddedEvent.
/// </summary>
public class PlayerAddedEventArgs : EventArgs 
{
	/// <summary>
	/// The newly added player.
	/// </summary>
    public Player Player { get; set; }

	// Constructor
	public PlayerAddedEventArgs(Player player)
	{
		this.Player = player;
	}
}
}