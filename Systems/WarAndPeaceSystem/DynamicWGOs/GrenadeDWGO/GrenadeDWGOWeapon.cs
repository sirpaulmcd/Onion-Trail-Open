using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instance of Dynamic WGO Weapons that spawns GrenadeDWGOs.
/// </summary>
public class GrenadeDWGOWeapon : ADynamicWGOWeapon, IGrenadeDWGO
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    // [Header("IGrenade variables")] 
    /// <summary>
    /// The health point damage induced by the explosion.
    /// </summary>
    [SerializeField] protected int explosionDamage = default;
    /// <summary>
    /// The particle effect associated with the explosion.
    /// </summary>
    [SerializeField] protected GameObject explosionEffect = default;
    /// <summary>
    /// The duration of the explosion particle effect.
    /// </summary>
    [SerializeField] protected float explosionEffectDuration = 1f;
    /// <summary>
    /// The pushing force of the explosion.
    /// </summary>
    [SerializeField] protected float explosionForce = 100f;
    /// <summary>
    /// The radius of the explosion.
    /// </summary>
    [SerializeField] protected float explosionRadius = 3f;
    /// <summary>
    /// The sound of the explosion.
    /// </summary>
    [SerializeField] protected AudioClipName explosionSound = default;
    /// <summary>
    /// The volume of the explosion sound.
    /// </summary>
    [SerializeField] protected float explosionVolume = 0.5f;
    /// <summary>
    /// The vertical lift of the grenade when thrown.
    /// </summary>
    [SerializeField] protected float verticalLift = 10f;

    //=========================================================================
    // [Header("GrenadeGOWeaponBase variables")] 
    /// <summary>
    /// The grenade DWGO prefab instantiated by this weapon.
    /// </summary>
    [SerializeField] protected GameObject grenadePrefab = default;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("IGrenade properties")]
    /// <summary>
    /// The health point damage induced by the explosion.
    /// </summary>
    public int ExplosionDamage 
    { 
        get => explosionDamage;
        set => explosionDamage = value;
    }
    /// <summary>
    /// The particle effect associated with the explosion.
    /// </summary>
    public GameObject ExplosionEffect 
    { 
        get => explosionEffect; 
        set => explosionEffect = value;
    }
    /// <summary>
    /// The duration of the explosion particle effect.
    /// </summary>
    public float ExplosionEffectDuration
    { 
        get => explosionEffectDuration; 
        set => explosionEffectDuration = value;
    }
    /// <summary>
    /// The pushing force of the explosion.
    /// </summary>
    public float ExplosionForce
    { 
        get => explosionForce; 
        set => explosionForce = value;
    }
    /// <summary>
    /// The radius of the explosion.
    /// </summary>
    public float ExplosionRadius
    { 
        get => explosionRadius; 
        set => explosionRadius = value;
    }
    /// <summary>
    /// The sound of the explosion.
    /// </summary>
    public AudioClipName ExplosionSound
    { 
        get => explosionSound; 
        set => explosionSound = value;
    }
    /// <summary>
    /// The volume of the explosion sound.
    /// </summary>
    public float ExplosionVolume
    { 
        get => explosionVolume; 
        set => explosionVolume = value;
    }
    /// <summary>
    /// The vertical lift of the grenade when thrown.
    /// </summary>
    public float VerticalLift
    { 
        get => verticalLift; 
        set => verticalLift = value;
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Instantiates and initilaizes a self-regulating grenade prefab in front 
    /// of the attacker in the attack direction.
    /// </summary>
    /// <param name="direction">
    /// The direction of the attack.
    /// </param>
    public override void Attack(Vector3 direction)
    {
        if (onCooldown) { return; }
        if (this.CooldownSeconds > 0) { StartCoroutine(base.Cooldown()); }
        GameObject grenade = Instantiate(grenadePrefab, 
            this.Attacker.transform.position + direction * this.SpawnOffsetDistance, 
            Quaternion.identity);
        grenade.GetComponent<GrenadeDWGO>().Init(this, this.Attacker, direction);
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected override void InitOnStart()
    {
        base.InitOnStart();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.CheckMandatoryComponents();
        Assert.IsNotNull(grenadePrefab, gameObject.name + " is missing grenadePrefab");
    }
    #endregion
}
}