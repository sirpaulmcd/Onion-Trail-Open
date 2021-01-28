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
	// Event handler delegate
	public static event EventHandler<EventArgs> GameOverEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return GameOverEventHandler;
	}
}

/// <summary>
/// The arguments for a GameOverEvent.
/// </summary>
public class GameOverEventArgs : EventArgs 
{
    // No arguments
}
}