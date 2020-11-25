// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instances of Dynamic WGO Weapons that spawn Trigger DWGOs.
/// </summary>
public abstract class ATriggerDWGOWeapon : ADynamicWGOWeapon, ITriggerDWGO
{
    //=========================================================================
    #region Instance variables
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
    [SerializeField] protected bool knockTowardsMovementDirection = false;
    #endregion

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
    public bool KnockTowardsMovementDirection 
    { 
        get => knockTowardsMovementDirection;
        set => knockTowardsMovementDirection = value;
    }
    #endregion
}
}