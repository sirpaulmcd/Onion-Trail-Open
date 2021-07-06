using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Base state for chasing.
    /// </summary>
    public class ChaseState : State
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected ChaseStateSO stateData;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public ChaseState(FSMNavMeshController fsmNavMeshController, ChaseStateSO stateData) : base(fsmNavMeshController)
        {
            this.stateData = stateData;
            animatorBoolName = "chase";
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
            fsmNavMeshController.NavMeshAgent.speed = stateData.chaseSpeed;
            fsmNavMeshController.TargetTransform = fsmNavMeshController.GetClosestPlayer().GameObject.transform;
            Debug.Log("Entering Chase State");
        }
        #endregion

        //=====================================================================
        #region Player location
        //=====================================================================
        private void UpdateTarget()
        {
            if (!fsmNavMeshController.EntityStats.IsKnockedBack)
            {
                GameObject closestPlayerObject = fsmNavMeshController.GetClosestPlayer().GameObject;
                if (fsmNavMeshController.TargetTransform != closestPlayerObject.transform)
                {
                    fsmNavMeshController.TargetTransform = closestPlayerObject.transform;
                }
                fsmNavMeshController.NavMeshAgent.SetDestination(fsmNavMeshController.TargetTransform.position);
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrappers
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            UpdateTarget();
        }
        #endregion
    }
}
