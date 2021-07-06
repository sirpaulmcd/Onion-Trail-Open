using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when all players vote to skip wave prep time.
    /// </summary>
    public static class SkipPrepTimeEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> SkipPrepTimeEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return SkipPrepTimeEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a SkipPrepTimeEvent.
    /// </summary>
    public class SkipPrepTimeEventArgs : EventArgs
    {
        // No arguments
    }
}
