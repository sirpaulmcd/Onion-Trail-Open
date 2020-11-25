using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when something with _health has died.
/// </summary>
public static class DeathEvent
{
	/// <summary>
	/// The DeathEvent handler.
	/// </summary>
	public static event EventHandler<EventArgs> DeathEventHandler;
	/// <summary>
	/// The getter for a DeathEvent handler.
	/// </summary>
	/// <returns>The DeathEvent handler.</returns>
	public static EventHandler<EventArgs> GetDeathEventHandler()
	{
		return DeathEventHandler;
	}
}

/// <summary>
/// The arguments for a DeathEvent.
/// </summary>
public class DeathEventArgs : EventArgs 
{
	// No arguments necessary
}
}