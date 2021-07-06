using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Patrol state for base enemy.
    /// </summary>
    public class BaseEnemyPatrolState : PatrolState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseEnemyController baseEnemyController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseEnemyPatrolState(BaseEnemyController baseEnemyController, PatrolStateSO stateData) : base(baseEnemyController, stateData)
        {
            this.baseEnemyController = baseEnemyController;
        }
        #endregion

        //=====================================================================
        #region State transition
        //=====================================================================
        private void CheckStateTransitionConditions()
        {
            if (baseEnemyController.IsPlayerInMinAgroRange())
            {
                finiteStateMachine.ChangeState(baseEnemyController.PlayerDetectedState);
            }
            if (isAtPatrolPoint)
            {
                finiteStateMachine.ChangeState(baseEnemyController.IdleState);
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrappers
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckStateTransitionConditions();
        }
        #endregion
    }
}
