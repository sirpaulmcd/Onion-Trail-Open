using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace EGS
{
/// <summary>
/// Instance of Health used for the Player characters. Responsible for tracking
/// _health as well as death animation. Adds the ability to be incapacitated.
/// </summary>
public class PlayerHealth : Health
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// Boolean indicating whether player is currently incapacitated. That is,
    /// paralyzed without _health but not dead.
    /// </summary>
    private bool _isIncapacitated = false;
    /// <summary>
    /// The number of seconds a player can be incapacitated before they die.
    /// </summary>
    [SerializeField] private int _incapacitationSeconds = 5;
    /// <summary>
    /// The PlayerController component necessary to toggle controls.
    /// </summary>
    private PlayerController _playerController;
    /// <summary>
    /// The rigidbody component necessary to toggle limbo effects.
    /// </summary>
    private Rigidbody _rigidbody;
    /// <summary>
    /// The amount of HP an incapacitated player is given upon revival.
    /// </summary>
    [SerializeField] private int _revivalHealth = 15;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The current _health of the player. When setting, ensures player _health 
    /// is between zero and maximum _health (inclusive). Sends HealthUpdated 
    /// events to observers when _health changes. If player _health is zero, 
    /// sends death event to observer and destroys player instance.
    /// </summary>
    public override int CurrentHealth
    {
        get { return base.currentHealth; }
        set
        {
            int newHealth = value;
            // Invariant: 0 <= currentHealth <= maximumHealth
            if (newHealth > base.maximumHealth) { newHealth = base.maximumHealth; }
            else if (newHealth < 0) { newHealth = 0; }
            // If _health has changed...
            if (newHealth != base.currentHealth)
            {
                // If being healed/hurt, react accordingly
                if (newHealth > base.currentHealth) { ReactToHeal(); }
                else { ReactToHurt(); }
                // Update internal state and send event
                currentHealth = newHealth;
                InvokeHealthUpdatedEvent();
                // If out of _health...
                if (IsOutOfHealth())
                {
                    // Kill if last living player
                    if (LARSGameSession.Instance.GetLivingPlayerCount() == 1) { base.IsDead = true; }
                    // Otherwise, incapacitate
                    else { IncapacitatePlayer(); }
                }
            }
        }
    }

    /// <summary>
    /// Whether or not the GameObject is dead.
    /// </summary>
    public override bool IsDead 
    {
        get { return base.isDead; }
        set 
        {
            if (value != base.isDead)
            {
                if (value) { ReactToDeath(); }
                else { ReactToResurrection(); }
                base.isDead = value;
            }
        }
    }

    /// <summary>
    /// Boolean indicating whether player is currently incapacitated. That is,
    /// paralyzed without _health but not dead. If true, paralyzes player 
    /// controls. If false, player controls returned.
    /// </summary>
    public bool IsIncapacitated
    {
        get { return _isIncapacitated; }
        set 
        {
            if (value != _isIncapacitated)
            {
                if (value) { ReactToIncapacitation(); }
                else { ReactToRevival(); }
                _isIncapacitated = value;
            }
        }
    }
    #endregion
    
    //=========================================================================
    #region Monobehavior
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
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Revives the player from incapacitation.
    /// </summary>
    public void Revive()
    {
        this.Heal(_revivalHealth);
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
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// To be called when player is hurt. Drops lifted objects and activates
    /// invulnerability.
    /// </summary>
    protected override void ReactToHurt()
    {
        DropLiftedObjects();
        if (!isInvulnerable) { StartCoroutine(ActivateInvulnerability()); }
    }

    /// <summary>
    /// To be called when player has been healed. Removes incapacitation
    /// effects.
    /// </summary>
    protected override void ReactToHeal()
    {
        this.IsIncapacitated = false;
    }

    /// <summary>
    /// To be called when a player has died. Plays death effects, sends player
    /// to limbo, and invokes death event.
    /// </summary>
    protected override void ReactToDeath()
    {
        PlayDeathEffects();
        SendToLimbo();
        // Identify player as dead
        PlayerManager.Instance.UpdatePlayerState(PlayerState.Dead, gameObject);
        InvokeDeathEvent();
    }

    /// <summary>
    /// Plays death effects.
    /// </summary>
    protected override void PlayDeathEffects()
    {
        AudioManager.Instance.PlaySFX(deathSound, deathSoundVolume);
        GameObject explosion = Instantiate(deathParticleEffect, transform.position, transform.rotation);
        Destroy(explosion, deathEffectDuration);
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerController = GetComponent<PlayerController>();
    }
    
    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_playerController, gameObject.name + " is missing _playerController");
        Assert.IsNotNull(_rigidbody, gameObject.name + " is missing _rigidbody");
    }

    /// <summary>
    /// Incapacitates the player. If the player is not healed in 
    /// _incapacitationSeconds seconds, they die.
    /// </summary>
    private void IncapacitatePlayer()
    {
        // Incapacitate player
        this.IsIncapacitated = true;
        // If all players incapacitated, kill all players
        if (LARSGameSession.Instance.AllIncapacitatedOrDead()) { LARSGameSession.Instance.KillAllPlayers(); }
        // If all players not yet incapacitated...
        else { StartCoroutine(StartIncapacitationTimer()); }
    }

    /// <summary>
    /// Checks if the player has been revived (i.e. is not out of _health) before
    /// _incapacitationSeconds has elapsed. If not, kills the player.
    /// </summary>
    private IEnumerator StartIncapacitationTimer()
    {
        // Check if player revived before _incapacitationSeconds
        bool revived = false;
        for (int i = 0; i < _incapacitationSeconds; i++)
        {
            yield return new WaitForSeconds(1);
            if (!IsOutOfHealth()) { revived = true; }
        }
        // If player not yet revived and not already dead, kill player
        if (!revived && !this.IsDead) { this.IsDead = true; }
    }

    /// <summary>
    /// To be called when the player has been resurrected. Heals player to full
    /// _health and undoes limbo effects.
    /// </summary>
    private void ReactToResurrection()
    {
        this.CurrentHealth = this.MaximumHealth;
        UndoLimboEffects();
    }

    /// <summary>
    /// To be called when the player has been incapacitated. Applies 
    /// incapacitation effects and updates player state.
    /// </summary>
    private void ReactToIncapacitation()
    {
        DropLiftedObjects();
        _playerController.IsIncapacitated = true;
        PlayerManager.Instance.UpdatePlayerState(PlayerState.Incapacitated, gameObject);
    }

    /// <summary>
    /// To be called when the player has been revived from incapacitation.
    /// Removes incapcitation effects and updates player state.
    /// </summary>
    private void ReactToRevival()
    {
        _playerController.IsIncapacitated = false;
        PlayerManager.Instance.UpdatePlayerState(PlayerState.Default, gameObject);
    }

    /// <summary>
    /// Drops what the player is currently carrying, if anything.
    /// </summary>
    private void DropLiftedObjects()
    {
        _playerController.DropLiftedObjects();
    }

    /// <summary>
    /// Sends the player to the limbo position and applies limbo effects.
    /// Removes player from camera focus, drops lifted objects, incapacitates 
    /// them so they can't move, converts them to a kinematic object so they 
    /// cannot be influenced by physics, and teleports them to the limbo 
    /// position. 
    /// </summary>
    /// <remarks>
    /// The GameObject is sent to limbo rather than being destroyed as that 
    /// would delete the PlayerInput component and disconnect the player from 
    /// the controller. 
    /// </remarks>
    private void SendToLimbo()
    {
        DropLiftedObjects();
        Camera.main.GetComponent<LARSCameraMovement>().RemoveFocus(this.gameObject.transform);
        _playerController.IsIncapacitated = true;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        _rigidbody.isKinematic = true;
        this.gameObject.transform.position = LARSGameSession.Instance.LimboPosition;
    }

    /// <summary>
    /// Undoes limbo effects applied by SendToLimbo(). Teleportation back to
    /// map is handled by LARSGameSession.
    /// </summary>
    private void UndoLimboEffects()
    {
        _playerController.IsIncapacitated = false;
        _rigidbody.isKinematic = false;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Camera.main.GetComponent<LARSCameraMovement>().AddFocus(this.gameObject.transform);
        PlayerManager.Instance.UpdatePlayerState(PlayerState.Default, gameObject);
    }
    #endregion
}
}