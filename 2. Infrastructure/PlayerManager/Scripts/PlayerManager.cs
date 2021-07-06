using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem; // For PlayerInput

namespace EGS
{
    /// <summary>
    /// This class manages the Player objects created for individual players.
    /// Upon player join/leave a Player is created/destroyed and the player
    /// GameObjects are processed accordingly. In game, it is responsible for
    /// input swaps, state swaps, etc. This component MUST be connected to the
    /// UnityEvents of Unity's PlayerInputManager component through the inspector
    /// in order to subsrcibe to its UnityEvents.
    /// </summary>
    public class PlayerManager : Singleton<PlayerManager>
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
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
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// List of Player objects currently participating in the game.
        /// </summary>
        public Player[] Players
        {
            get { return _players; }
            private set { _players = value; }
        }
        #endregion

        //=====================================================================
        #region Monobehavior
        //=====================================================================
        protected override void Awake()
        {
            base.Awake();
            InitOnAwake();
        }

        private void OnEnable()
        {
            RegisterAsListener();
        }

        private void OnDisable()
        {
            DeregisterAsListener();
        }
        #endregion

        //=====================================================================
        #region Event invokers
        //=====================================================================
        /// <summary>
        /// Invokes a PlayerAddedEvent.
        /// </summary>
        private void InvokePlayerAddedEvent(Player player)
        {
            Debug.Log("PlayerManager.InvokePlayerAddedEvent: A new player has been added to the session.");
            EventManager.Instance.Invoke(EventName.PlayerAddedEvent, new PlayerAddedEventArgs(player), this);
        }

        /// <summary>
        /// Invokes a PlayerRemovedEvent.
        /// </summary>
        private void InvokePlayerRemovedEvent()
        {
            Debug.Log("PlayerManager.InvokePlayerRemovedEvent: A player has been removed from the session.");
            EventManager.Instance.Invoke(EventName.PlayerRemovedEvent, new PlayerRemovedEventArgs(), this);
        }
        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        /// <summary>
        /// Register as listener to relevant events.
        /// </summary>
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        /// <summary>
        /// Deregister as listener to relevant events.
        /// </summary>
        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void HandleGameStateUpdatedEvent(object invoker, System.EventArgs e)
        {
            GameStateUpdatedEventArgs args = (GameStateUpdatedEventArgs)e;
            if (args.PrevState == GameManager.GameState.PAUSED || args.PrevState == GameManager.GameState.DIALOGUE)
            {
                UndoLeaderControlChange();
                Debug.Log("PlayerManager.HandleGameStateUpdatedEvent: Reverting pause leader control change.");
            }
            else if (args.NewState == GameManager.GameState.PAUSED || args.NewState == GameManager.GameState.DIALOGUE)
            {
                LeaderControlChange(args.Initiator, ActionMapName.UI);
                Debug.Log("PlayerManager.HandleGameStateUpdatedEvent: Performing UI leader control change.");
            }
        }
        #endregion

        //=====================================================================
        #region Unity Event handlers
        //=====================================================================
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
            ChangePlayerControls(pi.transform.gameObject, SceneInitializer.Instance.InitialActionMapName);
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
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        /// <summary>
        /// Initializes the component in Awake().
        /// </summary>
        private void InitOnAwake()
        {
            InitVars();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            if (_players == null) { _players = new Player[_maxPlayers]; }
            InitializeControlStack();
        }

        /// <summary>
        /// Initializes the control stack such that previous action maps can be
        /// sourced when undoing control changes. Called at the beginning of
        /// every scene.
        /// </summary>
        public void InitializeControlStack()
        {
            if (_previousActionMaps == null)
            {
                _previousActionMaps = new Stack<string>[_maxPlayers];
                for (int i = 0; i < _previousActionMaps.Length; i++)
                {
                    _previousActionMaps[i] = new Stack<string>();
                }
            }
        }
        #endregion

        //=====================================================================
        #region Player adding/removal
        //=====================================================================
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
            InvokePlayerAddedEvent(player);
        }

        /// <summary>
        /// Removes a player from the _players list.
        /// </summary>
        /// <param name="player">
        /// Player object of considered player.
        /// </param>
        public void RemovePlayer(int playerIndex)
        {
            InvokePlayerRemovedEvent();
            Players[playerIndex]?.Terminate();
            Players[playerIndex] = null;
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
        #endregion

        //=====================================================================
        #region Player state management
        //=====================================================================
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
        #endregion

        //=====================================================================
        #region Player input management
        //=====================================================================
        /// <summary>
        /// Changes all player controls to the input actionmap.
        /// </summary>
        /// <param name="actionMap">
        /// The input action map that all players will be assigned.
        /// </param>
        public void ChangeAllPlayerControls(string actionMap)
        {
            foreach (Player player in Players)
            {
                if (player != null) { ChangePlayerControls(player.GameObject, actionMap); }
            }
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
        /// Changes the controls of all players whose state is not "dead".
        /// </summary>
        /// <param name="actionMap">
        /// The name of the action map the player controls are to swap to. See
        /// ActionMapName.cs for options.
        /// </param>
        public void ChangeAllLivingPlayerControls(string actionMap)
        {
            foreach (Player player in Players)
            {
                if (player == null || player.State == PlayerState.Dead) { continue; }
                ChangePlayerControls(player.GameObject, actionMap);
            }
        }

        /// <summary>
        /// Reverts the controls of all players whose state is not "dead".
        /// </summary>
        /// <param name="actionMap">
        /// The name of the action map the player controls are to swap to. See
        /// ActionMapName.cs for options.
        /// </param>
        public void RevertAllLivingPlayerControls()
        {
            foreach (Player player in Players)
            {
                if (player == null || player.State == PlayerState.Dead) { continue; }
                RevertPlayerControls(player.GameObject);
            }
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
            if (initiatingPlayer == null) { return; }
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
        /// Stores the current action maps of the input player so that it can be
        /// returned later if necessary.
        /// </summary>
        private void StoreCurrentActionMap(GameObject playerObject)
        {
            Player player = GetPlayerInstance(playerObject);
            _previousActionMaps[player.Index].Push(player.Input.currentActionMap.name);
        }
        #endregion

        //=====================================================================
        #region Player instance getter
        //=====================================================================
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
    }
}
