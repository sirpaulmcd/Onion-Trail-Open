using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    public class Health : MonoBehaviour, IHealth
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("Health point manipulation")]
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
        /// Boolean indicating if GameObject is currently invulnerable.
        /// </summary>
        protected bool isInvulnerable = false;
        /// <summary>
        /// The number of seconds a GameObject is to be invulnerable after being
        /// hit.
        /// </summary>
        [SerializeField] protected float invulnerabilitySeconds = 0.2f;

        //=====================================================================
        // [Header("Death")]
        /// <summary>
        /// Whether or not the GameObject is dead.
        /// </summary>
        protected bool isDead = false;
        /// <summary>
        /// The duration that death particle effect should play.
        /// </summary>
        [SerializeField] protected int deathEffectDuration = 1;
        /// <summary>
        /// The volume of the death sound played upon GameObject death.
        /// </summary>
        [SerializeField] [Range(0, 1)] protected float deathSoundVolume = 0.5f;
        /// <summary>
        /// The death effect played upon GameObject death.
        /// </summary>
        [SerializeField] protected GameObject deathParticleEffect = default;
        /// <summary>
        /// The death sound played upon GameObject death.
        /// </summary>
        [SerializeField] protected SFXClipName deathSound = default;
        /// <summary>
        /// The damage particle effect prefab. Not setting it means no effect.
        /// </summary>
        [SerializeField] protected GameObject damageParticleEffect = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("Health point manipulation")]
        /// <summary>
        /// Max _health of the GameObject.
        /// </summary>
        public int MaximumHealth
        {
            get { return maximumHealth; }
            private set { maximumHealth = value; }
        }

        /// <summary>
        /// Current _health of the GameObject. Ensures proper set value and triggers
        /// events for updates in _health and death.
        /// </summary>
        public virtual int CurrentHealth
        {
            get { return currentHealth; }
            private set { currentHealth = value; }
        }

        /// <summary>
        /// Whether or not the GameObject is invulnerable to damage.
        /// </summary>
        public virtual bool IsInvulnerable
        {
            get { return isInvulnerable; }
            set { isInvulnerable = value; }
        }

        //=====================================================================
        // [Header("Death")]
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
                    else { ReactToResurrection(); }
                    isDead = value;
                }
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour
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
        #region Event invokers
        //=====================================================================
        /// <summary>
        /// Invokes a DeathEvent.
        /// </summary>
        public virtual void InvokeDeathEvent()
        {
            Debug.Log("Health.InvokeDeathEvent: " + gameObject.name + " has died.");
            EventManager.Instance.Invoke(EventName.DeathEvent, new DeathEventArgs(), this);
        }

        /// <summary>
        /// Invokes a HealthUpdatedEvent.
        /// </summary>
        protected virtual void InvokeHealthUpdatedEvent()
        {
            Debug.Log("Health.InvokeHealthUpdatedEvent: " + gameObject.name + " health has changed.");
            EventManager.Instance.Invoke(EventName.HealthUpdatedEvent,
                new HealthUpdatedEventArgs(this.MaximumHealth, this.CurrentHealth),
                this);
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        protected virtual void InitOnStart()
        {
            CheckInvariants();
            PreventParticleEffectOnSpawn();
        }

        private void PreventParticleEffectOnSpawn()
        {
            if (damageParticleEffect)
            {
                damageParticleEffect.GetComponent<ParticleSystem>().Stop();
            }
        }

        /// <summary>
        /// Checks component invariants.
        /// </summary>
        private void CheckInvariants()
        {
            Assert.IsTrue(0 <= this.CurrentHealth);
            Assert.IsTrue(this.CurrentHealth <= this.MaximumHealth);
        }
        #endregion

        //=====================================================================
        #region Health point manipulation
        //=====================================================================
        /// <summary>
        /// Deal damage to the GameObject and plays the corresponding particle effect.
        /// </summary>
        /// <param name="HP">
        /// Units of damage to be dealt.
        /// </remarks>
        public void Hurt(int HP)
        {
            if (!isInvulnerable) { ProcessHealthChange(CurrentHealth - HP, -HP); }
        }

        /// <summary>
        /// Deal damage to the GameObject regardless of invulnerability.
        /// </summary>
        /// <param name="HP">
        /// Units of damage to be dealt.
        /// </remarks>
        public void HurtIgnoreInvulnerability(int HP)
        {
            ProcessHealthChange(CurrentHealth - HP, -HP);
        }

        /// <summary>
        /// Heal the GameObject.
        /// </summary>
        /// <param name="HP">
        /// Units of _health to be restored.
        /// </remarks>
        public void Heal(int HP)
        {
            ProcessHealthChange(CurrentHealth + HP, HP);
        }

        protected virtual void ProcessHealthChange(int newHealth, int magnitude)
        {
            // Invariant: 0 <= currentHealth <= maximumHealth
            if (newHealth > this.MaximumHealth) { newHealth = this.MaximumHealth; }
            else if (newHealth < 0) { newHealth = 0; }
            // If _health has changed...
            if (newHealth != currentHealth)
            {
                // If being healed/hurt, react accordingly
                if (magnitude > 0) { ReactToHeal(magnitude); }
                else { ReactToHurt(magnitude); }
                // Update internal state and send event
                currentHealth = newHealth;
                InvokeHealthUpdatedEvent();
                // If GameObject is out of health...
                if (IsOutOfHealth()) { this.IsDead = true; }
            }
        }

        /// <summary>
        /// To be called when GameObject has been healed.
        /// </summary>
        protected virtual void ReactToHeal(int value)
        {
            // Healing effects to be implemented later...
        }

        /// <summary>
        /// To be called when the GameObject has been hurt. Activates
        /// invulnerability.
        /// </summary>
        protected virtual void ReactToHurt(int value)
        {
            if (!isInvulnerable) { StartCoroutine(ActivateInvulnerability()); }
            TextPopup.CreateIntPopup(
                transform.position + new Vector3(0, 2, 0),
                value,
                TextAnimName.DAMAGE);
            if (damageParticleEffect)
            {
                damageParticleEffect.GetComponent<ParticleSystem>().Play();
            }
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
        #endregion

        //=====================================================================
        #region Death
        //=====================================================================
        /// <summary>
        /// Kills the GameObject.
        /// </summary>
        public void Kill()
        {
            this.IsDead = true;
        }

        /// <summary>
        /// Plays death effects.
        /// </summary>
        protected virtual void PlayDeathEffects()
        {
            AudioManager.Instance.PlaySFX(deathSound, deathSoundVolume);
            if (deathParticleEffect != null)
            {
                GameObject explosion = Instantiate(deathParticleEffect, transform.position, transform.rotation);
                Destroy(explosion, deathEffectDuration);
            }
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
        /// Called when IsDead is toggled to false.
        /// </summary>
        protected virtual void ReactToResurrection()
        {
            // To be overwritten by children...
        }
        #endregion
    }
}
