using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when squad fuel is updated.
    /// </summary>
    public static class FuelUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> FuelUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return FuelUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a FuelUpdatedEvent.
    /// </summary>
    public class FuelUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new value for Fuel.
        /// </summary>
        public int newFuel { get; set; }

        // Constructor
        public FuelUpdatedEventArgs(int newFuel)
        {
            this.newFuel = newFuel;
        }
    }
}
