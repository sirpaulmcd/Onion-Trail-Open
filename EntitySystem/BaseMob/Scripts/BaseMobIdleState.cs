using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Idle state for base enemy.
    /// </summary>
    public class BaseMobIdleState : IdleState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseMobController baseMobController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseMobIdleState(BaseMobController baseMobController, IdleStateSO stateData) : base(baseMobController, stateData)
        {
            this.baseMobController = baseMobController;
        }
        #endregion

        //=====================================================================
        #region State transition
        //=====================================================================
        private void CheckStateTransitionConditions()
        {
            if (baseMobController.IsPlayerInAgroRange())
            {
                finiteStateMachine.ChangeState(baseMobController.ChaseState);
            }
            else if (baseMobController.IsPlayerInAttackRange())
            {
                finiteStateMachine.ChangeState(baseMobController.AttackState);
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
