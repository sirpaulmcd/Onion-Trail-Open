using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the scene objective has been complete.
    /// </summary>
    public static class ObjectiveCompleteEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> ObjectiveCompleteEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return ObjectiveCompleteEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a ObjectiveCompleteEvent.
    /// </summary>
    public class ObjectiveCompleteEventArgs : EventArgs
    {
        // No arguments
    }
}
