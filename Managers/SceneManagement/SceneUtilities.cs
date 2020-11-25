using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class provides the utilities required to initialize static
/// classses and destroy game related DontDestroyOnLoad objects if the
/// players return to the main menu.
/// </summary>
public class SceneUtilities : MonoBehaviour
{
    //=========================================================================
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The current instance of SceneUtilties.
    /// </summary>
    public static SceneUtilities instance = default;
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
	private void Awake()
	{
        InitStaticClasses();
        SetUpSingleton();
	}

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Call InitOnSceneLoad() whenever a scene is loaded.
        SceneManager.sceneLoaded += InitOnSceneLoad;
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Destroys the Singleton (i.e. DontDestroyOnLoad) objects related to
    /// the current game. To be called when a game is left to return to the
    /// main menu.
    /// </summary>
    public void DestroyGameSingletons()
    {
        Destroy(HUD.instance.gameObject);
        Destroy(GameSession.instance.gameObject);
        Destroy(PlayerManager.instance.gameObject);
        Destroy(DialogueCanvas.instance.gameObject);
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Sets up a singleton pattern such that, if an instance of SceneUtilites
    /// already exists, no more are created. Using DontDestroyOnLoad, the
    /// SceneUtilites instance can persist through various scenes.
    /// </summary>
    private void SetUpSingleton()
    {
        if (SceneUtilities.instance == null) 
        {
            SceneUtilities.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Initialises static classes. Must check if instance is null or this will
    /// be called whenever a new scene is loaded.
    /// </summary>
    private void InitStaticClasses()
    {
        if (SceneUtilities.instance == null)
        {
            EventManager.Init();
            AudioManager.Init();
            CanvasManager.Init();
        }
    }

    /// <summary>
    /// To be called when a scene is loaded. The initializer for CanvasManager
    /// is called here to ensure that the pause menu is updated to reflect the
    /// current scene.
    /// </summary>
    private void InitOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        CanvasManager.InitPauseCanvas();
        AudioManager.InitAudioSource();
    }
    #endregion
}
}