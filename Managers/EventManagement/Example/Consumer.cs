using System; // Necessary for EventArgs class
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This is an example of how to implement an event listener. Steps include:
/// - Add "using System;" to imports
/// - Within Start(), register as listener for desired events
/// - Within OnDestroy(), unregister as listener from desired events
/// - Create a handler method to be called when the event is invoked
///    - Store in "Event handlers" region
/// </summary>
public class Consumer : MonoBehaviour
{
    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    {
        // Register as listener for the SpacePressedEvent and connect appropriate handler method
        EventManager.AddListener(EventName.SpacePressedEvent, HandleSpacePressedEvent);
    }

    /// <summary>
    /// Called when a Scene or game ends.
    /// </summary>
    void OnDestroy()
    {
        // You should unregister listeners as they are destroyed to help
        // conserve memory.
        EventManager.RemoveListener(EventName.SpacePressedEvent, HandleSpacePressedEvent);
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Called when a SpacePressedEvent is invoked. It has the same name as the
    /// event with the addition of "Handle" at the beginning.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleSpacePressedEvent(object invoker, EventArgs e)
    {
        // Typecast input EventArgs object to appropriate SpacePressedEventArgs object
        SpacePressedEventArgs args = (SpacePressedEventArgs) e;
        // Access properties
        Debug.Log("Pressed space " + args.spaceCount + " times.");
    }
    #endregion
}
}