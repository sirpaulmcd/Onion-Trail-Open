using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instances of DWGOs that use typical colliders and interact with GameObjects 
/// via MonoBehaviour.OnCollisionEnter(). Collider DWGOs are used when the 
/// projectile is expected to bounce off of other GameObjects rather than 
/// passing through. As such, these objects typically utilize the physics 
/// system to calculate their trajectories. For example, after a grenade is 
/// thrown, it is expected to bounce around the environment using the physics 
/// system before exploding.
/// </summary>
public abstract class AColliderDWGO : ADynamicWGO
{
    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called when this collider/rigidbody has begun touching another 
    /// rigidbody/collider.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    private void OnCollisionEnter(Collision collision) 
    {
        ProcessColliderInteraction(collision);
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Processes the collision between colliders. Ignores certain collisions. 
    /// Deals health effects to anything that has an IHealth component.
    /// Applies knockback effects.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    protected virtual void ProcessColliderInteraction(Collision collision)
    {
        if (IgnoreCollision(collision)) { return; }
        prevGameObject = collision.gameObject;
        IHealth health = collision.gameObject.GetComponent<IHealth>();
        if (health != null) { ProcessHealthEffects(collision, health); }
        Knockback(collision);
    }

    /// <summary>
    /// Checks whether the collision should be ignored. Collisions are ignored
    /// if the DWGO hits another DWGO or if it hits the same object more than 
    /// once in a row.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <returns>
    /// True if collision is ignored, false otherwise.
    /// </returns>
    protected virtual bool IgnoreCollision(Collision collision)
    {
        return collision.gameObject.GetComponent<IDynamicWGO>() != null ||
            collision.gameObject == prevGameObject;
    }

    /// <summary>
    /// Computes the damage/heal magnitude and deals it accordingly.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <param name="health">
    /// The health component of the colliding object.
    /// </param>
    protected virtual void ProcessHealthEffects(Collision collision, IHealth health)
    {
        int magnitude = base.ComputeMagnitude();
        if (IsHurt(collision)) { Hurt(collision, health, magnitude); }
        else if (IsHealed(collision)) { Heal(health, magnitude); }
    }

    /// <summary>
    /// Checks whether the colliding object should be hurt by the DWGO.
    /// </summary>
    /// <remarks>
    /// Could check for undead tag and hurt an undead monster if the player 
    /// uses a heal.
    /// </remarks>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <returns>
    /// True if the colliding object should be hurt, false otherwise.
    /// </returns>
    protected virtual bool IsHurt(Collision collision)
    {
        // If action is not a heal, friendly fire is enabled, and hits the attacker's own tag...
        return !IsHeal && collision.gameObject.CompareTag(Attacker.tag) && IsFriendlyFire ||
            // If the action is not a heal and hits a tag different than the attacker's...
            !IsHeal && !collision.gameObject.CompareTag(Attacker.tag);
    }

    /// <summary>
    /// Checks whether the colliding object should be healed by the DWGO.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <returns>
    /// True if the colliding object should be healed, false otherwise.
    /// </returns>
    protected virtual bool IsHealed(Collision collision)
    {
        // If action is a heal and hits the attacker's own tag...
        return IsHeal && collision.gameObject.CompareTag(Attacker.tag);
    }

    /// <summary>
    /// Deals damage to and knocks back the colliding GameObject.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <param name="health">
    /// The IHealth component of the colliding GameObject.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude of the attack/heal.
    /// </param>
    protected virtual void Hurt(Collision collision, IHealth health, int magnitude)
    {
        health.Hurt(magnitude);
    }

    /// <summary>
    /// Knocks the colliding GameObject away. Knockback is handled differently 
    /// for CombativeEntities than for typical Rigidbodies to avoid bugs.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    protected virtual void Knockback(Collision collision)
    {
        if (KnockbackableMagnitude <= 0 || KnockbackMagnitude <= 0 ) { return; }
        Vector3 knockbackDir = GetKnockbackDirection(collision);
        IKnockbackable knockbackable = collision.gameObject.GetComponent<IKnockbackable>();
        if (knockbackable != null) { knockbackable.Knockback(knockbackDir, KnockbackableMagnitude); }
        else { ApplyForceToRigidbody(collision, knockbackDir); }
    }

    /// <summary>
    /// Adds a force to the colliding Rigidbody to induce knockback.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <param name="knockbackDir">
    /// The direction of the knockback.
    /// </param>
    protected virtual void ApplyForceToRigidbody(Collision collision, Vector3 knockbackDir)
    {
        Rigidbody collisionRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (collisionRigidbody == null) { return; }
        collisionRigidbody.AddForce(knockbackDir * KnockbackMagnitude, ForceMode.Impulse);
    }

    /// <summary>
    /// Gets the direction of knockback with respect to the knockback origin.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param>
    /// <returns>
    /// Vector3 indicating knockback direction.
    /// </returns>
    protected virtual Vector3 GetKnockbackDirection(Collision collision)
    {
        Vector3 victimPos = collision.gameObject.transform.position;
        Vector3 knockbackPos = KnockbackOrigin.transform.position;
        return Vector3.Normalize(victimPos - knockbackPos);
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
    #endregion
}
}