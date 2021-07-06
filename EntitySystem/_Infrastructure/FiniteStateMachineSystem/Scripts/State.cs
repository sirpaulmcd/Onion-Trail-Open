using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// A state for the finite state machine.
    /// </summary>
    public class State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The FiniteStateMachine of the state.
        /// </summary>
        protected FiniteStateMachine finiteStateMachine;
        /// <summary>
        /// The FSMNavMeshController of the state.
        /// </summary>
        protected FSMNavMeshController fsmNavMeshController;
        /// <summary>
        /// The start time of the state for measuring state duration.
        /// </summary>
        protected float startTime;
        /// <summary>
        /// The animator boolean name of the state. Used to trigger an animation
        /// based on the current state.
        /// </summary>
        protected string animatorBoolName = "default";
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        /// <summary>
        /// Constructs a State object.
        /// </summary>
        /// <param name="fsmNavMeshController">The controller of the state.</param>
        public State(FSMNavMeshController fsmNavMeshController)
        {
            this.fsmNavMeshController = fsmNavMeshController;
            this.finiteStateMachine = fsmNavMeshController.FiniteStateMachine;
        }
        #endregion

        //=====================================================================
        #region State transitions
        //=====================================================================
        /// <summary>
        /// To be called when the state is being entered.
        /// </summary>
        public virtual void Enter()
        {
            startTime = Time.time;
            fsmNavMeshController.Animator.SetBool(animatorBoolName, true);
        }

        /// <summary>
        /// To be called when the state is being exited.
        /// </summary>
        public virtual void Exit()
        {
            fsmNavMeshController.Animator.SetBool(animatorBoolName, false);
        }
        #endregion

        //=====================================================================
        #region MoboBehaviour wrappers
        //=====================================================================
        public virtual void PhysicsUpdate()
        {

        }

        public virtual void LogicUpdate()
        {

        }
        #endregion
    }
}
