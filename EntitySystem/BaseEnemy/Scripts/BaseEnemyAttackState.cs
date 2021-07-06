using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Attack state for base enemy.
    /// </summary>
    public class BaseEnemyAttackState : AttackState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseEnemyController baseEnemyController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseEnemyAttackState(BaseEnemyController baseEnemyController, AttackStateSO stateData) : base(baseEnemyController, stateData)
        {
            this.baseEnemyController = baseEnemyController;
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
            baseEnemyController.StartAttacking();
        }

        /// <summary>
        /// To be called when the state is being exited.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            baseEnemyController.StopAttacking();
        }

        private void CheckStateTransitionConditions()
        {
            if (!baseEnemyController.IsPlayerInMaxAgroRange())
            {
                finiteStateMachine.ChangeState(baseEnemyController.IdleState);
            }
            else if (!baseEnemyController.IsPlayerInAttackRange())
            {
                finiteStateMachine.ChangeState(baseEnemyController.ChaseState);
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
