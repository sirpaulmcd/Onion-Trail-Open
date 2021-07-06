using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Base state for detection.
    /// </summary>
    public class PlayerDetectedState : State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        public PlayerDetectedStateSO stateData;
        protected bool performAction = false;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public PlayerDetectedState(FSMNavMeshController fsmNavMeshController, PlayerDetectedStateSO stateData) : base(fsmNavMeshController)
        {
            this.stateData = stateData;
            animatorBoolName = "playerDetected";
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
            performAction = false;
            fsmNavMeshController.NavMeshAgent.speed = 0.00f;
            fsmNavMeshController.TargetTransform = fsmNavMeshController.GetClosestPlayer().GameObject.transform;
            Debug.Log("Entering Detected State");
        }
        #endregion

        //=====================================================================
        #region Detection action
        //=====================================================================
        private void CheckActionTime()
        {
            if (Time.time >= startTime + stateData.actionTime && !performAction)
            {
                performAction = true;
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrappers
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckActionTime();
        }
        #endregion
    }
}
