using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when an objects _health is updated.
/// </summary>
public static class HealthUpdatedEvent
{
	/// <summary>
	/// The HealthUpdatedEvent handler.
	/// </summary>
	public static event EventHandler<EventArgs> HealthUpdatedEventHandler;
	/// <summary>
	/// The getter for a HealthUpdatedEvent handler.
	/// </summary>
	/// <returns>The HealthUpdatedEvent handler.</returns>
	public static EventHandler<EventArgs> GetHealthUpdatedEventHandler()
	{
		return HealthUpdatedEventHandler;
	}
}

/// <summary>
/// The arguments for a HealthUpdatedEvent.
/// </summary>
public class HealthUpdatedEventArgs : EventArgs 
{
	public int maxHealth { get; set; }
    public int newHealth { get; set; }
	/// <summary>
	/// Constructor for HealthUpdatedEventArgs
	/// </summary>
	/// <param name="maxHealth">
	/// The maximum _health of the _health source.
	/// </param>
	/// <param name="newHealth">
	/// The minimum _health of the _health source.
	/// </param>
	public HealthUpdatedEventArgs(int maxHealth, int newHealth)
	{
		this.maxHealth = maxHealth;
		this.newHealth = newHealth;
	}
}
}