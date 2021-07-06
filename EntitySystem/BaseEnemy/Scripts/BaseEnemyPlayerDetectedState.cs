using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Player detected state for base enemy.
    /// </summary>
    public class BaseEnemyPlayerDetectedState : PlayerDetectedState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseEnemyController baseEnemyController;
        #endregion

        //=====================================================================
        #region Constructor
        //=====================================================================
        public BaseEnemyPlayerDetectedState(BaseEnemyController baseEnemyController, PlayerDetectedStateSO stateData) : base(baseEnemyController, stateData)
        {
            this.baseEnemyController = baseEnemyController;
        }
        #endregion

        //=====================================================================
        #region State transition
        //=====================================================================
        private void CheckStateTransitionConditions()
        {
            if (!baseEnemyController.IsPlayerInMaxAgroRange())
            {
                finiteStateMachine.ChangeState(baseEnemyController.IdleState);
            }
            else if (performAction)
            {
                finiteStateMachine.ChangeState(baseEnemyController.ChaseState);
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour wrapper
        //=====================================================================
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            CheckStateTransitionConditions();
        }
        #endregion
    }
}
