using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Chase state for base enemy.
    /// </summary>
    public class BaseEnemyChaseState : ChaseState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseEnemyController baseEnemyController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseEnemyChaseState(BaseEnemyController baseEnemyController, ChaseStateSO stateData) : base(baseEnemyController, stateData)
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
            else if (baseEnemyController.IsPlayerInAttackRange())
            {
                finiteStateMachine.ChangeState(baseEnemyController.AttackState);
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
