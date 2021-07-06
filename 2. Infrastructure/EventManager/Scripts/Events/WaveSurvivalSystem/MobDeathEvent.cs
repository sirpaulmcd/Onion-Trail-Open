using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when a mob has died.
    /// </summary>
    public static class MobDeathEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> MobDeathEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return MobDeathEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a MobDeathEvent.
    /// </summary>
    public class MobDeathEventArgs : EventArgs
    {
        // No arguments
    }
}
