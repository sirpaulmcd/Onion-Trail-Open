using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

namespace EGS
{
    /// <summary>
    /// Controller for entities that utilize a finite state machine.
    /// </summary>
    public class FSMController : AEntityController
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The Animator of the entity.
        /// </summary>
        public Animator Animator { get; private set; }
        /// <summary>
        /// The FiniteStateMachine of the entity.
        /// </summary>
        public FiniteStateMachine FiniteStateMachine { get; private set; }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        protected override void Start()
        {
            base.Start();
            InitVars();
        }

        protected virtual void Update()
        {
            FiniteStateMachine.CurrentState.LogicUpdate();
        }

        protected virtual void FixedUpdate()
        {
            FiniteStateMachine.CurrentState.PhysicsUpdate();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private new void InitVars()
        {
            Animator = GetComponent<Animator>();
            FiniteStateMachine = new FiniteStateMachine();
        }
        #endregion
    }
}
