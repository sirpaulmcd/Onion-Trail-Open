using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Interface that corresponds with Piercing DWGOs. See APiercingDWGO.cs.
    /// </summary>
    public interface IPiercingDWGO : ITriggerDWGO
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The number of entities that the DWGO can pierce through before being
        /// destroyed on impact.
        /// </summary>
        int PiercingCount { get; set; }
        #endregion
    }
}
