// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// 
/// </summary>
public class OTCameraMovement : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The animator of the tractor component.
    /// </summary>
    private Animator _tractorAnimator = default;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Moves the camera to the input position in the input amount of time.
    /// </summary>
    /// <param name="position">
    /// The position that the camera will travel to.
    /// </param>
    /// <param name="travelSeconds">
    /// The seconds it will take to move the camera to the input position.
    /// </param>
    public void TravelToPosition(Vector3 position, float travelSeconds)
    {
        StartCoroutine(MoveToPosition(position, travelSeconds));
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    private void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        this.gameObject.name = "MainCamera";
        _tractorAnimator = GetComponentInChildren<Animator>();
        transform.position = new Vector3(OTGameSession.Instance.DistanceTraveled, 0, 0);
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null) { player.Camera = this.gameObject; } 
        }
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_tractorAnimator, gameObject.name + " is missing _tractorAnimator");
    }

    /// <summary>
    /// Moves the camera to the input position in the input amount of time.
    /// </summary>
    /// <param name="position">
    /// The position that the camera will travel to.
    /// </param>
    /// <param name="travelSeconds">
    /// The seconds it will take to move the camera to the input position.
    /// </param>
    private IEnumerator MoveToPosition(Vector3 position, float travelSeconds)
    {
        _tractorAnimator.SetBool("isMoving", true);
        Vector3 currentPosition = this.transform.position;
        Vector3 targetPosition = position;
        float currentTime = 0.0f;
    
        while(currentTime <= travelSeconds)
        {
            float movementFactor = currentTime / travelSeconds;
            this.transform.position = Vector3.Lerp(currentPosition, targetPosition, movementFactor);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.position = position;
        _tractorAnimator.SetBool("isMoving", false);
    }
    #endregion
}
}