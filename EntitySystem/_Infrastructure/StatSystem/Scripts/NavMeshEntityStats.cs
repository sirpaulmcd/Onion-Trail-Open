using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This script manages stats and status effects for entities using a nav mesh
    /// agent.
    /// </summary>
    public class NavMeshEntityStats : EntityStats
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The controller component of the entity.
        /// </summary>
        protected new FSMNavMeshController controller = default;
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
            controller = GetComponent<FSMNavMeshController>();
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(controller, gameObject.name + " is missing controller");
        }
        #endregion

        //=====================================================================
        #region Knockback
        //=====================================================================
        /// <summary>
        /// Adds knockback effects to the NavMeshAgent.
        /// </summary>
        protected override void AddKnockbackEffects()
        {
            controller.NavMeshAgent.enabled = false;
            rigidbody.isKinematic = false;
        }

        /// <summary>
        /// Removes knockback effects from the NavMeshAgent. A null check is
        /// required because the GameObject may have been killed before reaching
        /// their knockback destination.
        /// </summary>
        protected override void RemoveKnockbackEffects()
        {
            controller.NavMeshAgent.enabled = true;
            rigidbody.isKinematic = true;
            knockbackDirection = default;
            knockbackMagnitude = default;
        }
        #endregion
    }
}
