using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This script manages the stats and status effects of the entity.
    /// </summary>
    public class EntityStats : MonoBehaviour, IKnockbackable
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        //[Header("EntityStats basics")]
        /// <summary>
        /// The controller component of the entity.
        /// </summary>
        protected IEntityController controller = default;
        /// <summary>
        /// The health component of the entity.
        /// </summary>
        protected IHealth health = default;
        /// <summary>
        /// The Rigidbody component of the entity.
        /// </summary>
        protected new Rigidbody rigidbody = default;
        /// <summary>
        /// The EntityStatsSO for the GameObject.
        /// </summary>
        [SerializeField] protected EntityStatsSO stats = default;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        protected virtual void Start()
        {
            InitOnStart();
        }

        /// <summary>
        /// Called every physics update.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            HandleKnockback();
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            SpreadFire(other);
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
            controller = GetComponent<IEntityController>();
            health = GetComponent<Health>();
            rigidbody = GetComponent<Rigidbody>();
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(controller, gameObject.name + " is missing controller");
            Assert.IsNotNull(health, gameObject.name + " is missing health");
            Assert.IsNotNull(rigidbody, gameObject.name + " is missing rigidbody");
        }
        #endregion

        //=====================================================================
        #region Knockback
        //=====================================================================
        //[Header("Instance variables")]
        /// <summary>
        /// Whether or not the knockbackable ignores knockback effects.
        /// </summary>
        [SerializeField] protected bool ignoreKnockback = false;
        /// <summary>
        /// Whether the GameObject is currently experiencing knockback effects.
        /// </summary>
        protected bool isKnockedBack = false;
        /// <summary>
        /// The direction of knockback.
        /// </summary>
        protected Vector3 knockbackDirection = default;
        /// <summary>
        /// The magnitude of knockback.
        /// </summary>
        protected float knockbackMagnitude = default;
        /// <summary>
        /// The seconds that a velocity in the knockback direction is applied to
        /// the rigidbody. Should likely stay constant for all entities.
        /// </summary>
        protected const float KNOCKBACKSECONDS = 0.1f;

        //=====================================================================
        //[Header("Properties")]
        /// <summary>
        /// Whether the knockbackable ignores knockback effects.
        /// </summary>
        public bool IgnoreKnockback
        {
            get => ignoreKnockback;
            set => ignoreKnockback = value;
        }

        /// <summary>
        /// Whether the GameObject is currently experiencing knockback effects.
        /// </summary>
        public bool IsKnockedBack
        {
            get { return isKnockedBack; }
            set
            {
                if (isKnockedBack != value)
                {
                    if (value) { AddKnockbackEffects(); }
                    else { RemoveKnockbackEffects(); }
                    isKnockedBack = value;
                }
            }
        }

        //=====================================================================
        //[Header("Public methods")]
        /// <summary>
        /// Calls the knockback coroutine that knocks the knockbackable in the
        /// input direction by the input magnitude.
        /// </summary>
        /// <param name="direction">
        /// The normalized directional Vector3 corresponding to the direction of
        /// knockback.
        /// </param>
        /// <param name="magnitude">
        /// The magnitude at which the knockbackable is knocked back.
        /// </param>
        public virtual void Knockback(Vector3 direction, float magnitude)
        {
            if (!IgnoreKnockback) { StartCoroutine(SimulateKnockback(direction, magnitude)); }
        }

        //=====================================================================
        //[Header("Protected methods")]
        /// <summary>
        /// Knocks the knockbackable back in the input direction by the input
        /// magnitude.
        /// </summary>
        /// <param name="direction">
        /// The normalized directional Vector3 corresponding to the direction of
        /// knockback.
        /// </param>
        /// <param name="magnitude">
        /// The magnitude at which the knockbackable is knocked back.
        /// </param>
        protected virtual IEnumerator SimulateKnockback(Vector3 direction, float magnitude)
        {
            knockbackDirection = direction;
            knockbackMagnitude = magnitude;
            IsKnockedBack = true;
            yield return new WaitForSeconds(KNOCKBACKSECONDS);
            IsKnockedBack = false;
        }

        /// <summary>
        /// Adds knockback effects to the NavMeshAgent.
        /// </summary>
        protected virtual void AddKnockbackEffects()
        {
            // To be overridden by children
        }

        /// <summary>
        /// Removes knockback effects from the NavMeshAgent. A null check is
        /// required because the GameObject may have been killed before reaching
        /// their knockback destination.
        /// </summary>
        protected virtual void RemoveKnockbackEffects()
        {
            knockbackDirection = default;
            knockbackMagnitude = default;
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }

        /// <summary>
        /// Sets velocity to knockbackMagnitude in the knockbackDirection.
        /// </summary>
        protected virtual void HandleKnockback()
        {
            if (IsKnockedBack)
            {
                rigidbody.velocity = new Vector3(knockbackDirection.x, rigidbody.velocity.y, knockbackDirection.z).normalized * knockbackMagnitude * Time.fixedDeltaTime;
            }
        }
        #endregion

        //=====================================================================
        #region Combustion
        //=====================================================================
        //[Header("Instance variables")]
        /// <summary>
        /// Whether or not the entity is currently combusting.
        /// </summary>
        private bool isCombusting = false;
        /// <summary>
        /// Combustion damage coroutine.
        /// </summary>
        private IEnumerator combustionDamageCoroutine = default;
        /// <summary>
        /// Combustion particle effect.
        /// </summary>
        private Transform flamesParticleEffect = default;

        //=====================================================================
        //[Header("Properties")]
        /// <summary>
        /// Whether or not the entity is current combusting.
        /// </summary>
        public bool IsCombusting
        {
            get => isCombusting;
            set
            {
                if (stats.isCombustible && value != isCombusting)
                {
                    isCombusting = value;
                    if (value) { AddCombustionEffects(); }
                    else { RemoveCombustionEffects(); }
                }
            }
        }

        //=====================================================================
        //[Header("Methods")]
        /// <summary>
        /// Called in IsCombusting setter.
        /// </summary>
        private void AddCombustionEffects()
        {
            controller.MoveSpeed *= stats.combustionMoveSpeedMultiplier;
            combustionDamageCoroutine = CombustionDamageCoroutine(
                stats.combustionIntervalDamage,
                stats.combustionIntervalSeconds);
            StartCoroutine(combustionDamageCoroutine);
            flamesParticleEffect = Instantiate(
                GameAssets.Instance.FlamesParticleEffect,
                transform.position,
                Quaternion.Euler(0, 0, 0));
            flamesParticleEffect.SetParent(gameObject.transform);
        }

        private IEnumerator CombustionDamageCoroutine(int damage, float intervalSeconds)
        {
            float startTime = Time.time;
            while (isCombusting && Time.time - startTime < stats.combustionDurationSeconds)
            {
                health.HurtIgnoreInvulnerability(damage);
                yield return new WaitForSeconds(intervalSeconds);
            }
            this.IsCombusting = false;
        }

        /// <summary>
        /// Called in IsCombusting setter.
        /// </summary>
        private void RemoveCombustionEffects()
        {
            controller.MoveSpeed /= 1.5f;
            StopCoroutine(combustionDamageCoroutine);
            Destroy(flamesParticleEffect.gameObject);
        }

        /// <summary>
        /// Called in OnCollisionEnter.
        /// </summary>
        private void SpreadFire(Collision other)
        {
            if (isCombusting)
            {
                EntityStats es = other.gameObject.GetComponent<EntityStats>();
                if (es != null) { es.IsCombusting = true; }
            }
        }
        #endregion
    }
}
