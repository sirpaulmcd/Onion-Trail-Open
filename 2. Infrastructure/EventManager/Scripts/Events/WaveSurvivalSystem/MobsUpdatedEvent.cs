using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the number of mobs is updated.
    /// </summary>
    public static class MobsUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> MobsUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return MobsUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a MobsUpdatedEvent.
    /// </summary>
    public class MobsUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The number of mobs currently alive.
        /// </summary>
        public int AliveCount { get; set; }
        /// <summary>
        /// The total number of mobs in the wave.
        /// </summary>
        public int TotalCount { get; set; }

        // Constructor
        public MobsUpdatedEventArgs(int aliveCount, int totalCount)
        {
            this.AliveCount = aliveCount;
            this.TotalCount = totalCount;
        }
    }
}
