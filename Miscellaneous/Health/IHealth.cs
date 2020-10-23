using System;

namespace EGS
{
/// <summary>
/// Component which enables _health/death functionality on GameObject.
/// </summary>
/// <remarks>
/// This class is responsible for maintaining the HP system of a GameObject.
/// Other classes may subscribe to the HealthUpdated and Death events to learn
/// when this instance has an update to it's _health or is no longer alive.
/// </remarks>
/// <invariants>
/// This class should always ensure 0 <= currentHealth <= maximumHealth
/// </invariants>
public interface IHealth
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The maximum _health of the GameObject.
    /// </summary>
    int MaximumHealth { get; }
    /// <summary>
    /// The current _health of the GameObject.
    /// </summary>
    int CurrentHealth { get; }
    #endregion
    /// <summary>
    /// Whether or not the GameObject is dead.
    /// </summary>
    bool IsDead { get; }
    /// <summary>
    /// Whether or not the GameObject is invulnerable.
    /// </summary>
    bool IsInvulnerable { get; set; }

    //=========================================================================
    #region Public Methods
    //=========================================================================
    /// <summary>
    /// Deal damage to the object which holds this component.
    /// </summary>
    /// <param name="HP">
    /// Units of damage to be dealt.
    /// </remarks>
    void Hurt(int HP);

    /// <summary>
    /// Heal the object which holds this component.
    /// </summary>
    /// <param name="HP">
    /// Units of _health to be restored
    /// </remarks>
    void Heal(int HP);
    #endregion
}
}
