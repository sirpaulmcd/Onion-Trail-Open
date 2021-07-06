// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Interface that corresponds with Melee DWGOs. See AMeleeDWGO.cs.
    /// </summary>
    public interface IMeleeDWGO : IPiercingDWGO
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The offset in front of the player in which the weapon spawns. Offset
        /// ensures swing arc is centered on facing direction rather than starting
        /// in front of the player. Note that the MeleeObject may not appear to
        /// actually spawn exactly at the angle if it is moving quickly.
        /// </summary>
        float SwingOffsetAngle { get; set; }
        #endregion
    }
}
