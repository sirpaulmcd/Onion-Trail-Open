using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when an objects health is updated.
    /// </summary>
    public static class HealthUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> HealthUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return HealthUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a HealthUpdatedEvent.
    /// </summary>
    public class HealthUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The maximum health of the health source.
        /// </summary>
        public int maxHealth { get; set; }
        /// <summary>
        /// The new health of the health source.
        /// </summary>
        public int newHealth { get; set; }

        // Constructor
        public HealthUpdatedEventArgs(int maxHealth, int newHealth)
        {
            this.maxHealth = maxHealth;
            this.newHealth = newHealth;
        }
    }
}
