using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when a players ammo is updated.
    /// </summary>
    public static class WeaponAmmoEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> WeaponAmmoEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return WeaponAmmoEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a WeaponAmmoEvent.
    /// </summary>
    public class WeaponAmmoEventArgs : EventArgs
    {
        /// <summary>
        /// The ammo the player can fire before reloading.
        /// </summary>
        public uint magAmmo { get; set; }
        /// <summary>
        /// The ammo the player has in reserve.
        /// </summary>
        public uint remainingAmmo { get; set; }

        // Constructor
        public WeaponAmmoEventArgs(uint magAmmo, uint remainingAmmo)
        {
            this.magAmmo = magAmmo;
            this.remainingAmmo = remainingAmmo;
        }
    }
}
