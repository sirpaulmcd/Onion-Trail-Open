using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instances of Trigger DWGOs that count the number of times they can pierce 
/// victims before they are destroyed. For example, developers may only want a 
/// bullet to pierce through 3 enemies before self destructing.
/// </summary>
public abstract class APiercingDWGO : ATriggerDWGO, IPiercingDWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("IPiercingDWGO properties")] 
    /// <summary>
    /// The number of entities that the DWGO can pierce through before being 
    /// destroyed on impact.
    /// </summary>
    public int PiercingCount { get; set; }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Initializes the DWGO variables, sets the appropraite rotation, launches 
    /// the GameObject in its "forward" direction, starts the self destruct 
    /// countdown.
    /// </summary>
    /// <param name="weapon">
    /// The weapon script that fired this GameObject.
    /// </param>
    /// <param name="attacker">
    /// The GameObject initiating the attack/heal.
    /// </param>
    /// <param name="direction">
    /// The direction that the attack is to be made.
    /// </param>
    public void Init(IPiercingDWGO weapon, GameObject attacker, Vector3 direction)
    {
        base.Init(weapon, attacker, direction);
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    protected void InitVars(IPiercingDWGO weapon, GameObject attacker)
    {
        base.InitVars(weapon, attacker);
        // Initialize IPiercingDWGO variables
        this.PiercingCount = weapon.PiercingCount;
    }

    /// <summary>
    /// Processes the collision between colliders. Ignores certain collisions. 
    /// Deals health effects to anything that has an IHealth component.
    /// Applies knockback effects. Destroys the object if it runs down the
    /// PiercingCount.
    /// </summary>
    /// <param name="collider">
    /// The collider that is interacting with the WeaponizedGameObject trigger.
    /// </param>
    protected override void ProcessColliderInteraction(Collider collider)
    {
        base.ProcessColliderInteraction(collider);
        if (IsDestroyed()) { Destroy(this.gameObject); }
    }

    /// <summary>
    /// Deals damage to and knocks back the colliding GameObject. Decrements 
    /// piercing count.
    /// </summary>
    /// <param name="collider">
    /// The collider that is interacting with the WeaponizedGameObject trigger.
    /// </param>
    /// <param name="health">
    /// The IHealth component of the collider.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude of the attack/heal.
    /// </param>
    protected override void Hurt(Collider collider, IHealth health, int magnitude)
    {
        base.Hurt(collider, health, magnitude);
        this.PiercingCount -= 1;
    }

    /// <summary>
    /// Heals the input Health component by the input magnitude. Decrements
    /// piercing count.
    /// </summary>
    /// <param name="health">
    /// The health component of to be healed.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude of the heal.
    /// </param>
    protected override void Heal(IHealth health, int magnitude)
    {
        base.Heal(health, magnitude);
        this.PiercingCount -= 1;
    }

    /// <summary>
    /// Whether or not the object should be destroyed after interacting with 
    /// the collider. The object is destroyed if it has ran down its 
    /// PiercingCount.
    /// </summary>
    /// <returns>
    /// True if piercing count count is below zero. False otherwise.
    /// </returns>
    protected virtual bool IsDestroyed()
    {
        return this.PiercingCount < 0;
    }
    #endregion
}
}