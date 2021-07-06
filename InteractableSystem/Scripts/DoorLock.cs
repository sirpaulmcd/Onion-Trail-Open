using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// This is a simple script that destroys the DoorLock and Key on collision.
    /// This script checks to see if the object interacting is tagged as a key.
    /// Future edits:
    /// - Add DoorLock animation for opening instead of destruction
    /// - Add different sprite/model for the Lock
    /// Possible future edits:
    /// - Add keycode system to only allow a specific key to work
    /// - Add key/lock interaction animation?
    /// </summary>
    public class DoorLock : MonoBehaviour
    {
        //=====================================================================
        // Public methods
        //=====================================================================
        /// <summary>
        /// On collision, detects if the collider is a key. If it is, destroy the
        /// barrier. Destruction should be replaced with an animation in the future.
        /// <summary>
        void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.tag == "Key")
            {
                Destroy(this.gameObject);
                Destroy(collider.gameObject);
            }
        }
    }
}
