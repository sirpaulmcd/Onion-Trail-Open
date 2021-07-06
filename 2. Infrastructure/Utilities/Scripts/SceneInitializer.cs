using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class is used to standardize scene initialization. For example,
    /// removing folder GameObjects, initializing player controls, initializing
    /// music, etc.
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        //=====================================================================
        #region Class variables
        //=====================================================================
        /// <summary>
        /// The current instance of scene initializer (overwritten every time
        /// a new scene is loaded).
        /// </summary>
        public static SceneInitializer Instance;
        #endregion

        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The input action map to be applied to living players when a scene
        /// is loaded.
        /// </summary>
        [SerializeField] private string _initialActionMapName = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        public string InitialActionMapName
        {
            get { return _initialActionMapName; }
            private set { _initialActionMapName = value; }
        }
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            InitVars();
            CheckMandatoryComponents();
            RemoveHierarchyFolders();
        }

        private void Start()
        {
            InitializePlayerControls();
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        private void InitVars()
        {
            Instance = this;
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_initialActionMapName, gameObject.name + " is missing _initialActionMapName");
        }

        /// <summary>
        /// Detaches children from and destroys all folder GameObjects.
        /// Folder GameObjects are for inspector use only and waste memory.
        /// Additionally, having everything as a child takes its toll on
        /// performance. Therefore, this method is used to delete folder
        /// GameObjects such that everything is placed back onto the root level
        /// of the scene hierarchy during runtime.
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

        private void InitializePlayerControls()
        {
            Debug.Log("Initializing player controls to " + _initialActionMapName);
            PlayerManager.Instance.InitializeControlStack();
            PlayerManager.Instance.ChangeAllLivingPlayerControls(_initialActionMapName);
        }
        #endregion
    }
}
