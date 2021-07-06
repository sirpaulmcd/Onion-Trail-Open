// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instances of weapons that, once activated, spawn/initialize self regulating
    /// DWGOs (i.e. projectiles) that deal damage/healing. For example, under this
    /// system, a gun (DWGO Weapon) that is activated by an entity (player or
    /// enemy) would create bullets (self regulating DWGOs) that deal damage. All
    /// variables are stored in the weapon and then injected into the projectile
    /// after it has been instantiated. This way, all parameter tweaking is
    /// performed in one place. As such, all Weapons inherit from the interface
    /// corresponding to their assigned DWGO. For example, Piercing DWGO Weapons
    /// inherit from the IPiercingDWGO interface.
    /// </summary>
    public abstract class ADynamicWGOWeapon : AWeapon, IDynamicWGO
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("IDynamicWGO variables")]
        /// <summary>
        /// The speed of the DWGO.
        /// </summary>
        [SerializeField] protected float moveSpeed = 20f;
        /// <summary>
        /// The number of seconds before an instantiated DWGO is to self destruct.
        /// Necessary if the DWGO never hits anything.
        /// </summary>
        [SerializeField] protected float selfDestructSeconds = 2f;
        /// <summary>
        /// The distance from the attacker transform that the DWGO should be
        /// spawned.
        /// </summary>
        [SerializeField] protected float spawnOffsetDistance = 1f;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IDynamicWGO properties")]
        /// <summary>
        /// The speed of the DWGO.
        /// </summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }
        /// <summary>
        /// The number of seconds before an instantiated DWGO is to self destruct.
        /// Necessary if the DWGO never hits anything.
        /// </summary>
        public float SelfDestructSeconds
        {
            get => selfDestructSeconds;
            set => selfDestructSeconds = value;
        }
        /// <summary>
        /// The distance from the attacker transform that the DWGO should be
        /// spawned.
        /// </summary>
        public float SpawnOffsetDistance
        {
            get => spawnOffsetDistance;
            set => spawnOffsetDistance = value;
        }
        #endregion
    }
}
