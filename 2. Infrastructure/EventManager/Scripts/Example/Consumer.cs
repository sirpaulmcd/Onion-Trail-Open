using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Example listener using the EventManager. To listen to an event:
/// - Create a method that will be called when the event is invoked
/// - In OnEnable(): Add self as a listener to the event inputting the event
///   name and the method to be called.
/// - In OnDisable(): Remove self as a listener to the event inputting the 
///   event name and the method to be called.
/// </summary>
public class Consumer : MonoBehaviour
{
    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    void OnEnable()
    {
        EventManager.Instance.AddListener(EventName.SpacePressedEvent, HandleSpacePressedEvent);
    }

    void OnDisable()
    {
        EventManager.Instance.RemoveListener(EventName.SpacePressedEvent, HandleSpacePressedEvent);
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
    private void HandleSpacePressedEvent(object invoker, System.EventArgs e)
    {
        SpacePressedEventArgs args = (SpacePressedEventArgs) e;
        Debug.Log("Pressed space " + args.spaceCount + " times.");
    }
    #endregion
}
}