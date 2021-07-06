using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For InitOnSceneLoad

namespace EGS
{
    /// <summary>
    /// This class is used to make sprites dynamically face the camera.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The main camera in the scene.
        /// </summary>
        [SerializeField] private GameObject _mainCamera;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            FaceCamera();
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Rotates object to face camera.
        /// </summary>
        private void FaceCamera()
        {
            if (_mainCamera == null) { _mainCamera = GameObject.Find("MainCamera"); }
            else
            {
                // transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                //     _mainCamera.transform.rotation * Vector3.up);
                transform.forward = _mainCamera.transform.forward;
            }
        }
        #endregion
    }
}
