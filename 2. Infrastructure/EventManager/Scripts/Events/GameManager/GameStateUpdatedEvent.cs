using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Event to be called when the game state is updated.
    /// </summary>
    public static class GameStateUpdatedEvent
    {
        // Event handler delegate
        public static event EventHandler<EventArgs> GameStateUpdatedEventHandler;
        // Getter
        public static EventHandler<EventArgs> GetEventHandler()
        {
            return GameStateUpdatedEventHandler;
        }
    }

    /// <summary>
    /// The arguments for a GameStateUpdatedEvent.
    /// </summary>
    public class GameStateUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The new game state.
        /// </summary>
        public GameManager.GameState NewState { get; set; }
        /// <summary>
        /// The previous game state.
        /// </summary>
        public GameManager.GameState PrevState { get; set; }
        /// <summary>
        /// The GameObject of the initiating player (optional).
        /// </summary>
        /// <value></value>
        public GameObject Initiator { get; set; }
        // Constructor
        public GameStateUpdatedEventArgs(GameManager.GameState newState, GameManager.GameState prevState, GameObject initiator = null)
        {
            this.NewState = newState;
            this.PrevState = prevState;
            this.Initiator = initiator;
        }
    }
}
