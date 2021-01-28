using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Example invoker using the EventManager. To invoke a method, simply call
/// the Invoke() method of the EventManager and input the EventName, EventArgs, 
/// and (optionally) the invoker as seen below.
/// </summary>
public class Producer : MonoBehaviour
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
    /// Invokes a SpacePressedEvent
    /// </summary>
    private void InvokeSpacePressedEvent()
    {
        Debug.Log("Producer.InvokeSpacePressedEvent: Space has been pressed.");
        EventManager.Instance.Invoke(EventName.SpacePressedEvent, new SpacePressedEventArgs(_spaceCount), this);
    }
    #endregion
}
}
