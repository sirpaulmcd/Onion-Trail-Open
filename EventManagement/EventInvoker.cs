using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EGS
{
/// <summary>
/// When inherited, this class allows an object to invoke events. It contains
/// a dictionary that stores event handlers. This way, it can invoke events
/// and add listeners to events that are stored in the dictionary. 
/// </summary>
public class EventInvoker : MonoBehaviour
{
	//=========================================================================
	#region Instance variables
	//=========================================================================
	/// <summary>
	/// Dictionary of event handlers that the invoker has been assigned. An
	/// invoker can invoke any handler that is stored within this dictionary.
	/// </summary>
	protected Dictionary<EventName, EventHandler<EventArgs>> invokableEvents = 
		new Dictionary<EventName, EventHandler<EventArgs>>();
	#endregion

	//=========================================================================
	#region Public methods
	//=========================================================================
	/// <summary>
	/// Adds a listener to the named event.
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="listener">The listener of the event.</param>
	public void AddListener(EventName eventName, EventHandler<EventArgs> listener)
    {
		// Check if invoker actually invokes input event
		if (invokableEvents.ContainsKey(eventName))
        {
			// Add listener to event
			invokableEvents[eventName] += listener;
		}
	}

	/// <summary>
	/// Removes a listener from the named event.
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <param name="listener">The listener of the event.</param>
	public void RemoveListener(EventName eventName, EventHandler<EventArgs> listener)
	{
		// Check if invoker actually invokes input event
		if (invokableEvents.ContainsKey(eventName))
        {
			// Remove listener from event
			invokableEvents[eventName] -= listener;
		}
	}
	#endregion
}
}