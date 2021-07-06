using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Idle state for base enemy.
    /// </summary>
    public class BaseEnemyIdleState : IdleState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseEnemyController baseEnemyController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseEnemyIdleState(BaseEnemyController baseEnemyController, IdleStateSO stateData) : base(baseEnemyController, stateData)
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
            else if (isIdleTimeOver)
            {
                finiteStateMachine.ChangeState(baseEnemyController.PatrolState);
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
