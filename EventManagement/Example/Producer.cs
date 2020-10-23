using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This is an example of how to implement an event invoker. Steps include:
/// - Add "using System;" to imports
/// - Inherit from the EventInvoker class
/// - Within Start():
///    - Add desired events to invokableEvents dictionary
///    - Register self as invoker for desired events
/// - Create invoker methods for desired events with "Invoke[event-name]" naming convention
///    - Store in "Event invokers" region
/// - Call the invoker method where applicable
/// </summary>
public class Producer : EventInvoker // Necessary for accessing "invokableEvents" variable.
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The number of times the space bar has been pressed since the scene
    /// was started.
    /// </summary>
    private int _spaceCount = 0;
    #endregion

	//=========================================================================
	#region MonoBehaviour
	//=========================================================================
	private void Start() 
	{
        // Register as invoker for SpacePressedEvent
		invokableEvents.Add(EventName.SpacePressedEvent, SpacePressedEvent.GetSpacePressedEventHandler());
		EventManager.AddInvoker(EventName.SpacePressedEvent, this);
	}

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _spaceCount++;
            // Invoke the event where applicable
            InvokeSpacePressedEvent();
        }
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    /// <summary>
    /// Invokes a SpacePressedEvent. It has the same name as the event with the
    /// addition of "Invoke" at the beginning. It is called within the script 
    /// to invoke the desired event.
    /// </summary>
    private void InvokeSpacePressedEvent()
    {
        // Create instance of event specific arguments. Check 
        // SpacePressedEvent class for argument details.
        SpacePressedEventArgs args = new SpacePressedEventArgs(_spaceCount);
        // Invoke the event. The question mark is used to check whether the
        // list of associated event handlers is not null before invoking it.
        // (i.e. checks that the event has listeners before invoking it)
        // Without this check, you will get a NullReferenceException because
        // the event will try to call delegates that do not exist. 
        invokableEvents[EventName.SpacePressedEvent]?.Invoke(this, args);
    }
    #endregion
}
}
