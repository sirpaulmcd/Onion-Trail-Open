using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This script manages player specific stats and status effects.
    /// </summary>
    public class PlayerStats : EntityStats
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The Controller component of the player.
        /// </summary>
        protected new PlayerController controller = default;
        /// <summary>
        /// The Health component of the player.
        /// </summary>
        protected new PlayerHealth health = default;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        protected override void Start()
        {
            base.Start();
            InitOnStart();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private void InitOnStart()
        {
            InitVars();
            CheckMandatoryComponents();
        }

        private void InitVars()
        {
            controller = GetComponent<PlayerController>();
            health = GetComponent<PlayerHealth>();
            // Incapacitation variables
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(controller, gameObject.name + " is missing controller");
            Assert.IsNotNull(health, gameObject.name + " is missing health");
            // Incapacitation variables
            Assert.IsNotNull(_animator, gameObject.name + " is missing _animator");
            Assert.IsNotNull(_spriteRenderer, gameObject.name + " is missing _spriteRenderer");
        }
        #endregion

        //=====================================================================
        #region Knockback
        //=====================================================================
        /// <summary>
        /// Adds knockback effects.
        /// </summary>
        /// <remarks>
        /// Since the player moves by setting their rigidbody velocity in the
        /// movement direction, movement must be frozen when velocity based
        /// knockback is applied to make sure it isn't overwritten.
        /// </remarks>
        protected override void AddKnockbackEffects()
        {
            controller.FreezeMovement = true;
        }

        /// <summary>
        /// Removes knockback effects.
        /// </summary>
        protected override void RemoveKnockbackEffects()
        {
            controller.FreezeMovement = false;
        }
        #endregion

        //=====================================================================
        #region Incapacitation
        //=====================================================================
        // [Header("Instance variables")]
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
        /// The SpriteRenderer used to render the player.
        /// </summary>
        private SpriteRenderer _spriteRenderer;
        /// <summary>
        /// The amount of HP an incapacitated player is given upon revival.
        /// </summary>
        [SerializeField] private int _revivalHealth = 15;
        /// <summary>
        /// The Animator used to update animator variables.
        /// </summary>
        private Animator _animator;

        //=====================================================================
        // [Header("Properties")]
        /// <summary>
        /// Boolean indicating whether player is currently incapacitated. That is,
        /// paralyzed without health but not dead. If true, paralyzes player
        /// controls. If false, player controls returned.
        /// </summary>
        public bool IsIncapacitated
        {
            get { return _isIncapacitated; }
            set
            {
                if (value != _isIncapacitated)
                {
                    if (value) { AddIncapacitationEffects(); }
                    else { RemoveIncapacitationEffects(); }
                    _isIncapacitated = value;
                    _animator.SetBool("isIncapacitated", _isIncapacitated);
                }
            }
        }

        //=====================================================================
        // [Header("Public methods")]
        /// <summary>
        /// Revives the player from incapacitation. Called when player is lifted
        /// by another player when incapacitated.
        /// </summary>
        public void Revive()
        {
            this.IsIncapacitated = false;
            health.Heal(_revivalHealth);
        }

        //=====================================================================
        // [Header("Private methods")]
        /// <summary>
        /// Checks if the player has been revived (i.e. is not out of _health) before
        /// _incapacitationSeconds has elapsed. If not, kills the player.
        /// </summary>
        private IEnumerator StartIncapacitationTimer()
        {
            // Wait approximately _incapacitation seconds
            for (int i = 0; i < _incapacitationSeconds; i++)
            {
                yield return new WaitForSeconds(1);
                // If player revivved, end timer
                if (health.CurrentHealth != 0) { yield break; }
            }
            // If player not yet revived and not already dead, kill player
            if (!health.IsDead) { health.IsDead = true; }
        }

        /// <summary>
        /// To be called when the player has been incapacitated. Applies
        /// incapacitation effects and updates player state.
        /// </summary>
        private void AddIncapacitationEffects()
        {
            // Add effects to newly incapacitated player
            controller.DropLiftedObjects();
            controller.FreezeMovement = true;
            _spriteRenderer.flipY = true;
            PlayerManager.Instance.ChangePlayerControls(gameObject, ActionMapName.PARALYZED);
            PlayerManager.Instance.UpdatePlayerState(PlayerState.Incapacitated, gameObject);
            // If all players incapacitated, kill all players
            if (LARSGameSession.Instance.AllIncapacitatedOrDead())
            {
                LARSGameSession.Instance.KillAllPlayers();
            }
            // If all players not yet incapacitated...
            else { StartCoroutine(StartIncapacitationTimer()); }
        }

        /// <summary>
        /// To be called when the player has been revived from incapacitation.
        /// Removes incapcitation effects and updates player state.
        /// </summary>
        private void RemoveIncapacitationEffects()
        {
            controller.FreezeMovement = false;
            _spriteRenderer.flipY = false;
            PlayerManager.Instance.RevertPlayerControls(gameObject);
            PlayerManager.Instance.UpdatePlayerState(PlayerState.Default, gameObject);
        }
        #endregion

        //=====================================================================
        #region Death (Player specific)
        //=====================================================================
        // [Header("Instance variables")]
        private bool _isDead = false;

        //=====================================================================
        // [Header("Properties")]
        /// <summary>
        /// Whether or not the GameObject is dead.
        /// </summary>
        public bool IsDead
        {
            get { return _isDead; }
            set
            {
                if (value != _isDead)
                {
                    if (value) { AddDeathEffects(); }
                    else { RemoveDeathEffects(); }
                    _isDead = value;
                }
            }
        }

        //=====================================================================
        // [Header("Private methods")]
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
        private void AddDeathEffects()
        {
            this.IsIncapacitated = false;
            controller.DropLiftedObjects();
            controller.FreezeMovement = true;
            Camera.main.GetComponent<LARSCameraMovement>().RemoveFocus(this.gameObject.transform);
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rigidbody.isKinematic = true;
            this.gameObject.transform.position = LARSGameSession.Instance.LimboPosition;
            PlayerManager.Instance.UpdatePlayerState(PlayerState.Dead, gameObject);
        }

        /// <summary>
        /// To be called when the player has been resurrected. Heals player to full
        /// health and undoes limbo effects. Teleportation back to map is handled
        /// by LARSGameSession.
        /// </summary>
        private void RemoveDeathEffects()
        {
            health.Heal(health.MaximumHealth);
            controller.FreezeMovement = false;
            rigidbody.isKinematic = false;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            Camera.main.GetComponent<LARSCameraMovement>().AddFocus(this.gameObject.transform);
            PlayerManager.Instance.UpdatePlayerState(PlayerState.Default, gameObject);
        }
        #endregion
    }
}
