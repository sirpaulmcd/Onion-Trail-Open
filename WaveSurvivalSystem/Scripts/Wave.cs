using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Represents a wave of enemies. It contains a variety of mobs.
    /// </summary>
    [Serializable]
    public class Wave
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private Mob[] _mobs;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The types of mobs in a specific wave.
        /// </summary>
        public Mob[] Mobs
        {
            get => _mobs;
            private set => _mobs = value;
        }
        #endregion
    }
}
