using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// The controller for the base mob.
    /// </summary>
    public class BaseMobController : FSMNavMeshController
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private IdleStateSO _idleStateData = default;
        [SerializeField] private ChaseStateSO _chaseStateData = default;
        [SerializeField] private AttackStateSO _attackStateData = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The idle state of the entity.
        /// </summary>
        public BaseMobIdleState IdleState { get; private set; }
        /// <summary>
        /// The chase state of the entity.
        /// </summary>
        public BaseMobChaseState ChaseState { get; private set; }
        /// <summary>
        /// The attack state of the entity.
        /// </summary>
        public BaseMobAttackState AttackState { get; private set; }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        protected override void Start()
        {
            base.Start();
            InitVars();
        }

        protected void OnDrawGizmos()
        {
            DrawGizmos();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private new void InitVars()
        {
            IdleState = new BaseMobIdleState(this, _idleStateData);
            ChaseState = new BaseMobChaseState(this, _chaseStateData);
            AttackState = new BaseMobAttackState(this, _attackStateData);
            FiniteStateMachine.Initialize(ChaseState);
        }
        #endregion

        //=====================================================================
        #region Gizmos
        //=====================================================================
        /// <summary>
        /// Draws gizmos for the detection ranges.
        /// </summary>
        private void DrawGizmos()
        {
            try
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(
                    transform.position + new Vector3(_attackStateData.range, 0, 0),
                    _attackStateData.radiusOfAttack);
            }
            catch (System.Exception)
            {
                Debug.Log(name + " does not have data to draw gizmos");
            }
        }
        #endregion

        //=====================================================================
        #region Player range detection
        //=====================================================================
        /// <summary>
        /// Checks whether a player is within the min agro range.
        /// </summary>
        public virtual bool IsPlayerInAgroRange()
        {
            Player player = GetClosestPlayer();
            if (player == null)
                return false;
            return Vector3.SqrMagnitude(
                transform.position - player.GameObject.transform.position) <
                999999999;
        }

        /// <summary>
        /// Checks whether a player is within attack range.
        /// </summary>
        public virtual bool IsPlayerInAttackRange()
        {
            Player player = GetClosestPlayer();
            if (player == null)
                return false;
            return Vector3.SqrMagnitude(
                transform.position - player.GameObject.transform.position) <=
                _attackStateData.range * _attackStateData.range;
        }
        #endregion

        //=====================================================================
        #region Attacking action
        //=====================================================================
        /// <summary>
        /// To be called if the when the attack state is entered.
        /// </summary>
        public void StartAttacking()
        {
            InvokeRepeating("Attack", 0, 1f);
        }

        /// <summary>
        /// To be called if the when the attack state is entered.
        /// </summary>
        public void StopAttacking()
        {
            CancelInvoke();
        }

        /// <summary>
        /// Hurts player if detected by overlap sphere.
        /// </summary>
        public void Attack()
        {
            Debug.Log("Attack triggered");
            Vector3 directionOfPlayer = GetPlayerDirection();
            WeaponSelector.UseSelectedWeapon(directionOfPlayer);
        }

        protected Vector3 GetPlayerDirection()
        {
            return (GetClosestPlayer().GameObject.transform.position - transform.position).normalized;
        }
        #endregion
    }
}
