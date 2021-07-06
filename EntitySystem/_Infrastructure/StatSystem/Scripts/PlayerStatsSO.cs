using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Scriptable object containing stat information inherent to all entities. It
    /// holds the base values for health and controller such as:
    /// - Maximum health
    /// - Movement speed
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "ScriptableObjects/PlayerStatsSO")]
    public class PlayerStatsSO : EntityStatsSO
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        //[Header("Controller")]
        /// <summary>
        /// The magnitude of the impulse used to jump the controlled GameObject.
        /// </summary>
        public float jumpMagnitude = 280f;
        #endregion
    }
}
