using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// The game manager keeps track of what level the game is currently in,
/// loading and unloading game levels, tracking game state, and generating
/// other persistent systems. This manager was made following a tutorial:
/// https://learn.unity.com/project/swords-and-shovels-game-managers-loads-and-the-game-loop?uv=2019.3 
/// </summary>
public class GameManager : Singleton<GameManager>
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    private string _currentSceneName;
    private GameState _currentGameState;
    private Stack<GameState> _prevGameStates = new Stack<GameState>();
    /// <summary>
    /// List of asynchronous load operations that are to be addressed when
    /// loading a level.
    /// </summary>
    private List<AsyncOperation> _loadOperations = new List<AsyncOperation>();
    /// <summary>
    /// List of system manager prefabs that the GameManager generates and 
    /// manages.
    /// </summary>
    [SerializeField] private GameObject[] _systemPrefabs = default;
    /// <summary>
    /// List of system prefabs that have been instantiated by the GameManager
    /// in the current game instance.
    /// </summary>
    private List<GameObject> _instancedSystemPrefabs = new List<GameObject>();
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    public string CurrentSceneName
    {
        get { return _currentSceneName; }
        private set { _currentSceneName = value; }
    }
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    protected override void Awake()
    {
        base.Awake();
        InitOnAwake();
        InstantiateSystemPrefabs();
        InitializeStaticSystems();
    }

    private void Start()
    {
        InitVars();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DestroySystemPrefabs();
    }
    #endregion

    //=========================================================================
    #region Initialization
    //=========================================================================
    private void InitOnAwake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void InitVars()
    {
        _currentSceneName = SceneManager.GetActiveScene().name;
        if (_currentSceneName == SceneName.BOOT) { CurrentGameState = GameState.PREGAME; }
        else { CurrentGameState = GameState.RUNNING; }
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    private void InvokeGameStateUpdatedEvent(GameState newState, GameState prevState, GameObject initiator)
    {
        Debug.Log("GameManager.InvokeGameStateUpdatedEvent: Game state " +
        "changed from " + prevState + " to " + newState + ".");
        EventManager.Instance.Invoke(EventName.GameStateUpdatedEvent, 
            new GameStateUpdatedEventArgs(newState, prevState, initiator),
            this);
    }
    #endregion

    //=========================================================================
    #region System management
    //=========================================================================
    private void InstantiateSystemPrefabs()
    {
        if (UIManager.Instance != null) { return; }
        GameObject instance; 
        foreach (GameObject systemPrefab in _systemPrefabs)
        {
            instance = Instantiate(systemPrefab);
            instance.transform.parent = this.transform;
            _instancedSystemPrefabs.Add(instance);
        }
    }

    private void DestroySystemPrefabs()
    {
        foreach (GameObject instance in _instancedSystemPrefabs)
        {
            Destroy(instance);
        }
        _instancedSystemPrefabs.Clear();
    }

    private void InitializeStaticSystems()
    {
        // TODO: EventManager.Init();
    }
    #endregion

    //=========================================================================
    #region Scene loading
    //=========================================================================
    public void LoadScene(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        if (ao == null) 
        { 
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return; 
        }
        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);
        _currentSceneName = levelName;
    }

    public void UnloadScene(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null) 
        { 
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return; 
        }
        ao.completed += OnUnloadOperationComplete;
    }

    /// <summary>
    /// Called after a scene is finished loading.
    /// </summary>
    private void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);
            if (_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }
        }
        Debug.Log("Load complete.");
    }

    /// <summary>
    /// Called after a scene is finished unloading.
    /// </summary>
    private void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload complete.");
    }
    #endregion

    //=========================================================================
    #region Game states 
    //=========================================================================
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        DIALOGUE,
        LOADING
    }

    private void UpdateState(GameState newState, GameObject initiator=null)
    {
        if (newState == CurrentGameState) { return; }
        GameState prevState = CurrentGameState;
        CurrentGameState = newState;
        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;
            case GameState.PAUSED:
                Time.timeScale = 0.0f;
                break;
            case GameState.LOADING: 
                Time.timeScale = 1.0f;
                break;
            case GameState.DIALOGUE:
                Time.timeScale = 0.0f;
                break;
            default:
                break;
        }
        InvokeGameStateUpdatedEvent(newState, prevState, initiator);
    }
    #endregion

    //=========================================================================
    #region Pausing, restarting, and quitting 
    //=========================================================================
    public void TogglePause(GameObject initiator=null)
    {
        if (CurrentGameState == GameState.PAUSED)
        {
            UpdateState(_prevGameStates.Pop());
        }
        else
        {
            _prevGameStates.Push(CurrentGameState);
            UpdateState(GameState.PAUSED);
        }
    }

    public void ToggleDialogue(GameObject initiator=null)
    {
        if (CurrentGameState == GameState.DIALOGUE)
        {
            UpdateState(_prevGameStates.Pop());
        }
        else
        {
            _prevGameStates.Push(CurrentGameState);
            UpdateState(GameState.DIALOGUE);
        }
    }

    public void RestartGame()
    {
        UpdateState(GameState.PREGAME);
    }

    public void QuitGame()
    {
        // TODO: Perform necessary cleanup
        Application.Quit();
    }
    #endregion
}
}