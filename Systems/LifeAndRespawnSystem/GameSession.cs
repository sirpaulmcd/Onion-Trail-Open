using System; // Holds EventHandler
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace EGS
{
/// <summary>
/// This class is responsible for tracking active Game Session information such
/// as squad lives. GameSession cannot be a completely static class because
/// it must invoke events and inherit from EventInvoker. To get around this,
/// a static variable called "instance" is used for the same effect.
/// </summary>
public class GameSession : EventInvoker
{
    //=========================================================================
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The instance of the GameSession. 
    /// </summary>
    /// <remarks>
    /// Since there is only ever one instance of GameSession necessary, storing
    /// it as a class variable will allow other classes to access it without
    /// linking it through an instance variable.
    /// </remarks>
    public static GameSession instance = default;
    #endregion

    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The number of lives the squad has left.
    /// </summary>
    [SerializeField] private int _squadLives = 1;
    /// <summary>
    /// The position of the latest checkpoint where players are to be respawned
    /// upon a squad death.
    /// </summary>
    [SerializeField] private Vector3 _checkpointPosition;
    /// <summary>
    /// The position a dead player is teleported to while they are deceased. 
    /// Player GameObjects are not destroyed upon death because this would 
    /// disconnect the PlayerInput.
    /// </summary>
    [SerializeField] private Vector3 _limboPosition = new Vector3(0, -100, 0);
    /// <summary>
    /// The delay between a GameOver event being called and the GameOver
    /// canvas appearing.
    /// </summary>
    [SerializeField] private int _gameOverScreenDelaySeconds = 3;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The number of lives the squad has left. When setting, ensures lives
    /// is not negative.
    /// </summary>
    public int SquadLives
    {
        get { return _squadLives; }
        set 
        {
            int newLives = value;
            if (newLives != _squadLives)
            {
                _squadLives = newLives;
                InvokeLivesUpdatedEvent();
                if (OutOfLives())
                {
                    InvokeGameOverEvent();
                    StartCoroutine(GameOver());
                }
            }
        }
    }

    /// <summary>
    /// The position of the latest checkpoint where players are to be respawned
    /// upon a squad death.
    /// </summary>
    public Vector3 CheckpointPosition
    {
        get { return _checkpointPosition; }
        set { _checkpointPosition = value; }
    }

    /// <summary>
    /// The position a dead player is teleported to while they are deceased. 
    /// Player GameObjects are not destroyed upon death because this would 
    /// disconnect the PlayerInput.
    /// </summary>
    public Vector3 LimboPosition 
    { 
        get { return _limboPosition; } 
        set { _limboPosition = value; } 
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    /// <summary>
    /// Invokes a LivesUpdatedEvent.
    /// </summary>
    private void InvokeLivesUpdatedEvent()
    {
        LivesUpdatedEventArgs args = new LivesUpdatedEventArgs(_squadLives);
        invokableEvents[EventName.LivesUpdatedEvent]?.Invoke(this, args);
    }

    /// <summary>
    /// Invokes a GameOverEvent.
    /// </summary>
    private void InvokeGameOverEvent()
    {
        invokableEvents[EventName.GameOverEvent]?.Invoke(this, new GameOverEventArgs());
    }
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        SetUpSingleton();
    }

    private void Start()
    {
        RegisterAsInvoker();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Increments lives by input integer.
    /// </summary>
    /// <param name="num">
    /// The integer of lives to be added.
    /// </param>
    public void IncrementLives(int num)
    {
        this.SquadLives += num;
    }

    /// <summary>
    /// Decrements lives by input integer. If squad has remaining lives,
    /// respawns them. This method only works if no players are currently
    /// living.
    /// </summary>
    /// <param name="num">
    /// The integer of lives to be removed.
    /// </param>
    public void AttemptDecrementLives(int num)
    {
        if (PlayerManager.instance.GetLivingPlayerCount() == 0) 
        {
            // Decrement squadlives
            this.SquadLives -= num;
            // If squad still has lives, respawn
            if (!OutOfLives()) { StartCoroutine(PlayerManager.instance.WaitAndRespawn()); }
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Sets up a singleton pattern such that, if an instance of GameSession
    /// already exists, no more are created. Using DontDestroyOnLoad, the
    /// GameSession instance can persist through various scenes.
    /// </summary>
    private void SetUpSingleton()
    {
        if (GameSession.instance == null) 
        {
            GameSession.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Registers the GameSession as an invoker of various events.
    /// </summary>
    private void RegisterAsInvoker()
    {
        // Register as invoker for GameOverEvent
		invokableEvents.Add(EventName.GameOverEvent, GameOverEvent.GetGameOverEventHandler());
		EventManager.AddInvoker(EventName.GameOverEvent, this);
        // Register as invoker for LivesUpdatedEvent
        invokableEvents.Add(EventName.LivesUpdatedEvent, LivesUpdatedEvent.GetLivesUpdatedEventHandler());
		EventManager.AddInvoker(EventName.LivesUpdatedEvent, this);
    }

    /// <summary>
    /// Returns bool indicating whether the squad is dead. A squad is 
    /// considered dead when they have died with zero lives remaining.
    /// (i.e. squadLives <= 0)
    /// </summary>
    /// <returns>
    /// True if squad cannot be respawned, false otherwise.
    /// </returns>
    private bool OutOfLives()
    {
        return _squadLives <= 0;
    }

    /// <summary>
    /// To be called when players have run out of lives. Activates Game Over
    /// canvas.
    /// </summary>
    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(_gameOverScreenDelaySeconds);
        CanvasManager.InstantiateCanvas(PrefabPaths.GAMEOVERCANVAS, true);
    }
    #endregion
}
}