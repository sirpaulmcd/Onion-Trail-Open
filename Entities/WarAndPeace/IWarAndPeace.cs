using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Interface for the Attack/Healing system known as the "War And Peace" system.
/// </summary>
public interface IWarAndPeace
{
    //==========================================================================
    // Properties
    //=========================================================================   

    /// <summary>
    /// The player/enemy GameObject who created the attack/heal.
    /// </summary>
    GameObject attacker { get; set; }

    /// <summary>
    /// The number which multiplies the damage value upon a critical hit.
    /// </summary>
    float criticalHitMultiplier { get; set; }

    /// <summary>
    /// The percent chance [0.0, 100.0] that an attack/heal is a critical hit.
    /// </summary>
    /// <invariants>
    /// 0.0 <= criticalHitChance <= 100.0
    /// </invariants>
    float criticalHitChance { get; set; }

    /// <summary>
    /// A flag which indicates if this is a heal (true) or an attack (false).
    /// </summary>
    bool heal { get; set; }

    /// <summary>
    /// The distance that the attack/heal will push the affected GameObject.  
    /// </summary>
    float knockback { get; set; }

    /// <summary>
    /// A GameObject which the knockback pushes away from.
    /// </summary>
    /// <remarks>
    /// This will typically be the attacker for a melee strike, and the 
    /// projectile for a ranged strike.
    /// </remarks>
    GameObject knockbackOrigin { get; set; }

    /// <summary>
    /// The lower-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    uint minimumDamage { get; set; }

    /// <summary>
    /// The upper-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    uint maximumDamage { get; set; }

    /// <summary>
    /// The name of the attack/heal.
    /// </summary>
    string name { get; set; }

    //==========================================================================
    // Public Methods
    //==========================================================================

    /// <summary>
    /// Ends the attack and destroys the parent GameObject.
    /// </summary>
    void DestroyAttack();
}
}