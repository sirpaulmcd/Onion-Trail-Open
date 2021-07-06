using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AI;

namespace EGS
{
    /// <summary>
    /// Controller for entities that utilize a finite state machine and the nav
    /// mesh agent.
    /// </summary>
    public class FSMNavMeshController : FSMController
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The NavMeshAgent of the entity.
        /// </summary>
        public NavMeshAgent NavMeshAgent { get; private set; }
        /// <summary>
        /// The transform of the target of the entity.
        /// </summary>
        public Transform TargetTransform { get; set; }
        /// <summary>
        /// The angular direction (degrees on the x/z plane) that the entity is
        /// facing. Used by the animator. The directions are as follows:
        /// - Up: 0
        /// - Left: -90
        /// - Right: 90
        /// - Down: +-180
        /// </summary>
        public float FacingDirection { get; set; }
        /// <summary>
        /// The WeaponSelector of the entity.
        /// </summary>
        public WeaponSelector WeaponSelector { get; private set; }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        protected override void Start()
        {
            base.Start();
            InitVars();
        }

        protected override void Update()
        {
            base.Update();
            UpdateAnimatorVariables();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private new void InitVars()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            WeaponSelector = GetComponentInChildren<WeaponSelector>();
        }
        #endregion

        //=====================================================================
        #region Animation
        //=====================================================================
        private void UpdateFacingDirection()
        {
            if (TargetTransform != null)
            {
                Vector3 vectorToTarget = TargetTransform.position - transform.position;
                FacingDirection = Mathf.Atan2(vectorToTarget.x, vectorToTarget.z) * Mathf.Rad2Deg;
            }
        }

        private void UpdateAnimatorVariables()
        {
            UpdateFacingDirection();
            Animator.SetFloat("facingDirection", FacingDirection);
        }
        #endregion

        //=====================================================================
        #region Player location
        //=====================================================================
        /// <summary>
        /// Finds the closest player to the entity.
        /// </summary>
        /// <returns>
        /// The Player object of the closest player to the entity.
        /// </returns>
        public virtual Player GetClosestPlayer()
        {
            Player closestPlayer = null;
            float minDistance = float.MaxValue;
            foreach (Player player in PlayerManager.Instance.Players)
            {
                if (player != null)
                {
                    float distance = Vector3.Distance(player.GameObject.transform.position, transform.position);
                    if (distance < minDistance) { closestPlayer = player; minDistance = distance; }
                }
            }
            return closestPlayer;
        }
        #endregion
    }
}
