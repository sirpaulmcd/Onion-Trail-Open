using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Base state for patrolling.
    /// </summary>
    public class PatrolState : State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected PatrolStateSO stateData;
        protected bool isAtPatrolPoint;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public PatrolState(FSMNavMeshController fsmNavMeshController, PatrolStateSO stateData) : base(fsmNavMeshController)
        {
            this.stateData = stateData;
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
            animatorBoolName = "patrol";
            fsmNavMeshController.TargetTransform = stateData.patrolPoints[stateData.currentPatrolPointIndex];
            fsmNavMeshController.NavMeshAgent.SetDestination(fsmNavMeshController.TargetTransform.position);
            fsmNavMeshController.NavMeshAgent.speed = stateData.patrolSpeed;
            isAtPatrolPoint = false;
            Debug.Log("Entering Patrol State");
        }

        /// <summary>
        /// To be called when the state is being exited.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            stateData.currentPatrolPointIndex = stateData.currentPatrolPointIndex == stateData.patrolPoints.Length - 1 ? 0 : stateData.currentPatrolPointIndex + 1;
        }
        #endregion

        //=====================================================================
        #region Patrol action
        //=====================================================================
        private void CheckPatrolProgress()
        {
            if (fsmNavMeshController.NavMeshAgent.remainingDistance < 0.3f) { isAtPatrolPoint = true; }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrappers
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckPatrolProgress();
        }
        #endregion
    }
}
