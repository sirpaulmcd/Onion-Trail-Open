using System; // For Math
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instance Bullet DWGO specialized to induce the combustion status effect and
    /// spread out in a wave pattern.
    /// </summary>
    public class FlamethrowerDWGO : BulletDWGO
    {
        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Processes the collision between colliders. Ignores certain collisions.
        /// Deals health effects to anything that has an IHealth component.
        /// Reflects off of anything that doesn't. Applies knockback effects.
        /// Lights enemies on fire.
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
            EntityStats es = collider.gameObject.GetComponent<EntityStats>();
            if (es != null) { es.IsCombusting = true; }
            if (health == null) { Reflect(); }
            else { base.ProcessHealthEffects(collider, health); }
            Knockback(collider);
            if (IsDestroyed()) { Destroy(this.gameObject); }
        }
        #endregion
    }
}
