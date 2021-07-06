using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Base state for attacking.
    /// </summary>
    public class AttackState : State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected AttackStateSO stateData;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public AttackState(FSMNavMeshController fsmNavMeshController, AttackStateSO stateData) : base(fsmNavMeshController)
        {
            this.stateData = stateData;
            animatorBoolName = "attack";
        }
        #endregion

        //=====================================================================
        #region State transition
        //=====================================================================
        /// <summary>
        /// To be called when the state is being entered.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            fsmNavMeshController.NavMeshAgent.speed = 0.000f;
            Debug.Log("Entering Attack State");
        }
        #endregion
    }
}
