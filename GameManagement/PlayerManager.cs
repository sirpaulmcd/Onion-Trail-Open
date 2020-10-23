using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem; // For PlayerInput 
using UnityEngine.SceneManagement;

namespace EGS
{
/// <summary>
/// This class manages the Player objects created for individual players.
/// Upon player join/leave a Player is created/destroyed and the player
/// GameObjects are processed accordingly. In game, it is responsible for
/// managing squad respawning, input swaps, state swaps, etc. This component 
/// MUST be connected to the UnityEvents of Unity's PlayerInputManager 
/// component through the inspector in order to subsrcibe to its UnityEvents. 
/// </summary>
public class PlayerManager : MonoBehaviour
{
    //=========================================================================
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The instance of the PlayerManager. 
    /// </summary>
    public static PlayerManager instance = default;
    #endregion

    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// List of Player objects currently participating in the game.
    /// </summary>
    [SerializeField] private Player[] _players;
    /// <summary>
    /// The maximum number of players allowed in the game.
    /// </summary>
    private int _maxPlayers = 4;
    /// <summary>
    /// The previous action maps used by the players before a change occurred.
    /// This change could have been triggered by players entering dialogue or
    /// the scene being paused.
    /// </summary>
    private Stack<string>[] _previousActionMaps;
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
    /// List of Player objects currently participating in the game.
    /// </summary>
    public Player[] Players
    {
        get { return _players; }
        private set { _players = value; }
    }
    #endregion

    //=========================================================================
    #region Monobehavior
    //=========================================================================
    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        InitOnAwake();
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        // Call InitOnSceneLoad when new scene is loaded.
        SceneManager.sceneLoaded += InitOnSceneLoad;
    }
    #endregion

    //=========================================================================
    #region Unity Event handlers
    //=========================================================================
    /// <summary>
    /// Handler for PlayerJoined event. Upon player join, sets the parent of 
    /// the player GameObject to the PlayerManager GameObject, assigns an
    /// info UI, and creates a new PlayerConfig.
    /// </summary>
    /// <param name="pi"> 
    /// The PlayerInput of the newly added player.
    /// </param>
    public void PlayerJoinedEventHandler(PlayerInput pi)
    {
        // Set players as child of PlayerManager singleton so they persist 
        // between scenes
        pi.transform.SetParent(transform);
        AddPlayer(new Player(pi));
        TeleportToCheckpoint(pi.gameObject);
        Debug.Log("Player " + pi.playerIndex + " joined successfully!");
    }

    /// <summary>
    /// Handler for PlayerLeft event. Upon player leave, removes the
    /// Player from the player list, deactivates info UI, and destroys player 
    /// GameObject.
    /// </summary>
    /// <param name="pi"> 
    /// The PlayerInput of the newly lost player.
    /// </param>
    public void PlayerLeftEventHandler(PlayerInput pi)
    {
        // Remove player from player list
        RemovePlayer(pi.playerIndex);
        // Destroy player GameObject
        Destroy(pi.transform.gameObject);
        Debug.Log("Player " + pi.playerIndex + " left successfully!");
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Adds a player to the _players array in the index corresponding to the
    /// players PlayerInput index.
    /// </summary>
    /// <param name="player">
    /// Player object of considered player.
    /// </param>
    public void AddPlayer(Player player)
    {
        _players[player.Index] = player;
    }

    /// <summary>
    /// Removes a player from the _players list.
    /// </summary>
    /// <param name="player">
    /// Player object of considered player.
    /// </param>
    public void RemovePlayer(int playerIndex)
    {
        Players[playerIndex]?.Terminate();
        Players[playerIndex] = null;
    }

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
        foreach (Player player in Players)
        {
            if (player != null && player.State != PlayerState.Dead) { livingPlayerCount += 1; }
        }
        return livingPlayerCount;
    }

