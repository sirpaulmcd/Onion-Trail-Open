using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This class is a temporary way for objects to deal damage to players upon
/// collision. Likely will be replaced by the WAP system for consistency.
/// Currently, this class is just for testing purposes.
/// </summary>
public class DamageDealer : MonoBehaviour
{
    //=========================================================================
    #region  Instance variables
    //=========================================================================
    /// <summary>
    /// The amount of damage to be dealt upon collision.
    /// </summary>
    [SerializeField] private int _damageAmount = 100;
    #endregion

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
    private void OnCollisionEnter(Collision collision) 
    {
        DealDamage(collision);
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Deals _damageAmount damage to colliding object if it has a health 
    /// component.
    /// </summary>
    /// <param name="collision"> 
    /// Collision associated with GameObject touching this object.
    /// </param> 
    private void DealDamage(Collision collision)
    {
        IHealth health = collision.gameObject.GetComponent<IHealth>();
        health?.Hurt(_damageAmount);
    }
    #endregion
}
}