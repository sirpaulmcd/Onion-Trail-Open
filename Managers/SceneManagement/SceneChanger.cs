using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class is responsible for changing between scenes in the game.
/// </summary>
public static class SceneChanger
{
    //==========================================================================
    #region Class methods
    //==========================================================================
    public static void DefaultSceneChange(string sceneName)
    {
        Camera.main.GetComponent<CameraMovement>().RemoveAllFocuses();
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Destroys all game related DontDestroyOnLoad objects before loading a 
    /// menu scene.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void GameToMenuSceneChange(string sceneName)
    {
        SceneUtilities.instance.DestroyGameSingletons();
        Camera.main.GetComponent<CameraMovement>().RemoveAllFocuses();
        SceneManager.LoadScene(sceneName);
    }
    #endregion
}
}
