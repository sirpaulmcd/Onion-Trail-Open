using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This class is used to make sprites dynamically face the camera.
/// </summary>
public class Billboard : MonoBehaviour
{
    //=========================================================================
    // Instance variables
    //=========================================================================
    /// <summary>
    /// The main camera in the scene.
    /// </summary>
    private Camera _mainCamera;

    //=========================================================================
    // Monobehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    {
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        FaceCamera();
    }

    //=========================================================================
    // Helper methods
    //=========================================================================
    /// <summary>
    /// Rotates object to face camera.
    /// </summary>
    private void FaceCamera()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, 
                         _mainCamera.transform.rotation * Vector3.up);
        }
    }
}
}