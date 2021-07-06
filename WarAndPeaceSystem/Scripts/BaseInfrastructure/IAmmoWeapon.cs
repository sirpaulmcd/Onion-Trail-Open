using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Interface that corresponds to a weapon with an ammo supply.
    /// </summary>
    public interface IAmmoWeapon
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// Boolean indicating whether weapon is out of ammo.
        /// </summary>
        bool OutOfAmmo { get; }
        /// <summary>
        /// The maximum capacity of an ammo magazine.
        /// </summary>
        uint MagCapacity { get; set; }
        /// <summary>
        /// The amount of ammo in your reserve.
        /// </summary>
        uint TotalAmmo { get; set; }
        /// <summary>
        /// The amount of times you can fire before a reload.
        /// 0 means bottomless mag
        /// </summary>
        uint MaxMagCapacity { get; set; }
        /// <summary>
        /// The maximum amount of ammo you can carry.
        /// 0 means infinite ammo
        /// </summary>
        uint MaxTotalAmmo { get; set; }
        /// <summary>
        /// The amount of time reloading takes in seconds.
        /// </summary>
        float ReloadSeconds { get; set; }
        #endregion
    }
}
