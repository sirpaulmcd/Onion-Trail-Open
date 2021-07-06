using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Attack state for base mob.
    /// </summary>
    public class BaseMobAttackState : AttackState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseMobController baseMobController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseMobAttackState(BaseMobController baseMobController, AttackStateSO stateData) : base(baseMobController, stateData)
        {
            this.baseMobController = baseMobController;
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
            baseMobController.StartAttacking();
        }

        /// <summary>
        /// To be called when the state is being exited.
        /// </summary>
        public override void Exit()
        {
            base.Exit();
            baseMobController.StopAttacking();
        }

        private void CheckStateTransitionConditions()
        {
            if (!baseMobController.IsPlayerInAgroRange())
            {
                finiteStateMachine.ChangeState(baseMobController.IdleState);
            }
            else if (!baseMobController.IsPlayerInAttackRange())
            {
                finiteStateMachine.ChangeState(baseMobController.ChaseState);
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
