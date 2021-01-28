using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Event to be called when squad food is updated.
/// </summary>
public static class FoodUpdatedEvent
{
	// Event handler delegate
	public static event EventHandler<EventArgs> FoodUpdatedEventHandler;
	// Getter
	public static EventHandler<EventArgs> GetEventHandler()
	{
		return FoodUpdatedEventHandler;
	}
}

/// <summary>
/// The arguments for a FoodUpdatedEvent.
/// </summary>
public class FoodUpdatedEventArgs : EventArgs 
{
	/// <summary>
	/// The new value for food.
	/// </summary>
    public int newFood { get; set; }

	// Constructor
	public FoodUpdatedEventArgs(int newFood)
	{
		this.newFood = newFood;
	}
}
}