using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Bases state for idling.
    /// </summary>
    public class IdleState : State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected IdleStateSO stateData;
        protected float idleTime;
        protected bool isIdleTimeOver;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public IdleState(FSMNavMeshController fsmNavMeshController, IdleStateSO stateData) : base(fsmNavMeshController)
        {
            this.stateData = stateData;
            animatorBoolName = "idle";
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
            fsmNavMeshController.NavMeshAgent.ResetPath();
            fsmNavMeshController.NavMeshAgent.speed = 0f;
            isIdleTimeOver = false;
            SetRandomIdleTime();
            Debug.Log("Entering Idle State");
        }

        private void SetRandomIdleTime()
        {
            idleTime = UnityEngine.Random.Range(stateData.minIdleTime, stateData.maxIdleTime);
        }
        #endregion

        //=====================================================================
        #region Idle action
        //=====================================================================
        private void CheckIdleTime()
        {
            if (Time.time >= startTime + idleTime) { isIdleTimeOver = true; }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrappers
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckIdleTime();
        }
        #endregion
    }
}
