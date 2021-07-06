using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Any GameObject that is expected to deal damage/healing. A WGO calculates
    /// damage/healing magnitude based on several parameters. The magnitude is
    /// randomly selected between the minimum/maximum damage values. The critical
    /// chance determines whether this magnitude will be multiplied by the critical
    /// factor. If “friendly fire” is toggled, the weapon will hurt GameObjects of
    /// the same tag as the attacker. In addition to damaging/healing, a weaponized
    /// GameObject can also apply knockback effects from the specified origin. A
    /// desirable knockback origin changes depending on the implementation. For
    /// example, the knockback origin of a bullet should be the bullet itself
    /// whereas the knockback origin of a melee weapon should be the attacker.
    /// </summary>
    public abstract class AWeaponizedGameObject : MonoBehaviour, IWarAndPeace
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("IWarAndPeace variables")]
        /// <summary>
        /// The GameObject that initiated the attack/heal.
        /// </summary>
        protected GameObject attacker = default;
        /// <summary>
        /// The percent chance [0.0, 100.0] that an attack is a critical hit.
        /// </summary>
        [SerializeField] protected float criticalHitChance = 0f;
        /// <summary>
        /// The number which multiplies the damage value upon a critical hit.
        /// </summary>
        [SerializeField] protected float criticalHitMultiplier = 1f;
        /// <summary>
        /// Whether or not the attack can damage GameObjects of the same tag.
        /// </summary>
        [SerializeField] protected bool isFriendlyFire = false;
        /// <summary>
        /// Whether or not the attack is meant to heal.
        /// </summary>
        [SerializeField] protected bool isHeal = false;
        /// <summary>
        /// The magnitude that this object with push generic GameObjects with
        /// rigidbodies.
        /// </summary>
        [SerializeField] protected float knockbackMagnitude = 200f;
        /// <summary>
        /// The magnitude that this object with push GameObjects holding the
        /// IKnockbackable interface.
        /// </summary>
        [SerializeField] protected float knockbackableMagnitude = 1000f;
        /// <summary>
        /// The origin from which knockback forces will push away.
        /// </summary>
        protected GameObject knockbackOrigin = default;
        /// <summary>
        /// The upper-bound damage value for an attack/heal.
        /// </summary>
        [SerializeField] protected uint maximumDamage = 15;
        /// <summary>
        /// The lower-bound damage value for an attack/heal.
        /// </summary>
        [SerializeField] protected uint minimumDamage = 10;
        /// <summary>
        /// The name of the attack/heal.
        /// </summary>
        [SerializeField] protected string title = "BasicRanged";
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IWarAndPeace properties")]
        /// <summary>
        /// The GameObject that initiated the attack/heal.
        /// </summary>
        public GameObject Attacker
        {
            get => attacker;
            set => attacker = value;
        }
        /// <summary>
        /// The percent chance [0.0, 100.0] that an attack is a critical hit.
        /// </summary>
        /// <invariants>
        /// 0.0 <= criticalHitChance <= 100.0
        /// </invariants>
        public float CriticalHitChance
        {
            get => criticalHitChance;
            set => criticalHitChance = value;
        }
        /// <summary>
        /// The number which multiplies the damage value upon a critical hit.
        /// </summary>
        public float CriticalHitMultiplier
        {
            get => criticalHitMultiplier;
            set => criticalHitMultiplier = value;
        }
        /// <summary>
        /// Whether or not the attack can damage GameObjects of the same tag.
        /// </summary>
        public bool IsFriendlyFire
        {
            get => isFriendlyFire;
            set => isFriendlyFire = value;
        }
        /// <summary>
        /// Whether or not the attack is meant to heal.
        /// </summary>
        public bool IsHeal
        {
            get => isHeal;
            set => isHeal = value;
        }
        /// <summary>
        /// The magnitude that this object with push generic GameObjects with
        /// rigidbodies.
        /// </summary>
        public float KnockbackMagnitude
        {
            get => knockbackMagnitude;
            set => knockbackMagnitude = value;
        }
        /// <summary>
        /// The magnitude that this object with push GameObjects holding the
        /// IKnockbackable interface.
        /// </summary>
        public float KnockbackableMagnitude
        {
            get => knockbackableMagnitude;
            set => knockbackableMagnitude = value;
        }
        /// <summary>
        /// The origin from which knockback forces will push away.
        /// </summary>
        /// <remarks>
        /// This will typically be the attacker for a melee strike, and the
        /// projectile for a ranged strike.
        /// </remarks>
        public GameObject KnockbackOrigin
        {
            get => knockbackOrigin;
            set => knockbackOrigin = value;
        }
        /// <summary>
        /// The upper-bound damage value for an attack/heal.
        /// </summary>
        /// <invariants>
        /// minimumDamage <= maximumDamage
        /// </invariants>
        public uint MaximumDamage
        {
            get => maximumDamage;
            set => maximumDamage = value;
        }
        /// <summary>
        /// The lower-bound damage value for an attack/heal.
        /// </summary>
        /// <invariants>
        /// minimumDamage <= maximumDamage
        /// </invariants>
        public uint MinimumDamage
        {
            get => minimumDamage;
            set => minimumDamage = value;
        }
        /// <summary>
        /// The name of the attack/heal.
        /// </summary>
        public string Title
        {
            get => title;
            set => title = value;
        }
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Computes a value for the attack's damage (including crit.)
        /// </summary>
        /// <remarks>
        /// Determine if an attack is a critical hit. We use rng.NextDouble(),
        /// which returns a value in the range [0.0, 1.0), to construct an if()
        /// statement which is always false when criticalHitChance = 0.0, and
        /// always true when criticalHitChance = 100.
        /// </remarks>
        protected virtual int ComputeMagnitude()
        {
            System.Random rng = new System.Random();
            bool isCriticalHit = CriticalHitChance > rng.NextDouble() * 100.0;
            int damage = rng.Next((int)MinimumDamage, (int)MaximumDamage + 1);
            if (isCriticalHit) { damage = (int)Math.Round(damage * CriticalHitMultiplier); }
            return damage;
        }
        #endregion
    }
}
