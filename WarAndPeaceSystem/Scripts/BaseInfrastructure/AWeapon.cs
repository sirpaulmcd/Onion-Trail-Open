using System; // For math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instances of WGOs that are acitvated by an entity such as a player or enemy
    /// (i.e. have Attack() and Cooldown() methods).
    /// </summary>
    public abstract class AWeapon : AWeaponizedGameObject
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("AWeapon variables")]
        /// <summary>
        /// The minimum number of seconds between weapon uses.
        /// </summary>
        [SerializeField] protected float cooldownSeconds = 0.2f;
        /// <summary>
        /// Boolean indicating whether weapon is currently on cooldown.
        /// </summary>
        protected bool onCooldown = false;
        /// <summary>
        /// True if weapon attacks once per button press.
        /// False if weapon attacks multiple times per button press.
        /// </summary>
        [SerializeField] protected bool singleFire = true;
        /// <summary>
        /// The displayed image depicting the weapon.
        /// </summary>
        public Sprite UIElement;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("AWeapon properties")]
        /// <summary>
        /// The minimum number of seconds between weapon uses.
        /// </summary>
        public float CooldownSeconds
        {
            get => cooldownSeconds;
            set => cooldownSeconds = value;
        }
        /// <summary>
        /// Boolean indicating whether weapon is currently on cooldown.
        /// </summary>
        public bool OnCooldown
        {
            get => onCooldown;
            set => onCooldown = value;
        }
        /// <summary>
        /// True if weapon attacks once per button press.
        /// False if weapon attacks multiple times per button press.
        /// </summary>
        public bool SingleFire
        {
            get => singleFire;
            set => singleFire = value;
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
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Activates the main attack of the weapon.
        /// </summary>
        /// <param name="direction">
        /// The direction that the attack is to be made.
        /// </param>
        public abstract void Attack(Vector3 direction);
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        protected virtual void InitOnStart()
        {
            InitVars();
            CheckMandatoryComponents();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        protected virtual void InitVars()
        {
            SetAttackerGameObject();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        protected virtual void CheckMandatoryComponents()
        {
            Assert.IsNotNull(attacker, gameObject.name + " is missing attacker");
        }

        /// <summary>
        /// Sets the Attacker to the GameObject belonging to the user of the
        /// weapon. The user should be the grandparent of the weapon
        /// (i.e. User > WeaponHolder > Weapon).
        /// </summary>
        protected virtual void SetAttackerGameObject()
        {
            if (transform.parent.parent != null)
            {
                this.Attacker = transform.parent.parent.gameObject;
            }
            else { Debug.LogWarning("Attacker not found!", this.gameObject); }
        }

        /// <summary>
        /// Toggles OnCooldown boolean for CooldownSeconds.
        /// </summary>
        protected virtual IEnumerator Cooldown()
        {
            OnCooldown = true;
            yield return new WaitForSeconds(CooldownSeconds);
            OnCooldown = false;
        }
        #endregion
    }
}
