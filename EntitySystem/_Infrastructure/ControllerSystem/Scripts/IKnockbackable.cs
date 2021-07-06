// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Interface that corresponds with GameObjects whose knockback is inflicted
    /// by setting the rigidbody velocity in the knockback direction for a short
    /// period of time. See AEntityController.cs.
    /// </summary>
    public interface IKnockbackable
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("Knockback")]
        /// <summary>
        /// Whether the GameObject is currently experiencing knockback effects.
        /// </summary>
        bool IsKnockedBack { get; set; }
        /// <summary>
        /// Whether the knockbackable ignores knockback effects.
        /// </summary>
        bool IgnoreKnockback { get; set; }
        #endregion

        //=====================================================================
        #region Knockback
        //=====================================================================
        /// <summary>
        /// Calls the knockback coroutine that knocks the knockbackable in the
        /// input direction by the input magnitude.
        /// </summary>
        /// <param name="direction">
        /// The normalized directional Vector3 corresponding to the direction of
        /// knockback.
        /// </param>
        /// <param name="magnitude">
        /// The magnitude at which the knockbackable is knocked back.
        /// </param>
        void Knockback(Vector3 direction, float magnitude);
        #endregion
    }
}
