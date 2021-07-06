using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    ///
    /// Can start next wave and detect when a wave has ended. Manages time
    /// before, after, and in-between waves.
    /// </summary>
    public class WaveSystemGameSession : Singleton<WaveSystemGameSession>
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The waves associated with a wave survival event.
        /// </summary>
        [SerializeField] private WaveSurvivalSequence _waveSurvivalSequence = default;
        /// <summary>
        /// The current wave state of the wave survival game session.
        /// </summary>
        private WaveState _currentWaveState = WaveState.GAMEOVER;
        /// <summary>
        /// The number of seconds players have for prep between waves.
        /// </summary>
        [SerializeField] private int _prepTimeSeconds = 3; //180
        /// <summary>
        /// The number of seconds counting down into starting a new wave.
        /// </summary>
        [SerializeField] private int _waveStartCountdownSeconds = 5;
        /// <summary>
        /// The number of seconds the wave summary is shown to players before
        /// returning to prep time.
        /// </summary>
        [SerializeField] private int _waveSummarySeconds = 5;
        /// <summary>
        /// The number of remaining seconds in any given countdown.
        /// </summary>
        private int _remainingSeconds = 0;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The current wave state of the wave survival game session.
        /// </summary>
        public WaveState CurrentWaveState
        {
            get => _currentWaveState;
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void Start()
        {
            StartPrepTime();
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
        private void InvokeWaveStateUpdatedEvent(WaveState newState, WaveState prevState)
        {
            Debug.Log("WaveSystemGameSession.InvokeWaveStateUpdatedEvent: Wave state " +
            "changed from " + prevState + " to " + newState + ".");
            EventManager.Instance.Invoke(
                EventName.WaveStateUpdatedEvent,
                new WaveStateUpdatedEventArgs(newState, prevState),
                this);
        }

        private void InvokeTimerUpdatedEvent()
        {
            Debug.Log("WaveSystemGameSession.InvokeTimerUpdatedEvent: Timer has changed");
            EventManager.Instance.Invoke(
                EventName.TimerUpdatedEvent,
                new TimerUpdatedEventArgs(CurrentWaveState, _remainingSeconds % 60, _remainingSeconds / 60),
                this);
        }
        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.WaveCompleteEvent, HandleWaveCompleteEvent);
            EventManager.Instance.AddListener(EventName.SkipPrepTimeEvent, HandleSkipPrepTimeEvent);
            EventManager.Instance.AddListener(EventName.GameOverEvent, HandleGameOverEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.WaveCompleteEvent, HandleWaveCompleteEvent);
            EventManager.Instance.RemoveListener(EventName.SkipPrepTimeEvent, HandleSkipPrepTimeEvent);
            EventManager.Instance.RemoveListener(EventName.GameOverEvent, HandleGameOverEvent);
        }

        private void HandleWaveCompleteEvent(object invoker, System.EventArgs e)
        {
            WaveCompleteEventArgs args = (WaveCompleteEventArgs)e;
            Debug.Log("WaveSystemGameSession.HandleWaveCompleteEvent: Starting prep time or ending game.");
            EndCurrentWave();
        }

        private void HandleSkipPrepTimeEvent(object invoker, System.EventArgs e)
        {
            SkipPrepTimeEventArgs args = (SkipPrepTimeEventArgs)e;
            Debug.Log("WaveSystemGameSession.HandleSkipPrepTimeEvent: Skipping prep time.");
            _remainingSeconds = 0;
        }

        private void HandleGameOverEvent(object invoker, System.EventArgs e)
        {
            GameOverEventArgs args = (GameOverEventArgs)e;
            Debug.Log("WaveSystemGameSession.HandleGameOverEvent: Ending game.");
            EndGameOnLoss();
        }
        #endregion

        //=====================================================================
        #region Wave state management
        //=====================================================================
        private void StartPrepTime()
        {
            UpdateState(WaveState.PREP);
            StartCoroutine(PrepTime());
        }

        private IEnumerator PrepTime()
        {
            _remainingSeconds = _prepTimeSeconds;
            InvokeTimerUpdatedEvent();
            while (_remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1);
                _remainingSeconds -= 1;
                InvokeTimerUpdatedEvent();
            }
            CountPlayersIn();
        }

        private void CountPlayersIn()
        {
            UpdateState(WaveState.STARTING);
            StartCoroutine(WaveStartCountdown());
        }

        private IEnumerator WaveStartCountdown()
        {
            _remainingSeconds = _waveStartCountdownSeconds;
            InvokeTimerUpdatedEvent();
            while (_remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1);
                _remainingSeconds -= 1;
                InvokeTimerUpdatedEvent();
            }
            StartNextWave();
        }

        private void StartNextWave()
        {
            UpdateState(WaveState.LIVE);
            _waveSurvivalSequence.StartNextWave();
        }

        private void EndCurrentWave()
        {
            UpdateState(WaveState.ENDING);
            // Show wave complete canvas
            StartCoroutine(ShowWaveSummary());
        }

        private IEnumerator ShowWaveSummary()
        {
            // Show wave complete canvas
            _remainingSeconds = _waveSummarySeconds;
            while (_remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1);
                _remainingSeconds -= 1;
                InvokeTimerUpdatedEvent();
            }
            if (!_waveSurvivalSequence.SequenceComplete) { StartPrepTime(); }
            else { EndGameOnVictory(); }
        }

        public void EndGameOnLoss()
        {
            UpdateState(WaveState.GAMEOVER);
            // Show gameover canvas
        }

        public void EndGameOnVictory()
        {
            UpdateState(WaveState.VICTORY);
            // Show victory canvas
        }
        #endregion

        //=====================================================================
        #region State management
        //=====================================================================
        public enum WaveState
        {
            PREP,
            STARTING,
            LIVE,
            ENDING,
            VICTORY,
            GAMEOVER
        }

        private void UpdateState(WaveState newState)
        {
            if (newState == _currentWaveState) { return; }
            WaveState prevState = _currentWaveState;
            _currentWaveState = newState;
            switch (_currentWaveState)
            {
                case WaveState.PREP:
                    break;
                case WaveState.STARTING:
                    break;
                case WaveState.LIVE:
                    break;
                case WaveState.ENDING:
                    break;
                case WaveState.VICTORY:
                    break;
                case WaveState.GAMEOVER:
                    break;
                default:
                    break;
            }
            InvokeWaveStateUpdatedEvent(newState, prevState);
        }
        #endregion
    }
}
