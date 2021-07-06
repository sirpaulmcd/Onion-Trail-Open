// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instances of Trigger DWGO Weapons that are to be destroyed after piercing
    /// through a certain number of targets.
    /// </summary>
    public abstract class APiercingDWGOWeapon : ATriggerDWGOWeapon, IPiercingDWGO
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The number of targets that the DWGO can pierce through before being
        /// destroyed on impact.
        /// </summary>
        [SerializeField] protected int piercingCount = 0;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The number of targets that the DWGO can pierce through before being
        /// destroyed on impact.
        /// </summary>
        public int PiercingCount
        {
            get => piercingCount;
            set => piercingCount = value;
        }
        #endregion
    }
}
