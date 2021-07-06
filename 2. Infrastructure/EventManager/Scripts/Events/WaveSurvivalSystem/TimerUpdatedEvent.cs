using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the prep time timer is updated.
    /// </summary>
    public static class TimerUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> TimerUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return TimerUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a TimerUpdatedEvent.
    /// </summary>
    public class TimerUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new value for seconds.
        /// </summary>
        public int Seconds { get; set; }
        /// <summary>
        /// The new value for minutes.
        /// </summary>
        public int Minutes { get; set; }
        /// <summary>
        /// The current wave state.
        /// </summary>
        public WaveSystemGameSession.WaveState WaveState { get; set; }

        // Constructor
        public TimerUpdatedEventArgs(WaveSystemGameSession.WaveState currentState, int seconds, int minutes = 0)
        {
            this.WaveState = currentState;
            this.Seconds = seconds;
            this.Minutes = minutes;
        }
    }
}
