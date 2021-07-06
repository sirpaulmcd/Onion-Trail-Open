using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instances of WGOs that are dynamically spawned, move through space, and
    /// interact with GameObjects that touch their colliders. DWGOs are self
    /// regulating projectiles. After being spawned into a scene and initialized by
    /// their corresponding weapon, they are responsible for themselves and self
    /// destruct after a designated amount of time unless otherwise specified. It
    /// is important to note that all DWGOs (i.e. projectiles) should be placed on
    /// the “Ignore Raycast” layer to avoid complications. Additionally, the
    /// ATriggerDWGO and AColliderDWGO abstract classes may appear to share much of
    /// the same code. This is necessary because ATriggerDWGO uses OnTriggerEnter()
    /// (yields a Collider argument) whereas AColliderDWGO uses OnCollisionEnter()
    /// (yields a Collision argument). Since the OnXEnter() methods yield different
    /// collision related arguments, the code cannot be shared.
    /// </summary>
    public abstract class ADynamicWGO : AWeaponizedGameObject, IDynamicWGO
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The GameObject that the projectile has most recently interacted with.
        /// This ensures that GameObjects with 2 colliders don't get hit twice.
        /// </summary>
        protected GameObject prevGameObject;
        /// <summary>
        /// The Rigidbody of the projectile. The new keyword is used to hide an
        /// outdated Unity property.
        /// </summary>
        new protected Rigidbody rigidbody;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IDynamicGameObject properties")]
        /// <summary>
        /// The speed of the DWGO.
        /// </summary>
        public float MoveSpeed { get; set; }
        /// <summary>
        /// The number of seconds before an instantiated DWGO is to self destruct.
        /// Necessary if the DWGO never hits anything.
        /// </summary>
        public float SelfDestructSeconds { get; set; }
        /// <summary>
        /// The distance from the attacker transform that the DWGO should be
        /// spawned.
        /// </summary>
        public float SpawnOffsetDistance { get; set; }
        #endregion

        //=====================================================================
        #region MonoBehavior
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
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initializes the DWGO variables, sets the appropraite rotation,
        /// launches the GameObject as desired, starts the self destruct countdown.
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
        public virtual void Init(IDynamicWGO weapon, GameObject attacker, Vector3 direction)
        {
            InitVars(weapon, attacker);
            transform.eulerAngles = new Vector3(0, GetAngleFromVectorFloat(direction), 0);
            StartCoroutine(StartSelfDestructCountdown());
            // Fill in: Move object in some way...
        }
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        protected virtual void InitOnStart()
        {
            CheckLayer();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        protected virtual void InitVars(IDynamicWGO weapon, GameObject attacker)
        {
            // Initialize IWarAndPeace variables
            this.Attacker = attacker;
            this.CriticalHitMultiplier = weapon.CriticalHitMultiplier;
            this.CriticalHitChance = weapon.CriticalHitChance;
            this.IsFriendlyFire = weapon.IsFriendlyFire;
            this.IsHeal = weapon.IsHeal;
            this.KnockbackMagnitude = weapon.KnockbackMagnitude;
            this.KnockbackableMagnitude = weapon.KnockbackableMagnitude;
            this.KnockbackOrigin = this.gameObject;
            this.MaximumDamage = weapon.MaximumDamage;
            this.MinimumDamage = weapon.MinimumDamage;
            this.Title = weapon.Title;
            // Initialize IDynamicGameObject variables
            this.MoveSpeed = weapon.MoveSpeed;
            this.SelfDestructSeconds = weapon.SelfDestructSeconds;
            this.SpawnOffsetDistance = weapon.SpawnOffsetDistance;
            // Initialize ADynamicGameObject variables
            rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Converts a Vector3 into an angle on the xz-plane. Used to get the
        /// proper rotation of the WeaponizedGameObject given its movement
        /// direction.
        /// </summary>
        /// <param name="direction">
        /// The direction that the WeaponizedGameObject is to be launched.
        /// </param>
        /// <returns>
        /// The angle (in degrees) in the xz-plane that the Vector is pointing.
        /// Z+ = 0 degrees
        /// X+ = 90 degrees
        /// Z- = 180 degrees
        /// X- = 270 degrees
        /// </returns>
        protected float GetAngleFromVectorFloat(Vector3 direction)
        {
            direction = direction.normalized;
            float n = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (n < 0) { n += 360; }
            return n;
        }

        /// <summary>
        /// Waits SelfDestructSeconds and destroys the object.
        /// </summary>
        protected virtual IEnumerator StartSelfDestructCountdown()
        {
            yield return new WaitForSeconds(SelfDestructSeconds);
            Destroy(gameObject);
        }

        /// <summary>
        /// Checks the layer belonging to the component's GameObject. If it is not
        /// on the 'Ignore Raycasting' layer, prints warning message.
        /// </summary>
        protected virtual void CheckLayer()
        {
            if (this.gameObject.layer != 2)
            {
                Debug.LogWarning("Weaponized GameObject does not belong to the " +
                    "'Ignore Raycasting' layer.", this.gameObject);
            }
        }
        #endregion
    }
}
