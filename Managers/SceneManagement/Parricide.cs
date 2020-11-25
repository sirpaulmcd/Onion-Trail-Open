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
            GameObject[] folderGameObjects = GameObject.FindGameObjectsWithTag("Folder");
            if (folderGameObjects.Length <= 0) { return; }
            foreach (GameObject folderObject in folderGameObjects)
            {
                folderObject.transform.DetachChildren();
                Destroy(folderObject);
            }
        }
        #endregion
    }
}