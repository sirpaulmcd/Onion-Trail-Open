using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// A generic finite state machine.
    /// </summary>
    public class FiniteStateMachine
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The current state of the finite state machine.
        /// </summary>
        public State CurrentState { get; private set; }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        /// <summary>
        /// Initializes the finite state machine with the input state.
        /// </summary>
        /// <param name="startingState">
        /// The starting state of the finite state machine.
        /// </param>
        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }
        #endregion

        //=====================================================================
        #region Changing state
        //=====================================================================
        /// <summary>
        /// Changes the state of the finite state machine to the input state.
        /// </summary>
        /// <param name="newState">
        /// The state to be changed to.
        /// </param>
        public void ChangeState(State newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
        #endregion
    }
}
