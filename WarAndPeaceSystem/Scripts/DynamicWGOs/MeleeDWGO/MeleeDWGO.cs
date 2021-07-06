using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instance Trigger DWGO Weapons that are further specialized to pierce
    /// through a specific number of enemies before self destructing. Melee weapons
    /// involve swinging a Trigger DWGO radially around the attacker. Futher
    /// specializations of this weapon can yield more complex swinging animations
    /// and projectile skins.
    /// </summary>
    public class MeleeDWGO : APiercingDWGO, IMeleeDWGO
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IMeleeObject properties")]
        /// <summary>
        /// The offset in front of the player in which the weapon spawns. Offset
        /// ensures swing arc is centered on facing direction rather than starting
        /// in front of the player. Note that the MeleeObject may not appear to
        /// actually spawn exactly at the angle if it is moving quickly.
        /// </summary>
        public float SwingOffsetAngle { get; set; }
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            RotateAroundAttacker();
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initializes the MeleeObject variables, sets the appropraite GameObject
        /// rotation, swings the object radially around the attacker, and starts
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
        public void Init(IMeleeDWGO weapon, GameObject attacker, Vector3 direction)
        {
            InitVars(weapon, attacker);
            InitializeSwing(direction);
            StartCoroutine(StartSelfDestructCountdown());
        }
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        protected void InitVars(IMeleeDWGO weapon, GameObject attacker)
        {
            base.InitVars(weapon, attacker);
            // Initialise IMeleeObject variables
            this.KnockbackOrigin = this.Attacker;
            this.SwingOffsetAngle = weapon.SwingOffsetAngle;
        }

        /// <summary>
        /// Rotates weaponized object (if any) radially around the y-axis of the
        /// attacker.
        /// </summary>
        private void RotateAroundAttacker()
        {
            this.transform.RotateAround(this.Attacker.transform.position, Vector3.up, this.MoveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Initializes the positioning of the swinging object. The actual swinging
        /// is done by RotateAroundAttacker(). The input direction is rotated
        /// counterclockwise by SwingOffsetAngle. The rotation of the object is
        /// then changed to reflect this rotation. The objects transform is moved
        /// the new location and gravity is toggled off.
        /// </summary>
        /// <param name="direction">
        /// The direction that the object will swing.
        /// </param>
        protected virtual void InitializeSwing(Vector3 direction)
        {
            direction = Quaternion.Euler(0, -this.SwingOffsetAngle, 0) * direction;
            transform.eulerAngles = new Vector3(0, GetAngleFromVectorFloat(direction), 0);
            this.transform.position = this.Attacker.transform.position + direction.normalized * this.SpawnOffsetDistance;
            this.rigidbody.useGravity = false;
        }
        #endregion
    }
}
