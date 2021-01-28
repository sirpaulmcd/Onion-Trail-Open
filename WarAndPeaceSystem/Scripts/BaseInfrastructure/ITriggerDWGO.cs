using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Interface that corresponds with Trigger DWGOs. See ATriggerDWGO.cs.
/// </summary>
public interface ITriggerDWGO : IDynamicWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// Whether or not the TriggerDWGO should always knock GameObjects in the
    /// movement direction. Since TriggerDWGOs can travel through other 
    /// GameObjects, fast moving projectiles can pass through the transform of 
    /// the victim before a collision is properly registered. As such, when the
    /// collision occurs, the resulting knockback seemingly pushes the player 
    /// towards the projectile rather than away from it. This boolean prevents 
    /// these cases.
    /// </summary>
    bool KnockTowardsMovementDirection { get; set; }
    #endregion
}
}