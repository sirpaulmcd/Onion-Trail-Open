// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Interface that corresponds with Grenade DWGOs. See AGrenadeDWGO.cs.
/// </summary>
public interface IGrenadeDWGO : IDynamicWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The health point damage induced by the explosion.
    /// </summary>
    int ExplosionDamage { get; set; }
    /// <summary>
    /// The particle effect associated with the explosion.
    /// </summary>
    GameObject ExplosionEffect { get; set; }
    /// <summary>
    /// The duration of the explosion particle effect.
    /// </summary>
    float ExplosionEffectDuration { get; set; }
    /// <summary>
    /// The pushing force of the explosion.
    /// </summary>
    float ExplosionForce { get; set; }
    /// <summary>
    /// The radius of the explosion.
    /// </summary>
    float ExplosionRadius { get; set; }
    /// <summary>
    /// The sound of the explosion.
    /// </summary>
    SFXClipName ExplosionSound { get; set; }
    /// <summary>
    /// The volume of the explosion sound.
    /// </summary>
    float ExplosionVolume { get; set; }
    /// <summary>
    /// The vertical lift of the grenade when thrown.
    /// </summary>
    float VerticalLift { get; set; }
    #endregion
}
}