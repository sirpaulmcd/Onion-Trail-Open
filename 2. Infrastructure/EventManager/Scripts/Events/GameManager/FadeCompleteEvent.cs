using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when main menu fade in/out animation is complete.
    /// </summary>
    public static class FadeCompleteEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> FadeCompleteEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return FadeCompleteEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a FadeCompleteEvent.
    /// </summary>
    public class FadeCompleteEventArgs : EventArgs
    {
        /// <summary>
        /// Whether or not the fade out animation was played. If false, the fade in
        /// animation was played.
        /// </summary>
        public bool FadeOut { get; set; }

        // Constructor
        public FadeCompleteEventArgs(bool fadeOut)
        {
            this.FadeOut = fadeOut;
        }
    }
}
