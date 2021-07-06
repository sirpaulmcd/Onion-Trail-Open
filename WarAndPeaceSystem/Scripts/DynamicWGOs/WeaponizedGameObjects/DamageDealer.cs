using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Instance of AWeaponizedGameObject that inflicts damage via
    /// OnCollisionEnter.
    /// </summary>
    public class DamageDealer : AWeaponizedGameObject
    {
        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
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

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Deals damage to colliding object if it has a health component.
        /// </summary>
        /// <param name="collision">
        /// Collision associated with GameObject touching this object.
        /// </param>
        private void DealDamage(Collision collision)
        {
            IHealth health = collision.gameObject.GetComponent<IHealth>();
            health?.Hurt(base.ComputeMagnitude());
        }
        #endregion
    }
}
