using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when a wave has been complete.
    /// </summary>
    public static class WaveCompleteEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> WaveCompleteEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return WaveCompleteEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a WaveCompleteEvent.
    /// </summary>
    public class WaveCompleteEventArgs : EventArgs
    {
        // No arguments
    }
}
