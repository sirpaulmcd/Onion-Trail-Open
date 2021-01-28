using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when squad med is updated.
/// </summary>
public static class MedUpdatedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> MedUpdatedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return MedUpdatedEventHandler;
	}
}

/// <summary>
/// The arguments for a MedUpdatedEvent.
/// </summary>
public class MedUpdatedEventArgs : EventArgs 
{
	/// <summary>
	/// The new value for med.
	/// </summary>
    public int newMed { get; set; }
	
	// Constructor
	public MedUpdatedEventArgs(int newMed)
	{
		this.newMed = newMed;
	}
}
}