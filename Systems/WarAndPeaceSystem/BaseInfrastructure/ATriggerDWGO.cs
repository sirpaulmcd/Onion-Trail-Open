using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instances of DWGOs that use trigger colliders and interact with GameObjects 
/// via MonoBehaviour.OnTriggerEnter(). Trigger DWGOs are used when the 
/// projectile is expected to pass through other GameObjects. As such, these 
/// objects typically neglect the physics system and calculate their own 
/// trajectories. For example, a bullet may be expected to pass through 
/// multiple enemies without bouncing off and changing direction. The same 
/// could be said for a melee weapon. In some cases, you may want a Trigger 
/// DWGO that travels through some objects but reflects off of others. For 
/// instance, a bullet that travels through enemies but ricochets off of walls. 
/// In these situations, ricocheting can be simulated using methods such as 
/// raycasting (see bullet implementation). Additionally, fast moving Trigger 
/// DWGOs can pass the transform of their targets before a collision is 
/// registered. As a result, the induced knockback seemingly knocks objects 
/// towards the projectile rather than away. To account for these cases, 
/// Trigger DWGOs have the option to always induce knockback in the direction 
/// of movement. Similarly, Trigger DWGOs that move so fast that a collision is 
/// not even registered should be changed into raycasting weapons rather than 
/// using physical projectiles.
/// </summary>
public abstract class ATriggerDWGO : ADynamicWGO, ITriggerDWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("ITriggerDWGO properties")] 
    /// <summary>
    /// Whether or not the TriggerDWGO should always knock GameObjects in the
    /// movement direction. Since TriggerDWGOs can travel through other 
    /// GameObjects, fast moving projectiles can pass through the transform of 
    /// the victim before a collision is properly registered. As such, when the
    /// collision occurs, the resulting knockback seemingly pushes the player 
    /// towards the projectile rather than away from it. This boolean prevents 
    /// these cases.
    /// </summary>
    public bool KnockTowardsMovementDirection { get; set; }
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called when a collider enters this trigger.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    private void OnTriggerEnter(Collider collider) 
    {
        ProcessColliderInteraction(collider);
    }
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
    public void Init(ITriggerDWGO weapon, GameObject attacker, Vector3 direction)
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
    /// <param name="weapon">
    /// The weapon script that fired this GameObject.
    /// </param>
    /// <param name="attacker">
    /// The GameObject initiating the attack/heal.
    /// </param>
    protected void InitVars(ITriggerDWGO weapon, GameObject attacker)
    {
        base.InitVars(weapon, attacker);
        // Initialize ITriggerDWGO variables
        this.KnockTowardsMovementDirection = weapon.KnockTowardsMovementDirection;
    }

    /// <summary>
    /// Processes the collision between colliders. Ignores certain collisions. 
    /// Deals health effects to anything that has an IHealth component.
    /// Applies knockback effects.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    protected virtual void ProcessColliderInteraction(Collider collider)
    {
        if (IgnoreCollision(collider)) { return; }
        prevGameObject = collider.gameObject;
        IHealth health = collider.gameObject.GetComponent<IHealth>();
        if (health != null) { ProcessHealthEffects(collider, health); }
        Knockback(collider);
    }

    /// <summary>
    /// Checks whether the collision should be ignored. Collisions are ignored
    /// if the DWGO hits another DWGO or if it hits the same object more than 
    /// once in a row.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <returns>
    /// True if collision is ignored, false otherwise.
    /// </returns>
    protected virtual bool IgnoreCollision(Collider collider)
    {
        return collider.gameObject.GetComponent<ADynamicWGO>() != null ||
            collider.gameObject == prevGameObject;
    }

    /// <summary>
    /// Computes the damage/heal magnitude and deals it accordingly.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <param name="health">
    /// The health component of the collider.
    /// </param>
    protected virtual void ProcessHealthEffects(Collider collider, IHealth health)
    {
        int magnitude = base.ComputeMagnitude();
        if (IsHurt(collider)) { Hurt(collider, health, magnitude); }
        else if (IsHealed(collider)) { Heal(health, magnitude); }
    }

    /// <summary>
    /// Checks whether the collider should be hurt by the WeaponizedGameObject.
    /// </summary>
    /// <remarks>
    /// Could check for undead tag and hurt an undead monster if the player 
    /// uses a heal.
    /// </remarks>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <returns>
    /// True if the collider should be hurt, false otherwise.
    /// </returns>
    protected virtual bool IsHurt(Collider collider)
    {
        // If action is not a heal, friendly fire is enabled, and hits the attacker's own tag...
        return !IsHeal && collider.gameObject.CompareTag(Attacker.tag) && IsFriendlyFire ||
            // If the action is not a heal and hits a tag different than the attacker's...
            !IsHeal && !collider.gameObject.CompareTag(Attacker.tag);
    }

    /// <summary>
    /// Checks whether the colliding object should be healed by the DWGO.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <returns>
    /// True if the collider should be healed, false otherwise.
    /// </returns>
    protected virtual bool IsHealed(Collider collider)
    {
        // If action is a heal and hits the attacker's own tag...
        return IsHeal && collider.gameObject.CompareTag(Attacker.tag);
    }

    /// <summary>
    /// Deals damage to and knocks back the colliding GameObject.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <param name="health">
    /// The IHealth component of the collider.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude of the attack/heal.
    /// </param>
    protected virtual void Hurt(Collider collider, IHealth health, int magnitude)
    {
        health.Hurt(magnitude);
    }

    /// <summary>
    /// Heals the input Health component by the input magnitude.
    /// </summary>
    /// <param name="health">
    /// The health component of to be healed.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude of the heal.
    /// </param>
    protected virtual void Heal(IHealth health, int magnitude)
    {
        health.Heal(magnitude);
    }

    /// <summary>
    /// Knocks the colliding GameObject away. Knockback is handled differently 
    /// for CombativeEntities than for typical Rigidbodies to avoid bugs.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    protected virtual void Knockback(Collider collider)
    {
        if (KnockbackableMagnitude <= 0 || KnockbackMagnitude <= 0 ) { return; }
        Vector3 knockbackDir = GetKnockbackDirection(collider);
        IKnockbackable knockbackable = collider.gameObject.GetComponent<IKnockbackable>();
        if (knockbackable != null) { knockbackable.Knockback(knockbackDir, KnockbackableMagnitude); }
        else { ApplyForceToRigidbody(collider, knockbackDir); }
    }

    /// <summary>
    /// Adds a force to the colliding Rigidbody to induce knockback.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <param name="knockbackDir">
    /// The direction of the knockback.
    /// </param>
    protected virtual void ApplyForceToRigidbody(Collider collider, Vector3 knockbackDir)
    {
        Rigidbody colliderRigidbody = collider.gameObject.GetComponent<Rigidbody>();
        if (colliderRigidbody == null) { return; }
        colliderRigidbody.AddForce(knockbackDir * KnockbackMagnitude, ForceMode.Impulse);
    }

    /// <summary>
    /// Gets the direction of knockback with respect to the knockback origin.
    /// </summary>
    /// <param name="collider">
    /// The collider entering the trigger.
    /// </param> 
    /// <returns>
    /// Vector3 indicating knockback direction.
    /// </returns>
    protected virtual Vector3 GetKnockbackDirection(Collider collider)
    {
        if (KnockTowardsMovementDirection) { return transform.forward.normalized; }
        Vector3 victimPos = collider.gameObject.transform.position;
        Vector3 knockbackPos = KnockbackOrigin.transform.position;
        Vector3 direction = victimPos - knockbackPos;
        return new Vector3(direction.x, 0, direction.z).normalized;
    }
    #endregion
}
}