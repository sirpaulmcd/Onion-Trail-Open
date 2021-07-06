using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instance Trigger DWGO Weapons that are further specialized to pierce
    /// through a specific number of enemies before self destructing. In addition
    /// to piercing capabilities, bullets have also been specialized to use
    /// raycasting to elastically reflect off of walls. As such, bullets self
    /// destruct after either running down the piercing count or the ricochet
    /// count.
    /// </summary>
    public class BulletDWGO : APiercingDWGO, IBulletDWGO
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The information associated with the latest raycast. Used for reflection
        /// purposes.
        /// </summary>
        protected RaycastHit hitInfo;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The LayerMask that the raycast can detect. To set the LayerMask to
        /// detect everything, initialize as -1. In order for DWGO to work, the
        /// 'Ignore Raycast' layermask must be deselected through the inspector.
        /// </summary>
        public LayerMask LayerMask { get; set; }
        /// <summary>
        /// The maximum distance that the DWGO raycast will travel.
        /// </summary>
        public float MaxRaycastDistance { get; set; }
        /// <summary>
        /// The seconds between raycasts used to collect reflection information.
        /// </summary>
        public float RaycastRefreshSeconds { get; set; }
        /// <summary>
        /// The number of times the DWGO can ricochet off of objects in the
        /// environment.
        /// </summary>
        public int RicochetCount { get; set; }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initializes the DWGO variables, sets the appropraite rotation, launches
        /// the GameObject in its "forward" direction, starts the self destruct
        /// countdown. Starts raycasting if the DWGO is set to richochet.
        /// </summary>
        /// <param name="weapon">
        /// The weapon script that fired this GameObject.
        /// </param>
        /// <param name="attacker">
        /// The GameObject initiating the attack/heal.
        /// </param>
        /// <param name="direction">
        /// The direction that the attack is to be made.
        /// </param>
        public void Init(IBulletDWGO weapon, GameObject attacker, Vector3 direction)
        {
            InitVars(weapon, attacker);
            transform.eulerAngles = new Vector3(0, base.GetAngleFromVectorFloat(direction), 0);
            StartCoroutine(base.StartSelfDestructCountdown());
            LaunchForward();
            if (this.RicochetCount > 0)
            {
                InvokeRepeating("UpdateReflectionInfo", 0f, this.RaycastRefreshSeconds);
            }
        }
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        /// <param name="weapon">
        /// The weapon script that fired this GameObject.
        /// </param>
        /// <param name="attacker">
        /// The GameObject initiating the attack/heal.
        /// </param>
        protected void InitVars(IBulletDWGO weapon, GameObject attacker)
        {
            base.InitVars(weapon, attacker);
            // Initialize weapon-housed projectile variables
            this.LayerMask = weapon.LayerMask;
            this.MaxRaycastDistance = weapon.MaxRaycastDistance;
            this.RaycastRefreshSeconds = weapon.RaycastRefreshSeconds;
            this.RicochetCount = weapon.RicochetCount;
        }

        /// <summary>
        /// Processes the collision between colliders. Ignores certain collisions.
        /// Deals health effects to anything that has an IHealth component.
        /// Reflects off of anything that doesn't. Applies knockback effects.
        /// Destorys the object if it is on its last collision.
        /// </summary>
        /// <param name="collider">
        /// The collider that is interacting with the projectile trigger.
        /// </param>
        protected override void ProcessColliderInteraction(Collider collider)
        {
            if (base.IgnoreCollision(collider)) { return; }
            prevGameObject = collider.gameObject;
            IHealth health = collider.gameObject.GetComponent<IHealth>();
            if (health == null) { Reflect(); }
            else { base.ProcessHealthEffects(collider, health); }
            Knockback(collider);
            if (IsDestroyed()) { Destroy(this.gameObject); }
        }

        /// <summary>
        /// Whether or not the object should be destroyed after interacting with
        /// the collider. The object is destroyed if it has ran down its
        /// PiercingCount or its RicochetCount,
        /// </summary>
        /// <returns>
        /// True if piercing count or ricochet count are below zero. False
        /// otherwise.
        /// </returns>
        protected override bool IsDestroyed()
        {
            return this.PiercingCount < 0 || this.RicochetCount < 0;
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Performs a raycast to update the info required for the trigger to
        /// reflect off of objects. Draws lines that are visible in the scene
        /// window. Since the projectile uses a trigger collider, it does not
        /// actually collide with anything. As such, it needs to use Raycasting
        /// to get collision-related reflection info.
        /// </summary>
        private void UpdateReflectionInfo()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, this.MaxRaycastDistance, this.LayerMask,
                QueryTriggerInteraction.UseGlobal))
            {
                this.hitInfo = hit;
                Debug.DrawLine(ray.origin, hit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin +
                    ray.direction * this.MaxRaycastDistance, Color.green);
            }
        }

        /// <summary>
        /// Elastically reflects the DWGO off of the wall. Calculates the reflect
        /// direction/rotation and launches the projectile. Decrements ricochet
        /// count. Reflection info is updated after a reflection in case the
        /// refresh rate is too slow for a quick corner bounce.
        /// </summary>
        protected void Reflect()
        {
            Vector3 reflectDirection = Vector3.Reflect(transform.forward, hitInfo.normal);
            transform.eulerAngles = new Vector3(0, base.GetAngleFromVectorFloat(reflectDirection), 0);
            LaunchForward();
            this.RicochetCount -= 1;
            if (this.RicochetCount > 0) { UpdateReflectionInfo(); }
        }

        /// <summary>
        /// Resets the projectile velocity and launches it forward. In unity,
        /// "forward" corresponds to the Z axis. Projectile prefabs are setup so
        /// that they face positively this axis.
        /// </summary>
        private void LaunchForward()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(transform.forward * this.MoveSpeed, ForceMode.Impulse);
        }
        #endregion
    }
}
