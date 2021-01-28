using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class is responsible for tracking active Game Session information such
/// as the current checkpoint and respawning.
/// </summary>
public class LARSGameSession : Singleton<LARSGameSession>
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
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
    /// The number of seconds between a respawn call and when the player
    /// actually respawns. Ensures players do not respawn instantly after 
    /// death.
    /// </summary>
    [SerializeField] private int _squadRespawnWaitSeconds = 3;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
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
    #region MonoBehavior
    //=========================================================================
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Call InitOnSceneLoad when new scene is loaded.
        SceneManager.sceneLoaded += InitOnSceneLoad;
        RegisterAsListener();
    }

    private void OnDisable()
    {
        DeregisterAsListener();
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    /// <summary>
    /// Invokes a GameOverEvent.
    /// </summary>
    private void InvokeGameOverEvent()
    {
        Debug.Log("LARSGameSession.InvokeGameOverEvent: All players have died.");
        EventManager.Instance.Invoke(EventName.GameOverEvent, new GameOverEventArgs(), this);
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Register as listener to relevant events.
    /// </summary>
    private void RegisterAsListener()
    {
        EventManager.Instance.AddListener(EventName.DeathEvent, HandleDeathEvent);
        EventManager.Instance.AddListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        EventManager.Instance.AddListener(EventName.PlayerAddedEvent, HandlePlayerAddedEvent);
    }

    /// <summary>
    /// Deregister as listener to relevant events.
    /// </summary>
    private void DeregisterAsListener()
    {
        EventManager.Instance.RemoveListener(EventName.DeathEvent, HandleDeathEvent);
        EventManager.Instance.RemoveListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        EventManager.Instance.RemoveListener(EventName.PlayerAddedEvent, HandlePlayerAddedEvent);
    }

    /// <summary>
    /// Called after a GameOverEvent is invoked.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleDeathEvent(object invoker, System.EventArgs e)
    {
        DeathEventArgs args = (DeathEventArgs) e;
        // TODO: Check if all players dead
        // If so, invoke GameOver
        Debug.Log("LARSGameSession.HandleDeathEvent: Checking if all players have died.");
    }

    /// <summary>
    /// Called after a PlayerAddedEvent is invoked.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandlePlayerAddedEvent(object invoker, System.EventArgs e)
    {
        PlayerAddedEventArgs args = (PlayerAddedEventArgs) e;
        TeleportToCheckpoint(args.Player.GameObject);
        Debug.Log("LARSGameSession.HandlePlayerAddedEvent: Teleporting player to checkpoint.");
    }

    private void HandleGameStateUpdatedEvent(object invoker, System.EventArgs e)
    {
        GameStateUpdatedEventArgs args = (GameStateUpdatedEventArgs) e;
        if (args.PrevState == GameManager.GameState.DIALOGUE)
        {
            Debug.Log("LARSGameSession.HandleGameStateUpdatedEvent: Making players vulnerable.");
            MakePlayersVulnerable();
        }
        else if (args.NewState == GameManager.GameState.DIALOGUE)
        {
            Debug.Log("LARSGameSession.HandleGameStateUpdatedEvent: Making players invulnerable.");
            MakePlayersInvulnerable();
        }
    }
    #endregion


    //=========================================================================
    #region Initialization
    //=========================================================================
    /// <summary>
    /// To be called when a scene has loaded.
    /// </summary>
    private void InitOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        LARSCameraMovement cm = Camera.main.GetComponent<LARSCameraMovement>();
        if (cm != null) { AddExistingPlayersToCameraFocus(); }
    }

    /// <summary>
    /// Adds existing players in the scene to the camera focus if they are 
    /// alive. To be called when a new scene is loaded.
    /// </summary>
    private void AddExistingPlayersToCameraFocus()
    {
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null && player.GameObject != null && player.State != PlayerState.Dead)
            {
                Camera.main.GetComponent<LARSCameraMovement>().AddFocus(player.GameObject.transform);
            }
        }
    }
    #endregion

    //=========================================================================
    #region Status checkers
    //=========================================================================
    /// <summary>
    /// Gets the number of living players. Players are considered alive if
    /// their state is not dead.
    /// </summary>
    /// <returns>
    /// Returns the number of living players in the scene.
    /// </returns>
    public int GetLivingPlayerCount()
    {
        int livingPlayerCount = 0;
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null && player.State != PlayerState.Dead) { livingPlayerCount += 1; }
        }
        return livingPlayerCount;
    }

    /// <summary>
    /// Checks if all squad members are incapacitated or dead.
    /// </summary>
    /// <returns>
    /// True if no players are in the default PlayerState, false otherwise.
    /// </returns>
    public bool AllIncapacitatedOrDead()
    {
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null && player.State == PlayerState.Default) { return false; }
        }
        return true;
    }
    #endregion

    //=========================================================================
    #region Respawn and teleportation
    //=========================================================================
    /// <summary>
    /// Respawns currently dead players at the latest checkpoint.
    /// </summary>
    public void RespawnDeadPlayers()
    {
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null && player.State == PlayerState.Dead)
            {
                Respawn(player);
            }
        }
    }

    /// <summary>
    /// Waits _squadRespawnWaitSeconds and respawns squad. The wait time ensures
    /// squad does not spawn immediately after death.
    /// </summary>
    /// <returns>
    /// IEnumerator object associated with the coroutine.
    /// </returns>
    public IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(_squadRespawnWaitSeconds);
        RespawnDeadPlayers();
    }

    /// <summary>
    /// Respawns a player. Using this method, players are teleported to the
    /// most recent checkpoint and revived.
    /// </summary>
    /// <param name="player">
    /// The Player object associated with the GameObject to be respawned.
    /// </param>
    private void Respawn(Player player)
    {
        TeleportToCheckpoint(player.GameObject);
        Revive(player.GameObject);
    }

    /// <summary>
    /// Revives a player. Using this method, the player is given full health
    /// and all death effects are removed.
    /// </summary>
    /// <param name="player">
    /// The player GameObject to be revivied.
    /// </param>
    private void Revive(GameObject player)
    {
        player.GetComponent<PlayerHealth>().IsDead = false;
    }

    /// <summary>
    /// Teleports all players to the _checkpointPosition. For use when a new 
    /// scene is loaded.
    /// </summary>
    public void TeleportLivingPlayersToCheckpoint()
    {
        if (PlayerManager.Instance.Players.Length == 0 ) { return; }
        foreach (Player player in PlayerManager.Instance.Players)
        {   
            // If player not dead...
            if (player != null && player.State != PlayerState.Dead)
            {
                TeleportToCheckpoint(player.GameObject);
            }
        }
    }

    /// <summary>
    /// Teleports the input player to the proper checkpoint position. Players are
    /// teleported to checkpoint with an offset according to their player index
    /// such that they do not spawn inside each other.
    /// </summary>
    /// <param name="player">
    /// The GameObject of the player to be teleported.
    /// </param>
    public void TeleportToCheckpoint(GameObject player)
    {
        int playerIndex = PlayerManager.Instance.GetPlayerInstance(player).Index;
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        switch (playerIndex)
        {
            case 0: 
                playerTeleporter.Teleport(CheckpointPosition + new Vector3(-1, 0, 0));
                break;
            case 1: 
                playerTeleporter.Teleport(CheckpointPosition + new Vector3(1, 0, 0));
                break;
            case 2: 
                playerTeleporter.Teleport(CheckpointPosition + new Vector3(0, 0, 1));
                break;
            case 3: 
                playerTeleporter.Teleport(CheckpointPosition + new Vector3(0, 0, -1));
                break;
            default:
                playerTeleporter.Teleport(CheckpointPosition);
                Debug.LogError("Unexpected player index received.", this);
                break;
        }
    }
    #endregion

    //=========================================================================
    #region Invulnerability
    //=========================================================================
    /// <summary>
    /// Makes all players invulnerable to damage. Currently called to ensure
    /// players can't die by projectiles when stuck in a dialogue sequence.
    /// </summary>
    public void MakePlayersInvulnerable()
    {
        if (PlayerManager.Instance.Players.Length == 0 ) { return; }
        foreach (Player player in PlayerManager.Instance.Players)
        {   
            if (player != null)
            {
                player.Health.IsInvulnerable = true;
            }
        }
    }

    /// <summary>
    /// Makes all players vulnerable to damage.
    /// </summary>
    public void MakePlayersVulnerable()
    {
        if (PlayerManager.Instance.Players.Length == 0 ) { return; }
        foreach (Player player in PlayerManager.Instance.Players)
        {   
            if (player != null)
            {
                player.Health.IsInvulnerable = false;
            }
        }
    }
    #endregion

    //=========================================================================
    #region Kill players
    //=========================================================================
    /// <summary>
    /// Kills all living players. Used to call squad death after all squad
    /// members have been incapacitated.
    /// </summary>
    public void KillAllPlayers()
    {
        foreach (Player player in PlayerManager.Instance.Players)
        {
            if (player != null && player.State != PlayerState.Dead)
            {
                PlayerHealth playerHealth = player.GameObject.GetComponent<PlayerHealth>();
                playerHealth.Kill();
            }
        }
    }
    #endregion
}
}