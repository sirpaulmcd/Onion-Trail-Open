using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// This script is used to inhibit rotation of an object. Particuarly,
    /// it inhits the rotation of a child object in the case that the parent
    /// is rotated. This script is used to prevent entity sprites from
    /// rotating.
    /// </summary>
    public class FixRotation : MonoBehaviour
    {
        //=====================================================================
        // Instance variables
        //=====================================================================
        /// <summary>
        /// The original rotation of the object transform.
        /// </summary>
        private Quaternion _rotation;

        //=====================================================================
        // MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called when the script instance is loaded.
        /// </summary>
        void Awake()
        {
            // Stores the initial orientation of the object
            _rotation = transform.rotation;
        }

        /// <summary>
        /// Called after Update.
        /// </summary>
        void LateUpdate()
        {
            // Sets the object back to its initial orientation
            transform.rotation = _rotation;
        }
    }
}




