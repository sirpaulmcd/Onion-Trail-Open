using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the wave state is updated.
    /// </summary>
    public static class WaveStateUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> WaveStateUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return WaveStateUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a WaveStateUpdatedEvent.
    /// </summary>
    public class WaveStateUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new wave state.
        /// </summary>
        public WaveSystemGameSession.WaveState NewState { get; set; }
        /// <summary>
        /// The previous wave state.
        /// </summary>
        public WaveSystemGameSession.WaveState PrevState { get; set; }
        // Constructor
        public WaveStateUpdatedEventArgs(WaveSystemGameSession.WaveState newState, WaveSystemGameSession.WaveState prevState)
        {
            this.NewState = newState;
            this.PrevState = prevState;
        }
    }
}
