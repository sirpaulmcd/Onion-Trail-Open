using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EGS
{
/// <summary>
/// Manages connections between event listeners and event invokers. Stores
/// event invokers and listeners in separate dictionaries. Pairs listeners to 
/// their corresponding invokers through the common EventName key.
/// </summary>
public static class EventManager
{
	//=========================================================================
	#region Instance variables
	//=========================================================================
    /// <summary>
    /// Dictionary of EventInvoker objects using EventName as the key.
    /// </summary>
    /// <returns></returns>
	private static Dictionary<EventName, List<EventInvoker>> invokers = 
        new Dictionary<EventName, List<EventInvoker>>();
    /// <summary>
    /// Dictionary of listener EventHandler objects using EventName as the key.
    /// </summary>
    /// <returns></returns>
	private static Dictionary<EventName, List<EventHandler<EventArgs>>> listeners = 
        new Dictionary<EventName, List<EventHandler<EventArgs>>>();
	#endregion

	//=========================================================================
	#region Public methods
	//=========================================================================
    /// <summary>
    /// Initializes the EventManager. To be called once on initial load.
    /// </summary>
	public static void Init()
    {
		// Create empty lists for all dictionary keys (i.e. events)
		foreach (EventName name in Enum.GetValues(typeof(EventName)))
        {
			if (!invokers.ContainsKey(name))
            {
				invokers.Add(name, new List<EventInvoker>());
				listeners.Add(name, new List<EventHandler<EventArgs>>());
			}
            else
            {
				invokers[name].Clear();
				listeners[name].Clear();
			}
		}
	}
		
	/// <summary>
	/// Adds the given invoker to the invoker dictionary. Pairs corresponding
    /// listeners.
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="invoker">The invoker of the event.</param>
	public static void AddInvoker(EventName eventName, EventInvoker invoker)
    {
		// Pair listeners to new invoker
		foreach (EventHandler<EventArgs> listener in listeners[eventName])
        {
			invoker.AddListener(eventName, listener);
		}
        // Add new invoker to dictionary
		invokers[eventName].Add(invoker);
	}

	/// <summary>
	/// Adds the given listener for the listener dictionary. Pairs 
    /// corresponding invokers.   
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="listener">The listener to the event.</param>
	public static void AddListener(EventName eventName, EventHandler<EventArgs> listener)
    {
		// Pair listener to relevant invokers
		foreach (EventInvoker invoker in invokers[eventName])
        {
			invoker.AddListener(eventName, listener);
		}
        // Add new invoker to dictionary
		listeners[eventName].Add(listener);
	}

	/// <summary>
	/// Removes the input invoker from the dictionary.
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="invoker">The invoker to be removed.</param>
	public static void RemoveInvoker(EventName eventName, EventInvoker invoker)
    {
		invokers[eventName].Remove(invoker);
	}

	/// <summary>
	/// Removes the given listener from the listener dictionary. Unpairs from
    /// corresponding invokers.    
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="listener">The listener to the event.</param>
    public static void RemoveListener(EventName eventName, EventHandler<EventArgs> listener)
    {
		foreach (EventInvoker invoker in invokers[eventName])
        {
			invoker.RemoveListener(eventName, listener);
		}
        listeners[eventName].Remove(listener);
    }
	#endregion
}
}