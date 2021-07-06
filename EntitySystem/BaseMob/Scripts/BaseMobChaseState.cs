using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Chase state for base mob.
    /// </summary>
    public class BaseMobChaseState : ChaseState
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        protected BaseMobController baseMobController;
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        public BaseMobChaseState(BaseMobController baseMobController, ChaseStateSO stateData) : base(baseMobController, stateData)
        {
            this.baseMobController = baseMobController;
        }
        #endregion

        //=====================================================================
        #region State transition
        //=====================================================================
        private void CheckStateTransitionConditions()
        {
            if (!baseMobController.IsPlayerInAgroRange())
            {
                finiteStateMachine.ChangeState(baseMobController.IdleState);
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
