using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class manages the intatiation and toggling of UI canvases within the 
/// game. This includes menu navigation and pausing.
/// </summary>
public static class CanvasManager
{
    //=========================================================================
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The pause canvas in the current scene (if any).
    /// </summary>
    public static GameObject pauseCanvas;
    #endregion

    //=========================================================================
    #region Class methods
    //=========================================================================
    /// <summary>
    /// Initializes the CanvasManager. To be called once on initial load.
    /// </summary>
    public static void Init()
    {
        InitPauseCanvas();
    }

    /// <summary>
    /// Initializes the CanvasManager variables. To be called every scene 
    /// change.
    /// </summary>
    public static void InitPauseCanvas()
    {
        PauseMenu[] pauseMenu = Resources.FindObjectsOfTypeAll<PauseMenu>();
        if (pauseMenu.Length >= 1) { pauseCanvas = pauseMenu[0].gameObject; }
    }

    /// <summary>
    /// Instantiates a canvas prefab into the scene.
    /// </summary>
    /// <param name="name">
    /// The name of the canvas prefab to instantiate. See CanvasName.cs
    /// </param>
    public static void InstantiateCanvas(string name)
    {
        switch (name)
        {
            case CanvasName.GAMEOVER:
                GameObject.Instantiate(Resources.Load("Prefabs/UIManagement/" + CanvasName.GAMEOVER));
                Time.timeScale = 0;
                break;
            default:
                Debug.Log("Invalid input menu name: " + name);
                break;
        }
    }

    /// <summary>
    /// Toggles the pause menu in the scene (if any).
    /// </summary>
    /// <param name="togglingPlayer">
    /// The player that is pausing the game.
    /// </param>
    public static void TogglePause(GameObject togglingPlayer)
    {
        if (pauseCanvas == null) { return; }
        if (pauseCanvas.activeSelf) 
        {
            pauseCanvas.SetActive(false);
            Time.timeScale = 1;
            PlayerManager.instance.UndoLeaderControlChange();
        }
        else
        {
            pauseCanvas.SetActive(true);
            Time.timeScale = 0;
            PlayerManager.instance.LeaderControlChange(togglingPlayer, "UI");
        }
    }
    #endregion
}
}