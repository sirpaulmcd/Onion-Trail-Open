using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Adds destrucibility to a gameObject. This is to be used on in-game props
    /// that need to be able to be destroyed by the player.
    /// </summary>
    public class DestructibleWithBrokenGameObject : ADestructible
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The object that will replace the original object such as explosion or
        /// broken version of object.
        /// </summary>
        [SerializeField] private GameObject _destroyedSpawn = default;
        /// <summary>
        /// The offset from the transform position to apply the explosive force
        /// when the object is broken.
        /// </summary>
        [SerializeField] private Vector3 _explosiveForceOffset = default;
        /// <summary>
        /// The magnitude of the explosive force to apply to the destroyedSpawn
        /// when the object is broken.
        /// </summary>
        [SerializeField] private float _explosiveForce = 200.0f;
        /// <summary>
        /// The radius of the explosive force to apply to the destroyedSpawn
        /// when the object is broken.
        /// </summary>
        [SerializeField] private float _explosionRadius = 10.0f;
        #endregion

        //=====================================================================
        #region Protected Methods
        //=====================================================================
        /// <summary>
        /// Destroys the object, spawns the broken object and applies a destructive
        /// force to the broken object.
        /// </summary>
        override protected void Destruct()
        {
            if (!hasDestructed)
            {
                GameObject go = SpawnBrokenObject();
                AddExplosiveForceToChildren(go, _explosiveForceOffset,
                    _explosionRadius, _explosiveForce);
            }
            base.Destruct();
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Spawns the broken version of the object.
        /// </summary>
        /// <returns>The gameobject that was spawned. Null if nothing was
        /// spawned.</returns>
        private GameObject SpawnBrokenObject()
        {
            if (_destroyedSpawn == null) { return null; }
            return Instantiate(_destroyedSpawn, transform.position,
                transform.rotation);
        }

        /// <summary>
        /// Adds an explosive force to all children of a gameObject.
        /// </summary>
        /// <param name="go">The gameObject with children to add explosive
        /// force to</param>
        /// <summary>
        /// Adds an explosive force to all children of a gameObject centered at
        /// the center of the gameObject plus offset.
        /// </summary>
        /// <param name="go">The gameObject with children to add explosive force
        /// to</param>
        /// <param name="explosiveOffset">The offset to the explosion for visual
        /// effect</param>
        /// <param name="explosionRadius">The blast radius of the explosion</param>
        /// <param name="explosiveForce">The force of the explosion</param>
        private void AddExplosiveForceToChildren(GameObject go,
            Vector3 explosiveOffset, float explosionRadius, float explosiveForce)
        {
            foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosiveForce, go.transform.position +
                    explosiveOffset, explosionRadius);
            }
        }
        #endregion
    }
}
