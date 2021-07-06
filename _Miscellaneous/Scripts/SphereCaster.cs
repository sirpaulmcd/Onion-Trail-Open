using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// This class allows gameobjects to perform spherecasting.
    /// Essentially, a sphere is shot from the gameobject like a bullet
    /// and gathers information about the object that it hits.
    /// </summary>
    public class SphereCaster : MonoBehaviour
    {
        //=====================================================================
        // Instance variables
        //=====================================================================
        // [Header("SphereCast")]
        /// <summary>
        /// The radius of the sphere used in the spherecast.
        /// </summary>
        [SerializeField] private float _sphereRadius = 0.05f;
        /// <summary>
        /// The LayerMask that the spherecast can detect. To set the
        /// LayerMask to detect everything, initialize as -1.
        /// </summary>
        [SerializeField] private LayerMask _layerMask = -1;

        //=====================================================================
        // [Header("Visualization")]
        /// <summary>
        /// The distance of the current hit object.
        /// </summary>
        private float _currentHitDistance;
        /// <summary>
        /// The direction of the current spherecast.
        /// </summary>
        private Vector3 _currentDirection;

        //=====================================================================
        // Public methods
        //=====================================================================
        /// <summary>
        /// Sends a spherecast in the input direction and returns the
        /// first gameobject to collide with the sphere.
        /// </summary>
        /// <param name="direction">
        /// The direction to perform the spherecast.
        /// </param>
        /// <param name="maxDistance">
        /// The max distance that the spherecast is to travel.
        /// </param>
        /// <returns>
        /// Colliding gameobject (if any), null otherwise.
        /// </returns>
        public GameObject SphereCast(Vector3 direction, float maxDistance)
        {
            // Reset visualization variables
            _currentHitDistance = 0;
            _currentDirection = Vector3.zero;

            // If spherecast hits a gameobject...
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, _sphereRadius, direction,
                                    out hitInfo, maxDistance, _layerMask,
                                    QueryTriggerInteraction.UseGlobal))
            {
                // Update visualization variables
                _currentHitDistance = hitInfo.distance;
                _currentDirection = direction;
                // Return gameobject of collision
                return hitInfo.transform.gameObject;
            }
            // If spherecast hits nothing...
            else
            {
                // Update visualization variables
                _currentHitDistance = maxDistance;
                _currentDirection = direction;
                // Return null
                return null;
            }
        }

        //=====================================================================
        // Helper methods
        //=====================================================================
        /// <summary>
        /// Draws the spherecast so that it is visible in scene view for debugging.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Debug.DrawLine(transform.position, transform.position +
                            _currentDirection * _currentHitDistance);
            Gizmos.DrawWireSphere(transform.position +
                                    _currentDirection * _currentHitDistance,
                                    _sphereRadius);
        }
    }
}
