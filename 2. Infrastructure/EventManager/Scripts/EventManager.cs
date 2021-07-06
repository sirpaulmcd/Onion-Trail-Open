using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EGS
{
    /// <summary>
    /// Manages event invoking. Other objects can use this objects to invoke or
    /// listen to events.
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Dictionary of EventHandler delegates with EventName keys. Any events
        /// within this dictionary can be invoked.
        /// </summary>
        private Dictionary<EventName, EventHandler<EventArgs>> _invokableEvents;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        protected override void Awake()
        {
            base.Awake();
            Init();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        public void Init()
        {
            _invokableEvents = new Dictionary<EventName, EventHandler<EventArgs>>();
            // Game manager
            _invokableEvents.Add(EventName.GameStateUpdatedEvent, GameStateUpdatedEvent.GetEventHandler());
            // Health system
            _invokableEvents.Add(EventName.HealthUpdatedEvent, HealthUpdatedEvent.GetEventHandler());
            // LifeAndRespawn system
            _invokableEvents.Add(EventName.DeathEvent, DeathEvent.GetEventHandler());
            _invokableEvents.Add(EventName.GameOverEvent, GameOverEvent.GetEventHandler());
            _invokableEvents.Add(EventName.LivesUpdatedEvent, LivesUpdatedEvent.GetEventHandler());
            // OregonTrail system
            _invokableEvents.Add(EventName.FoodUpdatedEvent, FoodUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.FuelUpdatedEvent, FuelUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.MedUpdatedEvent, MedUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.ObjectiveCompleteEvent, ObjectiveCompleteEvent.GetEventHandler());
            // Player Management
            _invokableEvents.Add(EventName.PlayerAddedEvent, PlayerAddedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.PlayerRemovedEvent, PlayerRemovedEvent.GetEventHandler());
            // UI manager
            _invokableEvents.Add(EventName.FadeCompleteEvent, FadeCompleteEvent.GetEventHandler());
            // WaveSurvival System
            _invokableEvents.Add(EventName.MobDeathEvent, MobDeathEvent.GetEventHandler());
            _invokableEvents.Add(EventName.MobsUpdatedEvent, MobsUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.SkipPrepTimeEvent, SkipPrepTimeEvent.GetEventHandler());
            _invokableEvents.Add(EventName.TimerUpdatedEvent, TimerUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.WaveCompleteEvent, WaveCompleteEvent.GetEventHandler());
            _invokableEvents.Add(EventName.WaveNumberUpdatedEvent, WaveNumberUpdatedEvent.GetEventHandler());
            _invokableEvents.Add(EventName.WaveStateUpdatedEvent, WaveStateUpdatedEvent.GetEventHandler());
            // WarAndPeace System
            _invokableEvents.Add(EventName.WeaponSwitchEvent, WeaponSwitchEvent.GetEventHandler());
            _invokableEvents.Add(EventName.WeaponAmmoEvent, WeaponAmmoEvent.GetEventHandler());
            // Example event
            _invokableEvents.Add(EventName.SpacePressedEvent, SpacePressedEvent.GetEventHandler());
        }
        #endregion

        //=====================================================================
        #region Listener pairing
        //=====================================================================
        /// <summary>
        /// Adds a listener to the named event if the named event exists in the
        /// invokableEvents dictionary.
        /// </summary>
        /// <param name="eventName">The EventName of the event.</param>
        /// <param name="listenerDel">The EventHandler delegate of the listener.</param>
        public void AddListener(EventName eventName, EventHandler<EventArgs> listenerDel)
        {
            if (_invokableEvents.ContainsKey(eventName))
            {
                _invokableEvents[eventName] += listenerDel;
            }
        }

        /// <summary>
        /// Removes a listener from the named event if the named event exists in
        /// the invokableEvents dictionary.
        /// </summary>
        /// <param name="eventName">The EventName of the event.</param>
        /// <param name="listenerDel">The EventHandler delegate of the listener.</param>
        public void RemoveListener(EventName eventName, EventHandler<EventArgs> listenerDel)
        {
            if (_invokableEvents.ContainsKey(eventName))
            {
                _invokableEvents[eventName] -= listenerDel;
            }
        }
        #endregion

        //=====================================================================
        #region Event invoking
        //=====================================================================
        /// <summary>
        /// Invokes the input event with the input arguments.
        /// </summary>
        /// <param name="eventName">The EventName of the event.</param>
        /// <param name="args">The EventArgs of the event.</param>
        /// <param name="invoker">The invoker of the event (optional).</param>
        public void Invoke(EventName eventName, EventArgs args, System.Object invoker = null)
        {
            _invokableEvents[eventName]?.Invoke(invoker, args);
        }
        #endregion
    }

    /// <summary>
    /// The names of the events in the game.
    /// </summary>
    public enum EventName
    {
        // Game Manager
        GameStateUpdatedEvent,
        // Health System
        HealthUpdatedEvent,
        // Life And Respawn System
        DeathEvent,
        GameOverEvent,
        LivesUpdatedEvent,
        // Oregon Trail System
        FoodUpdatedEvent,
        FuelUpdatedEvent,
        MedUpdatedEvent,
        ObjectiveCompleteEvent,
        // Player Management
        PlayerAddedEvent,
        PlayerRemovedEvent,
        // UI Manager
        FadeCompleteEvent,
        // WaveSurvivalSystem
        MobDeathEvent,
        MobsUpdatedEvent,
        SkipPrepTimeEvent,
        TimerUpdatedEvent,
        WaveCompleteEvent,
        WaveNumberUpdatedEvent,
        WaveStateUpdatedEvent,
        // WarAndPeaceSystem
        WeaponSwitchEvent,
        WeaponAmmoEvent,
        // Event (used for EventManager example)
        SpacePressedEvent,
    }
}
