using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class is used to deconstruct the scene hierarchy folder system.
    /// Folder GameObjects are for inspector use only and waste memory. 
    /// Additionally, having everything as a child takes its toll on 
    /// performance. Therefore, this script is used to delete folder
    /// GameObjects such that everything is placed back onto the root level
    /// of the scene hierarchy during runtime.
    /// </summary>
    public class Parricide : MonoBehaviour
    {
        //=====================================================================
        #region Inspector linked components
        //=====================================================================
        /// <summary>
        /// The list of GameObjects used as folders to organize the Unity 
        /// hierarchy.
        /// </summary>
        [SerializeField] private GameObject[] _folderGameObjects = default;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            RemoveHierarchyFolders();   
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Detaches children from and destroys all folder GameObjects.
        /// </summary>
        private void RemoveHierarchyFolders()
        {
            if (_folderGameObjects.Length < 0) { return; }
            foreach (GameObject folderObject in _folderGameObjects)
            {
                folderObject.transform.DetachChildren();
                Destroy(folderObject);
            }
        }
        #endregion
    }
}