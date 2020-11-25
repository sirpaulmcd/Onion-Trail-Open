// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Interface that corresponds with Bullet DWGOs. See ABulletDWGO.cs.
/// </summary>
public interface IBulletDWGO : IPiercingDWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The LayerMask that the raycast can detect. To set the LayerMask to 
    /// detect everything, initialize as -1. In order for DWGO to work, the 
    /// 'Ignore Raycast' layermask must be deselected through the inspector.
    /// </summary>
    LayerMask LayerMask { get; set; }
    /// <summary>
    /// The maximum distance that the DWGO raycast will travel.
    /// </summary>
    float MaxRaycastDistance { get; set; }
    /// <summary>
    /// The seconds between raycasts used to collect reflection information.
    /// </summary>
    float RaycastRefreshSeconds { get; set; }
    /// <summary>
    /// The number of times the DWGO can ricochet off of objects in the 
    /// environment.
    /// </summary>
    int RicochetCount { get; set; }
    #endregion
}
}