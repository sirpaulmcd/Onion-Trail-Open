using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when Game Over is reached.
/// </summary>
public static class GameOverEvent
{
	/// <summary>
	/// The GameOverEvent handler.
	/// </summary>
	public static event EventHandler<EventArgs> GameOverEventHandler;
	/// <summary>
	/// The getter for a GameOverEvent handler.
	/// </summary>
	/// <returns>The GameOverEvent handler.</returns>
	public static EventHandler<EventArgs> GetGameOverEventHandler()
	{
		return GameOverEventHandler;
	}
}

/// <summary>
/// The arguments for a GameOverEvent.
/// </summary>
public class GameOverEventArgs : EventArgs 
{
    // No arguments necessary. Simply invoking the event is enough.
}
}