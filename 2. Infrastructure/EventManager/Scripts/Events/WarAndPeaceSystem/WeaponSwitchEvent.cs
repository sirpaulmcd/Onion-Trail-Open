using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when a player switches weapons.
    /// </summary>
    public static class WeaponSwitchEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> WeaponSwitchEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return WeaponSwitchEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a WeaponSwitchEvent.
    /// </summary>
    public class WeaponSwitchEventArgs : EventArgs
    {

        // Constructor
        public WeaponSwitchEventArgs()
        {

        }
    }
}
