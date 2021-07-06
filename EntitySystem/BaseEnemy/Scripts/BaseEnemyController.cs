using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// The controller for the base enemy.
    /// </summary>
    public class BaseEnemyController : FSMNavMeshController
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private IdleStateSO _idleStateData = default;
        [SerializeField] private PatrolStateSO _patrolStateData = default;
        [SerializeField] private PlayerDetectedStateSO _playerDetectedStateData = default;
        [SerializeField] private ChaseStateSO _chaseStateData = default;
        [SerializeField] private AttackStateSO _attackStateData = default;
        [SerializeField] private Transform[] _patrolPoints = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The idle state of the entity.
        /// </summary>
        public BaseEnemyIdleState IdleState { get; private set; }
        /// <summary>
        /// The patrol state of the entity.
        /// </summary>
        public BaseEnemyPatrolState PatrolState { get; private set; }
        /// <summary>
        /// The player detected state of the entity.
        /// </summary>
        public BaseEnemyPlayerDetectedState PlayerDetectedState { get; private set; }
        /// <summary>
        /// The chase state of the entity.
        /// </summary>
        public BaseEnemyChaseState ChaseState { get; private set; }
        /// <summary>
        /// The attack state of the entity.
        /// </summary>
        public BaseEnemyAttackState AttackState { get; private set; }
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
            _patrolStateData.patrolPoints = _patrolPoints;
            PatrolState = new BaseEnemyPatrolState(this, _patrolStateData);
            IdleState = new BaseEnemyIdleState(this, _idleStateData);
            ChaseState = new BaseEnemyChaseState(this, _chaseStateData);
            PlayerDetectedState = new BaseEnemyPlayerDetectedState(this, _playerDetectedStateData);
            AttackState = new BaseEnemyAttackState(this, _attackStateData);
            FiniteStateMachine.Initialize(PatrolState);
        }
        #endregion

        //=====================================================================
        #region Gizmos
        //=====================================================================
        /// <summary>
        /// Draws gizmos for the patrol points and various detection ranges.
        /// </summary>
        private void DrawGizmos()
        {
            try
            {
                foreach (Transform patrolPoint in _patrolPoints)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(patrolPoint.position, 0.5f);
                }
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(
                    transform.position,
                    _playerDetectedStateData.minimumAgroDistance);

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(
                    transform.position,
                    _playerDetectedStateData.maximumAgroDistance);

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
        public virtual bool IsPlayerInMinAgroRange()
        {
            Player player = GetClosestPlayer();
            if (player == null)
                return false;
            return Vector3.SqrMagnitude(
                transform.position - player.GameObject.transform.position) <
                _playerDetectedStateData.minimumAgroDistance *
                _playerDetectedStateData.minimumAgroDistance;
        }

        /// <summary>
        /// Checks whether a player is within the max agro range.
        /// </summary>
        public virtual bool IsPlayerInMaxAgroRange()
        {
            Player player = GetClosestPlayer();
            if (player == null)
                return false;
            return Vector3.SqrMagnitude(
                transform.position - player.GameObject.transform.position) <
                _playerDetectedStateData.maximumAgroDistance *
                _playerDetectedStateData.maximumAgroDistance;
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
