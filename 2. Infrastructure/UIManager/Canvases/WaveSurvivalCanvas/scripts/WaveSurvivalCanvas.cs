using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

namespace EGS
{
    /// <summary>
    /// This class manages WaveSurvivalCanvas responsible for displaying wave
    /// survival related information.
    /// </summary>
    public class WaveSurvivalCanvas : MonoBehaviour
    {
        //=====================================================================
        #region Inspector linked variables
        //=====================================================================
        /// <summary>
        /// The currently enabled panel. Initialized to prep time.
        /// </summary>
        private GameObject _currentPanel = default;

        //=====================================================================
        // [Header("Prep time panel")]
        /// <summary>
        /// The panel holding prep time info.
        /// </summary>
        [SerializeField] private GameObject _prepTimePanel = default;
        /// <summary>
        /// The prep time timer text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _prepTimerText = default;

        //=====================================================================
        // [Header("Wave start panel")]
        /// <summary>
        /// The panel holding wave starting info.
        /// </summary>
        [SerializeField] private GameObject _waveStartPanel = default;
        /// <summary>
        /// The wave start countdown text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _countdownNumberText = default;

        //=====================================================================
        // [Header("Wave live panel")]
        /// <summary>
        /// The panel holding live wave info.
        /// </summary>
        [SerializeField] private GameObject _waveLivePanel = default;
        /// <summary>
        /// The wave start countdown text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _waveNumberText = default;
        /// <summary>
        /// The wave start countdown text.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _mobsNumberText = default;

        //=====================================================================
        // [Header("Wave complete panel")]
        /// <summary>
        /// The panel holding wave complete info.
        /// </summary>
        [SerializeField] private GameObject _waveCompletePanel = default;

        //=====================================================================
        // [Header("Sequence victory panel")]
        /// <summary>
        /// The panel holding victory info.
        /// </summary>
        [SerializeField] private GameObject _sequenceVictoryPanel = default;

        //=====================================================================
        // [Header("Sequence failure panel")]
        /// <summary>
        /// The panel holding failure info.
        /// </summary>
        [SerializeField] private GameObject _sequenceFailurePanel = default;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void Start()
        {
            InitVars();
            CheckMandatoryComponents();
            RegisterAsListener();
        }

        private void OnDestroy()
        {
            DeregisterAsListener();
        }
        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.MobsUpdatedEvent, HandleMobsUpdatedEvent);
            EventManager.Instance.AddListener(EventName.TimerUpdatedEvent, HandleTimerUpdatedEvent);
            EventManager.Instance.AddListener(EventName.WaveNumberUpdatedEvent, HandleWaveNumberUpdatedEvent);
            EventManager.Instance.AddListener(EventName.WaveStateUpdatedEvent, HandleWaveStateUpdatedEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.MobsUpdatedEvent, HandleMobsUpdatedEvent);
            EventManager.Instance.RemoveListener(EventName.TimerUpdatedEvent, HandleTimerUpdatedEvent);
            EventManager.Instance.RemoveListener(EventName.WaveNumberUpdatedEvent, HandleWaveNumberUpdatedEvent);
            EventManager.Instance.RemoveListener(EventName.WaveStateUpdatedEvent, HandleWaveStateUpdatedEvent);
        }

        private void HandleMobsUpdatedEvent(object invoker, EventArgs e)
        {
            Debug.Log("WaveSurvivalCanvas.HandleMobsUpdatedEvent: Updating mob count.");
            MobsUpdatedEventArgs args = (MobsUpdatedEventArgs)e;
            _mobsNumberText.text = String.Format("{0}/{1}", args.AliveCount, args.TotalCount);
        }

        private void HandleTimerUpdatedEvent(object invoker, EventArgs e)
        {
            Debug.Log("WaveSurvivalCanvas.HandleTimerUpdatedEvent: Updating timer.");
            TimerUpdatedEventArgs args = (TimerUpdatedEventArgs)e;
            string minutes = args.Minutes + "";
            string seconds = args.Seconds + "";
            if (args.Minutes < 10) { minutes = "0" + minutes; }
            if (args.Seconds < 10) { seconds = "0" + seconds; }
            switch (args.WaveState)
            {
                case WaveSystemGameSession.WaveState.PREP:
                    _prepTimerText.text = String.Format("{0}:{1}", minutes, seconds);
                    break;
                case WaveSystemGameSession.WaveState.STARTING:
                    _countdownNumberText.text = args.Seconds + "";
                    break;
                default:
                    Debug.Log("Warning: Invalid wave state detected.");
                    break;
            }
        }

        private void HandleWaveStateUpdatedEvent(object invoker, EventArgs e)
        {
            Debug.Log("WaveSurvivalCanvas.HandleWaveStateUpdatedEvent: Switching panels.");
            WaveStateUpdatedEventArgs args = (WaveStateUpdatedEventArgs)e;
            switch (args.NewState)
            {
                case WaveSystemGameSession.WaveState.PREP:
                    SwitchPanels(_prepTimePanel);
                    break;
                case WaveSystemGameSession.WaveState.STARTING:
                    SwitchPanels(_waveStartPanel);
                    break;
                case WaveSystemGameSession.WaveState.LIVE:
                    SwitchPanels(_waveLivePanel);
                    break;
                case WaveSystemGameSession.WaveState.ENDING:
                    SwitchPanels(_waveCompletePanel);
                    break;
                case WaveSystemGameSession.WaveState.VICTORY:
                    SwitchPanels(_sequenceVictoryPanel);
                    break;
                case WaveSystemGameSession.WaveState.GAMEOVER:
                    SwitchPanels(_sequenceFailurePanel);
                    break;
                default:
                    Debug.Log("Warning: Invalid wave state detected.");
                    break;
            }
        }

        private void HandleWaveNumberUpdatedEvent(object invoker, EventArgs e)
        {
            Debug.Log("WaveSurvivalCanvas.HandleWaveNumberUpdatedEvent: Updating wave number.");
            WaveNumberUpdatedEventArgs args = (WaveNumberUpdatedEventArgs)e;
            _waveNumberText.text = args.NewNumber + "";
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private void InitVars()
        {
            if (WaveSystemGameSession.Instance != null)
            {
                _prepTimerText.text = "00:00";
                _countdownNumberText.text = "0";
                _waveNumberText.text = "0";
                _mobsNumberText.text = "0/0";
            }
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_prepTimePanel, gameObject.name + " is missing _prepTimePanel");
            Assert.IsNotNull(_prepTimerText, gameObject.name + " is missing _prepTimerText");
            Assert.IsNotNull(_waveStartPanel, gameObject.name + " is missing _waveStartPanel");
            Assert.IsNotNull(_countdownNumberText, gameObject.name + " is missing _countdownNumberText");
            Assert.IsNotNull(_waveLivePanel, gameObject.name + " is missing _waveLivePanel");
            Assert.IsNotNull(_waveNumberText, gameObject.name + " is missing _waveNumberText");
            Assert.IsNotNull(_mobsNumberText, gameObject.name + " is missing _mobsNumberText");
            Assert.IsNotNull(_waveCompletePanel, gameObject.name + " is missing _waveCompletePanel");
        }
        #endregion

        //=====================================================================
        #region Panel switching
        //=====================================================================
        private void SwitchPanels(GameObject newPanel)
        {
            if (_currentPanel != null) { _currentPanel.SetActive(false); }
            _currentPanel = newPanel;
            _currentPanel.SetActive(true);
        }
        #endregion
    }
}
