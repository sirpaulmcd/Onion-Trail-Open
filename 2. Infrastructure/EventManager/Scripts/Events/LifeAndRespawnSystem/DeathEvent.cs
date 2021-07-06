using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when a player has died.
    /// </summary>
    public static class DeathEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> DeathEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return DeathEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a DeathEvent.
    /// </summary>
    public class DeathEventArgs : EventArgs
    {
        // No arguments
    }
}
