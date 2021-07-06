using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace EGS
{
    /// <summary>
    /// Instance of Health used for the Player characters. Adds the ability to be
    /// incapacitated. Specializes player death so GameObject not destroyed.
    /// </summary>
    public class PlayerHealth : Health
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The PlayerStats component of the player.
        /// </summary>
        private PlayerStats _playerStats = default;

        //=====================================================================
        // [Header("Death")]
        /// <summary>
        /// The PlayerController component necessary to toggle controls.
        /// </summary>
        private PlayerController _playerController;
        #endregion

        //=====================================================================
        #region Monobehavior
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        private void Start()
        {
            InitOnStart();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
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
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _playerController = GetComponent<PlayerController>();
            _playerStats = GetComponent<PlayerStats>();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_playerController, gameObject.name + " is missing _playerController");
            Assert.IsNotNull(_playerStats, gameObject.name + " is missing _playerStats");
        }
        #endregion


        //=====================================================================
        #region Health point manipulation
        //=====================================================================
        protected override void ProcessHealthChange(int newHealth, int magnitude)
        {
            // Invariant: 0 <= currentHealth <= maximumHealth
            if (newHealth > base.maximumHealth) { newHealth = base.maximumHealth; }
            else if (newHealth < 0) { newHealth = 0; }
            // If _health has changed...
            if (newHealth != base.currentHealth)
            {
                // If being healed/hurt, react accordingly
                if (magnitude > 0) { ReactToHeal(magnitude); }
                else { ReactToHurt(magnitude); }
                // Update internal state and send event
                currentHealth = newHealth;
                InvokeHealthUpdatedEvent();
                // If out of _health...
                if (IsOutOfHealth())
                {
                    // Kill if last living player
                    if (LARSGameSession.Instance.GetLivingPlayerCount() == 1) { this.IsDead = true; }
                    // Otherwise, incapacitate
                    else { _playerStats.IsIncapacitated = true; }
                }
            }
        }

        /// <summary>
        /// To be called when player is hurt. Drops lifted objects and activates
        /// invulnerability.
        /// </summary>
        protected override void ReactToHurt(int value)
        {
            _playerController.DropLiftedObjects();
            if (!isInvulnerable) { StartCoroutine(ActivateInvulnerability()); }
            TextPopup.CreateIntPopup(
                transform.position + new Vector3(0, 2, 0),
                value,
                TextAnimName.DAMAGE);
        }

        /// <summary>
        /// To be called when player has been healed. Removes incapacitation
        /// effects.
        /// </summary>
        protected override void ReactToHeal(int value)
        {
            _playerStats.IsIncapacitated = false;
            TextPopup.CreateIntPopup(
                transform.position + new Vector3(0, 2, 0),
                value,
                TextAnimName.DAMAGE);
        }
        #endregion

        //=====================================================================
        #region Death
        //=====================================================================
        /// <summary>
        /// To be called when the GameObject has died. Plays death effects, invokes
        /// death event, and toggles the death status effect for the player.
        /// </summary>
        protected override void ReactToDeath()
        {
            PlayDeathEffects();
            InvokeDeathEvent();
            _playerStats.IsDead = true;
        }

        /// <summary>
        /// Called when IsDead is toggled to false.
        /// </summary>
        protected override void ReactToResurrection()
        {
            _playerStats.IsDead = false;
        }
        #endregion
    }
}
