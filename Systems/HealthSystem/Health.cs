using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
public class Health : EventInvoker, IHealth
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// DEFAULT_INITIALIZED_HEALTH is used to set the default values of
    /// currentHealth and maximumHealth. Usually these may be set with the Unity
    /// inspector, but that is not possible for unit testing, so a default value
    /// is created for instances of Health that are created programatically.
    /// </summary>
    [NonSerialized] public const int DEFAULT_INITIALIZED_HEALTH = 100;
    /// <summary>
    /// Current _health of the GameObject.
    /// </summary>
    [SerializeField] protected int currentHealth = DEFAULT_INITIALIZED_HEALTH;
    /// <summary>
    /// Max _health of the GameObject.
    /// </summary>
    [SerializeField] protected int maximumHealth = DEFAULT_INITIALIZED_HEALTH;
    /// <summary>
    /// Whether or not the GameObject is dead.
    /// </summary>
    protected bool isDead = false;
    /// <summary>
    /// Boolean indicating if GameObject is currently invulnerable.
    /// </summary>
    protected bool isInvulnerable = false;
    /// <summary>
    /// The number of seconds a GameObject is to be invulnerable after being 
    /// hit.
    /// </summary>
    [SerializeField] protected float invulnerabilitySeconds = 0.2f;
    /// <summary>
    /// The duration that death particle effect should play.
    /// </summary>
    [SerializeField] protected int deathEffectDuration = 1;
    /// <summary>
    /// The volume of the death sound played upon GameObject death.
    /// </summary>
    [SerializeField] [Range(0,1)] protected float deathSoundVolume = 0.5f;
    #endregion

    //=========================================================================
    #region Inspector linked variables
    //=========================================================================
    /// <summary>
    /// The death effect played upon GameObject death.
    /// </summary>
    [SerializeField] protected GameObject deathParticleEffect = default;
    /// <summary>
    /// The death sound played upon GameObject death.
    /// </summary>
    [SerializeField] protected AudioClipName deathSound = default;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// Max _health of the GameObject.
    /// </summary>
    public int MaximumHealth
    {
        get { return maximumHealth; }
    }

    /// <summary>
    /// Current _health of the GameObject. Ensures proper set value and triggers
    /// events for updates in _health and death.
    /// </summary>
    public virtual int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            int newHealth = value;
            // Invariant: 0 <= currentHealth <= maximumHealth
            if (newHealth > this.MaximumHealth) { newHealth = this.MaximumHealth; }
            else if (newHealth < 0) { newHealth = 0; }
            // If _health has changed...
            if (newHealth != currentHealth)
            {
                // If being healed/hurt, react accordingly
                if (newHealth > currentHealth) { ReactToHeal(); }
                else { ReactToHurt(); }
                // Update internal state and send event
                currentHealth = newHealth;
                InvokeHealthUpdatedEvent();
                // If GameObject is out of _health...
                if (IsOutOfHealth()) { this.IsDead = true; }
            }
        }
    }

    /// <summary>
    /// Whether or not the GameObject is dead.
    /// </summary>
    public virtual bool IsDead 
    {
        get { return isDead; }
        set 
        {
            if (value != isDead)
            {
                if (value) { ReactToDeath(); }
                isDead = value;
            }
        }
    }

    /// <summary>
    /// Whether or not the GameObject is invulnerable to damage.
    /// </summary>
    public virtual bool IsInvulnerable
    { 
        get { return isInvulnerable; } 
        set { isInvulnerable = value; }
    }
    #endregion
    
    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    /// <summary>
    /// Invokes a DeathEvent.
    /// </summary>
    protected virtual void InvokeDeathEvent()
    {
        if (invokableEvents[EventName.DeathEvent] != null)
        {
            invokableEvents[EventName.DeathEvent]?.Invoke(this, new DeathEventArgs());
        }
    }
    
    /// <summary>
    /// Invokes a HealthUpdatedEvent.
    /// </summary>
    protected virtual void InvokeHealthUpdatedEvent()
    {
        if (invokableEvents[EventName.HealthUpdatedEvent] != null)
        {
            HealthUpdatedEventArgs args = new HealthUpdatedEventArgs(this.MaximumHealth, this.CurrentHealth);
            invokableEvents[EventName.HealthUpdatedEvent].Invoke(this, args);
        }
    }
    #endregion
    
    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Deal damage to the GameObject.
    /// </summary>
    /// <param name="HP">
    /// Units of damage to be dealt.
    /// </remarks>
    public void Hurt(int HP)
    {
        if (!isInvulnerable) { this.CurrentHealth -= HP; }
    }
    
    /// <summary>
    /// Heal the GameObject.
    /// </summary>
    /// <param name="HP">
    /// Units of _health to be restored.
    /// </remarks>
    public void Heal(int HP)
    {
        this.CurrentHealth += HP;
    }

    /// <summary>
    /// Kills the GameObject.
    /// </summary>
    public void Kill()
    {
        this.IsDead = true;
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected virtual void InitOnStart()
    {
        CheckInvariants();
        RegisterAsInvoker();
    }

    /// <summary>
    /// Returns bool indicating whether the GameObject is out of _health.
    /// </summary>
    /// <returns>
    /// True if GameObject _health is less than or equal to zero, false 
    /// otherwise.
    /// </returns>
    protected bool IsOutOfHealth()
    {
        return this.CurrentHealth <= 0;
    }

    /// <summary>
    /// To be called when GameObject has been healed.
    /// </summary>
    protected virtual void ReactToHeal()
    {
        // Healing effects to be implemented later...
    }

    /// <summary>
    /// To be called when the GameObject has been hurt. Activates 
    /// invulnerability.
    /// </summary>
    protected virtual void ReactToHurt()
    {
        if (!isInvulnerable) { StartCoroutine(ActivateInvulnerability()); }
    }

    /// <summary>
    /// Makes the player invulnerable for invulnerabilitySeconds seconds.
    /// </summary>
    /// <returns>
    /// The IEnumerator corresponding to the coroutine.
    /// </returns>
    protected virtual IEnumerator ActivateInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilitySeconds);
        isInvulnerable = false;
    }

    /// <summary>
    /// To be called when the GameObject has died. Plays death effects, invokes
    /// death event, and destroys the GameObject.
    /// </summary>
    protected virtual void ReactToDeath()
    {
        PlayDeathEffects();
        InvokeDeathEvent();
        Destroy(transform.gameObject);
    }

    /// <summary>
    /// Plays death effects.
    /// </summary>
    protected virtual void PlayDeathEffects()
    {
        AudioManager.Play(deathSound, deathSoundVolume);
        if (deathParticleEffect != null)
        {
            GameObject explosion = Instantiate(deathParticleEffect, transform.position, transform.rotation);
            Destroy(explosion, deathEffectDuration);
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Checks component invariants.
    /// </summary>
    private void CheckInvariants()
    {
        Assert.IsTrue(0 <= this.CurrentHealth);
        Assert.IsTrue(this.CurrentHealth <= this.MaximumHealth);
    }

    /// <summary>
    /// Registers as an invoker of various events.
    /// </summary>
    private void RegisterAsInvoker()
    {
        // Register as invoker for DeathEvent
		invokableEvents.Add(EventName.DeathEvent, DeathEvent.GetDeathEventHandler());
		EventManager.AddInvoker(EventName.DeathEvent, this);
        // Register as invoker for HealthUpdatedEvent
		invokableEvents.Add(EventName.HealthUpdatedEvent, HealthUpdatedEvent.GetHealthUpdatedEventHandler());
		EventManager.AddInvoker(EventName.HealthUpdatedEvent, this);
    }
    #endregion
}
}