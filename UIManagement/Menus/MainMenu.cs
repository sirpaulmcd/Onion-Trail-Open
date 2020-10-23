using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class is responsible for managing the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
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
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_defaultButton);
    }
    #endregion

    //==========================================================================
    #region Public methods
    //==========================================================================
    /// <summary>
    /// To be called when the start button is pressed. Loads the first scene.
    /// </summary>
    public void HandleStartButtonPress()
    {
        SceneManager.LoadScene(SceneName.SAMPLE);
    }

    /// <summary>
    /// To be called when the quit button is pressed. Closes application 
    /// (does not work in editor).
    /// </summary>
    public void HandleQuitButtonPress()
    {
        Application.Quit();                     
    }
    #endregion

    //==========================================================================
    #region Private methods
    //==========================================================================
    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_defaultButton, gameObject.name + " is missing _defaultButton");
    }
    #endregion
}
}