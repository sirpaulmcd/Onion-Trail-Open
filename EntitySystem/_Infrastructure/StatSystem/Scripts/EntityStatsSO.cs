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
    [CreateAssetMenu(fileName = "EntityStatsSO", menuName = "ScriptableObjects/EntityStatsSO")]
    public class EntityStatsSO : ScriptableObject
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        //[Header("Health")]
        /// <summary>
        /// The maximum health of the entity.
        /// </summary>
        public int maximumHealth = 100;
        //=====================================================================
        //[Header("Controller")]
        /// <summary>
        /// The move speed of the controlled GameObject.
        /// </summary>
        public float moveSpeed = 350f;
        //=====================================================================
        //[Header("Combustion")]
        /// <summary>
        /// Whether the entity is combustible.
        /// </summary>
        public bool isCombustible = true;
        /// <summary>
        /// Move speed multiplier when combusting.
        /// </summary>
        public float combustionMoveSpeedMultiplier = 1.5f;
        /// <summary>
        /// Damage of a single combustion interval.
        /// </summary>
        public int combustionIntervalDamage = 5;
        /// <summary>
        /// Seconds between combustion damage intervals.
        /// </summary>
        public float combustionIntervalSeconds = 0.5f;
        /// <summary>
        /// The duration that the player is combusting.
        /// </summary>
        public float combustionDurationSeconds = 5f;
        #endregion
    }
}
