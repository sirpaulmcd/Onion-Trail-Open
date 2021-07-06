// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class represents entities that participate in the combat system. As
    /// such, they are susceptible to combat effects such as knockback and
    /// explosive forces.
    /// </summary>
    public class AEntityController : MonoBehaviour, IEntityController
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The Rigidbody component of the entity.
        /// </summary>
        protected new Rigidbody rigidbody = default;

        //=====================================================================
        // [Header("IEntityController variables")]
        /// <summary>
        /// Whether or not movement of the controlled GameObject is allowed.
        /// </summary>
        protected bool freezeMovement = false;
        /// <summary>
        /// The move speed of the controlled GameObject.
        /// </summary>
        [SerializeField] protected float moveSpeed = 350f;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IEntityController properties")]
        /// <summary>
        /// Whether or not movement of the controlled GameObject is allowed. To be
        /// overwritten by children.
        /// </summary>
        public virtual bool FreezeMovement
        {
            get => freezeMovement;
            set => freezeMovement = value;
        }

        /// <summary>
        /// The move speed of the controlled GameObject.
        /// </summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = value;
        }

        /// <summary>
        /// The EntityStats of the controller. Used to pass status effect info to
        /// the controller or state machine.
        /// </summary>
        public EntityStats EntityStats { get; private set; }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        protected virtual void Start()
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
            this.EntityStats = GetComponent<EntityStats>();
            rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        protected virtual void CheckMandatoryComponents()
        {
            Assert.IsNotNull(rigidbody, gameObject.name + " is missing rigidbody");
            Assert.IsNotNull(this.EntityStats, gameObject.name + " is missing EntityStats");
        }
        #endregion
    }
}
