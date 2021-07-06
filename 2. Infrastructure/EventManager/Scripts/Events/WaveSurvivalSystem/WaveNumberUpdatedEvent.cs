using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the wave number is updated.
    /// </summary>
    public static class WaveNumberUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> WaveNumberUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return WaveNumberUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a WaveNumberUpdatedEvent.
    /// </summary>
    public class WaveNumberUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new wave number.
        /// </summary>
        public int NewNumber { get; set; }

        // Constructor
        public WaveNumberUpdatedEventArgs(int newNumber)
        {
            this.NewNumber = newNumber;
        }
    }
}
