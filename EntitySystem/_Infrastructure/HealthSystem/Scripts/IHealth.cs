using System;

namespace EGS
{
    /// <summary>
    /// Interface which enables health/death functionality on GameObject.
    /// </summary>
    public interface IHealth
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("Health point manipulation")]
        /// <summary>
        /// The maximum health of the GameObject.
        /// </summary>
        int MaximumHealth { get; }
        /// <summary>
        /// The current health of the GameObject.
        /// </summary>
        int CurrentHealth { get; }
        /// <summary>
        /// Whether or not the GameObject is invulnerable.
        /// </summary>
        bool IsInvulnerable { get; set; }

        //=====================================================================
        // [Header("Death")]
        /// <summary>
        /// Whether or not the GameObject is dead.
        /// </summary>
        bool IsDead { get; }
        #endregion

        //=====================================================================
        #region Health point manipulation
        //=====================================================================
        /// <summary>
        /// Deal damage to the object which holds this component.
        /// </summary>
        /// <param name="HP">
        /// Units of health to be lost.
        /// </remarks>
        void Hurt(int HP);

        /// <summary>
        /// Heal the object which holds this component.
        /// </summary>
        /// <param name="HP">
        /// Units of health to be restored.
        /// </remarks>
        void Heal(int HP);

        /// <summary>
        /// Deal damage to the GameObject regardless of invulnerability.
        /// </summary>
        /// <param name="HP">
        /// Units of damage to be dealt.
        /// </remarks>
        void HurtIgnoreInvulnerability(int HP);
        #endregion
    }
}
