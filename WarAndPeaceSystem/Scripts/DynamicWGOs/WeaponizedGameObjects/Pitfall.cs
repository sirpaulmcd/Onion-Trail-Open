using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This class represents a pitfall trap. If a player falls into the trap, they
/// are damaged and teleported back to their latest respawn location. If an 
/// enemy or object falls into the pit, it is destroyed. Ensure that collider 
/// objects carrying this script do not touch parts of the environment as it 
/// will destroy anything it touches.
/// </summary>
public class Pitfall : AWeaponizedGameObject
{
    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called when this collider/rigidbody has begun touching another 
    /// rigidbody/collider.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    void OnCollisionEnter(Collision collision)
    {
        DestroyOrRespawn(collision);
    }
    #endregion

    //=========================================================================
    #region Helper methods
    //=========================================================================
    /// <summary>
    /// If a player falls into the pit, they are handled accordingly. If any 
    /// other GameObject falls into the pit, it is destroyed.
    /// </summary>
    /// <param name="other">
    /// Collision associated with the gameObject.
    /// </param>
    private void DestroyOrRespawn(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HandlePlayer(other);
        }
        else
        {
            Destroy(other.gameObject);
        }
    }

    /// <summary>
    /// Incapacitates and hurts player. If player still has _health, respawns
    /// them back on stable ground.
    /// </summary>
    /// <param name="other">
    /// Collision associated with the gameObject.
    /// </param>
    private void HandlePlayer(Collision other)
    {
        // Incapacitate player
        other.gameObject.GetComponent<PlayerHealth>().IsIncapacitated = true;
        // Hurt player
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        playerHealth.Hurt(base.ComputeMagnitude());
        // If not dead, respawn player
        if (!playerHealth.IsDead)
        {
            other.gameObject.GetComponent<PlayerTeleporter>().DelayedTeleport();
        }
    }
    #endregion
}
}