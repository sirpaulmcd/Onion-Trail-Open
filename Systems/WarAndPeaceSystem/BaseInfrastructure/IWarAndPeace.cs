using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Interface that corresponds with Weaponized GameObjects. See 
/// AWeaponizedGameObject.cs.
/// </summary>
public interface IWarAndPeace
{
    //=========================================================================
    #region Properties
    //=========================================================================   
    /// <summary>
    /// The player/enemy GameObject who created the attack/heal.
    /// </summary>
    GameObject Attacker { get; set; }
    /// <summary>
    /// The percent chance [0.0, 100.0] that an attack/heal is a critical hit.
    /// </summary>
    /// <invariants>
    /// 0.0 <= criticalHitChance <= 100.0
    /// </invariants>
    float CriticalHitChance { get; set; }
    /// <summary>
    /// The number which multiplies the damage value upon a critical hit.
    /// </summary>
    float CriticalHitMultiplier { get; set; }
    /// <summary>
    /// Whether or not the action can impact the user's own kind.
    /// </summary>
    bool IsFriendlyFire { get; set; }
    /// <summary>
    /// A flag which indicates if this is a heal (true) or an attack (false).
    /// </summary>
    bool IsHeal { get; set; }
    /// <summary>
    /// The magnitude that this object with push generic GameObjects with
    /// rigidbodies.
    /// </summary>
    float KnockbackMagnitude { get; set; }
    /// <summary>
    /// The magnitude that this object with push GameObjects holding the 
    /// IKnockbackable interface.
    /// </summary>
    float KnockbackableMagnitude { get; set; }
    /// <summary>
    /// The origin from which knockback forces will push away.
    /// </summary>
    /// <remarks>
    /// This will typically be the attacker for a melee strike, and the 
    /// projectile for a ranged strike.
    /// </remarks>
    GameObject KnockbackOrigin { get; set; }
    /// <summary>
    /// The upper-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    uint MaximumDamage { get; set; }
    /// <summary>
    /// The lower-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    uint MinimumDamage { get; set; }
    /// <summary>
    /// The title of the attack/heal.
    /// </summary>
    string Title { get; set; }
    #endregion
}
}