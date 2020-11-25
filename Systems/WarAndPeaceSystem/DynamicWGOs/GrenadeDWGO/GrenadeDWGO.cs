using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instance of Collider DWGO Weapons that are further specialized to provide 
/// explosive effects upon self destructing. Further specializations of this 
/// weapon can yield different explosive effects and projectile skins.
/// </summary>
public class GrenadeDWGO : AColliderDWGO, IGrenadeDWGO
{
    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("IGrenade properties")]
    /// <summary>
    /// The health point damage induced by the explosion.
    /// </summary>
    public int ExplosionDamage { get; set; }
    /// <summary>
    /// The particle effect associated with the explosion.
    /// </summary>
    public GameObject ExplosionEffect { get; set; }
    /// <summary>
    /// The duration of the explosion particle effect.
    /// </summary>
    public float ExplosionEffectDuration { get; set; }
    /// <summary>
    /// The pushing force of the explosion.
    /// </summary>
    public float ExplosionForce { get; set; }
    /// <summary>
    /// The radius of the explosion.
    /// </summary>
    public float ExplosionRadius { get; set; }
    /// <summary>
    /// The sound of the explosion.
    /// </summary>
    public AudioClipName ExplosionSound { get; set; }
    /// <summary>
    /// The volume of the explosion sound.
    /// </summary>
    public float ExplosionVolume { get; set; }
    /// <summary>
    /// The vertical lift of the grenade when thrown.
    /// </summary>
    public float VerticalLift { get; set; }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Initializes the grenade variables, sets the appropraite GameObject
    /// rotation, throws the grenade in the desired direction, and starts
    /// the self destruct countdown.
    /// </summary>
    /// <param name="weapon">
    /// The weapon script that fired this GameObject.
    /// </param>
    /// <param name="attacker">
    /// The GameObject initiating the attack/heal.
    /// </param>
    /// <param name="direction">
    /// The direction that the attack is to be made.
    /// </param>
    public void Init(IGrenadeDWGO weapon, GameObject attacker, Vector3 direction)
    {
        InitVars(weapon, attacker);
        StartCoroutine(StartSelfDestructCountdown());
        TossGrenade(direction);
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    protected void InitVars(IGrenadeDWGO weapon, GameObject attacker)
    {
        base.InitVars(weapon, attacker);
        // Initialise IGrenade variables
        this.ExplosionDamage = weapon.ExplosionDamage;
        this.ExplosionEffect = weapon.ExplosionEffect;
        this.ExplosionEffectDuration = weapon.ExplosionEffectDuration;
        this.ExplosionForce = weapon.ExplosionForce;
        this.ExplosionRadius = weapon.ExplosionRadius;
        this.ExplosionSound = weapon.ExplosionSound;
        this.ExplosionVolume = weapon.ExplosionVolume;
        this.VerticalLift = weapon.VerticalLift;
    }

    /// <summary>
    /// Tosses the grenade forward and up according to the input direction.
    /// </summary>
    /// <param name="direction">
    /// The direction that the attack is to be made.
    /// </param>
    protected void TossGrenade(Vector3 direction)
    {
        this.rigidbody.AddForce(Vector3.up * this.VerticalLift, ForceMode.Impulse);
        this.rigidbody.AddForce(direction * this.MoveSpeed, ForceMode.Impulse);
    }

    /// <summary>
    /// Waits SelfDestructSeconds and explodes.
    /// </summary>
    protected override IEnumerator StartSelfDestructCountdown()
    {
        yield return new WaitForSeconds(SelfDestructSeconds);
        Explode();
    }

    /// <summary>
    /// Explodes the grenade by playing effects, dealing damage, and applying 
    /// forces before destroying the GameObject.
    /// </summary>
    protected void Explode()
    {
        PlayExplosionEffects();
        DealDamage();
        ApplyForces();
        Destroy(gameObject);
    }

    /// <summary>
    /// Plays explosion sound effect and instantiates explosion particle effect.
    /// </summary>
    protected void PlayExplosionEffects()
    {
        AudioManager.Play(this.ExplosionSound, this.ExplosionVolume);
        GameObject explosion = Instantiate(this.ExplosionEffect, transform.position, transform.rotation);
        Destroy(explosion, this.ExplosionEffectDuration);
    }

    /// <summary>
    /// Deals damage to whatever colliders with health components that are 
    /// detected by an overlap sphere if they have a health component.
    /// </summary>
    protected void DealDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, this.ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            IHealth health = collider.gameObject.GetComponent<IHealth>();
            if (health != null) { health.Hurt(this.ExplosionDamage); }
        }
    }

    /// <summary>
    /// Applies explosion forces to all colliders with rigidbodies detected by 
    /// the overlap sphere. The overlap sphere from DealDamage is not reused
    /// in the case that a destructible object has broken apart and created new
    /// GameObjects to push.
    /// </summary>
    protected void ApplyForces()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, this.ExplosionRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
            if (rb != null) { rb.AddExplosionForce(this.ExplosionForce, transform.position, this.ExplosionRadius); }
        }
    }
    #endregion
}
}