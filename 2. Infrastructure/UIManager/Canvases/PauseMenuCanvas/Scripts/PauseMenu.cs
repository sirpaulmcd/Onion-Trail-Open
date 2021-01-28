using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class is responsible for adding functionality to the pause menu 
/// canvas.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    //==========================================================================
    #region Inspector linked components
    //==========================================================================
    /// <summary>
    /// Default button selected upon opening menu.
    /// </summary>
    /// <remarks>
    /// A default button needs to be selected since the menu may be controlled
    /// with the keys of a keyboard or a gamepad (not just clicked).
    /// </remarks>
    [SerializeField] private GameObject _defaultButton = default;
    #endregion

    //==========================================================================
    #region MonoBehaviour
    //==========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        SetDefaultSelection();
    }
    #endregion

    //==========================================================================
    #region Initialization
    //==========================================================================
    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_defaultButton, gameObject.name + " is missing _defaultButton");
    }

    /// <summary>
    /// Sets a default selected button such that controllers can navigate the 
    /// menu.
    /// </summary>
    private void SetDefaultSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_defaultButton);
    }
    #endregion

    //==========================================================================
    #region Button functionality
    //==========================================================================
    /// <summary>
    /// To be called when the resume button is pressed. Unpauses the game.
    /// </summary>
    public void HandleResumeButtonPress()
    {
        GameManager.Instance.TogglePause();
    }

    /// <summary>
    /// To be called when the menu button is pressed. Unpauses the game and 
    /// returns to the main menu.
    /// </summary>
    public void HandleResetButtonPress()
    {
        GameManager.Instance.TogglePause();
        GameManager.Instance.RestartGame();
    }

    /// <summary>
    /// To be called when the quit button is pressed. Closes the application
    /// (does not work in editor).
    /// </summary>
    public void HandleQuitButtonPress()
    {
        Application.Quit();                     
    }
    #endregion
}
}