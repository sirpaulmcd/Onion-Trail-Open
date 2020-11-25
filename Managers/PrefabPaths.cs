// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This static class is used to store paths to prefabs. These paths can be
/// used to dynamically instantiate prefabs in-game.
/// </summary>
public static class PrefabPaths
{
    //=========================================================================
    #region UI Elements
    //=========================================================================
    /// <summary>
    /// The path to all UI prefabs.
    /// </summary>
    static string UIElementsPath = "Prefabs/UI/UIElements/";
    /// <summary>
    /// The crosshair prefab for the player.
    /// </summary>
    public static string PLAYERCROSSHAIR = UIElementsPath + "PlayerCrosshair";
    #endregion

    //=========================================================================
    #region UI Canvases
    //=========================================================================
    /// <summary>
    /// The path to all UI prefabs.
    /// </summary>
    static string UICanvasesPath = "Prefabs/UI/Canvases/";
    /// <summary>
    /// The canvas for displaying dialogue.
    /// </summary>
    public static string DIALOGUECANVAS = UICanvasesPath + "DialogueCanvas";
    /// <summary>
    /// The canvas for displaying a Game Over info
    /// </summary>
    public static string GAMEOVERCANVAS = UICanvasesPath + "GameOverCanvas";
    /// <summary>
    /// The canvas for the main menu.
    /// </summary>
    public static string MAINCANVAS = UICanvasesPath + "MainCanvas";
    /// <summary>
    /// The canvas for the pause menu.
    /// </summary>
    public static string PAUSECANVAS = UICanvasesPath + "PauseCanvas";
    #endregion
}
}