    /// <summary>
    /// Kills all living players. Used to call squad death after all squad
    /// members have been incapacitated.
    /// </summary>
    public void KillAllPlayers()
    {
        foreach (Player player in Players)
        {
            if (player != null && player.State != PlayerState.Dead)
            {
                PlayerHealth playerHealth = player.GameObject.GetComponent<PlayerHealth>();
                playerHealth.Kill();
            }
        }
    }

    /// <summary>
    /// Checks if all squad members are incapacitated or dead.
    /// </summary>
    /// <returns>
    /// True if no players are in the default PlayerState, false otherwise.
    /// </returns>
    public bool AllIncapacitatedOrDead()
    {
        foreach (Player player in Players)
        {
            if (player != null && player.State == PlayerState.Default) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Respawns currently dead players at the latest checkpoint.
    /// </summary>
    public void RespawnDeadPlayers()
    {
        foreach (Player player in Players)
        {
            if (player != null && player.State == PlayerState.Dead)
            {
                Respawn(player);
            }
        }
    }

    /// <summary>
    /// Teleports all players to the _checkpointPosition. For use when a new 
    /// scene is loaded.
    /// </summary>
    public void TeleportLivingPlayersToCheckpoint()
    {
        if (Players.Length == 0 ) { return; }
        foreach (Player player in Players)
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
        int playerIndex = GetPlayerInstance(player).Index;
        PlayerTeleporter playerTeleporter = player.GetComponent<PlayerTeleporter>();
        switch (playerIndex)
        {
            case 0: 
                playerTeleporter.Teleport(GameSession.instance.CheckpointPosition + 
                    new Vector3(-1, 0, 0));
                break;
            case 1: 
                playerTeleporter.Teleport(GameSession.instance.CheckpointPosition + 
                    new Vector3(1, 0, 0));
                break;
            case 2: 
                playerTeleporter.Teleport(GameSession.instance.CheckpointPosition + 
                    new Vector3(0, 0, 1));
                break;
            case 3: 
                playerTeleporter.Teleport(GameSession.instance.CheckpointPosition + 
                    new Vector3(0, 0, -1));
                break;
            default:
                playerTeleporter.Teleport(GameSession.instance.CheckpointPosition);
                Debug.LogError("Unexpected player index received.", this);
                break;
        }
    }

    /// <summary>
    /// Updates the state of the player. States are outlined in the 
    /// PlayerStates enum found in Player.cs.
    /// </summary>
    /// <param name="newState">
    /// The new state that the player is taking on.
    /// </param>
    /// <param name="player">
    /// The GameObject of the player being considered.
    /// </param>
    public void UpdatePlayerState(PlayerState newState, GameObject player)
    {
        Players[GetPlayerInstance(player).Index].State = newState;
    }

    /// <summary>
    /// Changes the controls (action map) of the input player. Stores the
    /// ActionMap such that the control chance can be undone if necessary.
    /// Also zeros player inputs before switch to avoid problems.
    /// </summary>
    /// <param name="player">
    /// The player GameObject whose controls are to change.
    /// </param>
    /// <param name="actionMap">
    /// The name of the action map the player controls are to swap to. See 
    /// ActionMapName.cs for options.
    /// </param>
    public void ChangePlayerControls(GameObject player, string actionMap)
    {
        StoreCurrentActionMap(player);
        player.GetComponent<PlayerController>().ZeroPlayerInputs();
        Players[GetPlayerInstance(player).Index].Input.SwitchCurrentActionMap(actionMap);
    }

    /// <summary>
    /// Reverts player controls to the previous ActionMap. Used to undo a 
    /// control change. For example, when players unpause and get full controls
    /// again.
    /// </summary>
    public void RevertPlayerControls(GameObject playerObject)
    {
        Player player = GetPlayerInstance(playerObject);
        player.Input.SwitchCurrentActionMap(_previousActionMaps[player.Index].Pop());
    }

    /// <summary>
    /// Toggles player action maps such that only the initiating player has
    /// control of the UI menu. Used when a dialogue is opened or the game
    /// is paused.
    /// </summary>
    /// <param name="initiatingPlayer">
    /// The GameObject associated with the player that opened the UI.
    /// </param>
    public void LeaderControlChange(GameObject initiatingPlayer, string actionMap)
    {
        foreach (Player player in Players)
        {
            if (player == null) { continue; }
            if (player.GameObject == initiatingPlayer)
            {
                ChangePlayerControls(player.GameObject, actionMap);
            }
            else
            {
                ChangePlayerControls(player.GameObject, ActionMapName.BRICKED);
            }
        }
    }

    /// <summary>
    /// Reverts controls for all players to undo a leader control change.
    /// </summary>
    public void UndoLeaderControlChange()
    {
        foreach (Player player in Players)
        {
            if (player == null) { continue; }
            RevertPlayerControls(player.GameObject);
        }
    }

    /// <summary>
    /// Makes all players invulnerable to damage. Currently called to ensure
    /// players can't die by projectiles when stuck in a dialogue sequence.
    /// </summary>
    public void MakePlayersInvulnerable()
    {
        if (Players.Length == 0 ) { return; }
        foreach (Player player in Players)
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
        if (Players.Length == 0 ) { return; }
        foreach (Player player in Players)
        {   
            if (player != null)
            {
                player.Health.IsInvulnerable = false;
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
        PlayerManager.instance.RespawnDeadPlayers();
    }

    /// <summary>
    /// Removes all players from the Player list. To be called when the game
    /// has ended and users are returning to menu.
    /// </summary>
    public void RemoveAllPlayers()
    {
        foreach (Player player in Players)
        {
            RemovePlayer(player.Index);
        }
    }

    /// <summary>
    /// Gets the player instance from the Players list. Collects player index 
    /// by taking the last char of the GameObject name.
    /// /// </summary>
    /// <param name="playerObject">
    /// The player GameObject corresponding with the index.
    /// </param>
    /// <returns>
    /// The Player component associated with the input object.
    /// </returns>
    public Player GetPlayerInstance(GameObject playerObject)
    {
        int playerIndex = int.Parse(playerObject.name.Substring(playerObject.name.Length - 1));
        return Players[playerIndex];
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================    
    /// <summary>
    /// Initialises the component in Awake().
    /// </summary>
    private void InitOnAwake()
    {
        SetUpSingleton();
        InitVars();
    }

    /// <summary>
    /// Using the Singleton pattern, carries object data between scenes using 
    /// by inheriting the previous PlayerManager.
    /// </summary>
    private void SetUpSingleton()
    {
        if (PlayerManager.instance == null) 
        {
            PlayerManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        if (_players == null) { _players = new Player[_maxPlayers]; }
        if (_previousActionMaps == null ) 
        { 
            _previousActionMaps = new Stack<string>[_maxPlayers];
            for (int i = 0; i < _previousActionMaps.Length; i++)
            {
                _previousActionMaps[i] = new Stack<string>();
            } 
        }
    }

    /// <summary>
    /// Adds existing players in the scene to the camera focus if they are 
    /// alive. To be called when a new scene is loaded.
    /// </summary>
    private void AddExistingPlayersToCameraFocus()
    {
        foreach (Player player in Players)
        {
            if (player != null && player.GameObject != null && player.State != PlayerState.Dead)
            {
                Camera.main.GetComponent<CameraMovement>().AddFocus(player.GameObject.transform);
            }
        }
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
    /// Stores the current action maps of the input player so that it can be
    /// returned later if necessary.
    /// </summary>
    private void StoreCurrentActionMap(GameObject playerObject)
    {
        Player player = GetPlayerInstance(playerObject);
        _previousActionMaps[player.Index].Push(player.Input.currentActionMap.name);
    }

    /// <summary>
    /// To be called when a scene has loaded.
    /// </summary>
    private void InitOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        AddExistingPlayersToCameraFocus();
    }
    #endregion
}
}