using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace EGS
{
    /// <summary>
    ///
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private MainMenu _mainMenu = default;
        [SerializeField] private PauseMenu _pauseMenu = default;
        [SerializeField] private HUD _hud = default;
        [SerializeField] private DialogueCanvas _dialogueCanvas = default;
        [SerializeField] private LoadingScreen _loadingScreen = default;
        [SerializeField] private EventSystem _eventSystem = default;
        [SerializeField] private Camera _dummyCamera = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        public HUD HUD
        {
            get { return _hud; }
            private set { _hud = value; }
        }

        public DialogueCanvas DialogueCanvas
        {
            get { return _dialogueCanvas; }
            private set { _dialogueCanvas = value; }
        }
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        private void Start()
        {
            InitOnStart();
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
        #region Initialization
        //=====================================================================
        private void InitOnStart()
        {
            InitVars();
            CheckMandatoryComponents();
            ActivateAppropriateCanvas();
        }

        private void InitVars()
        {
            EventSystem.current = _eventSystem;
        }

        private void CheckMandatoryComponents()
        {
            // Assert.IsNotNull(_dummyCamera, gameObject.name + " is missing _dummyCamera");
        }

        private void ActivateAppropriateCanvas()
        {
            if (GameManager.Instance.CurrentSceneName == SceneName.BOOT)
            {
                _mainMenu.gameObject.SetActive(true);
                _dummyCamera.gameObject.SetActive(true);
            }
            else
            {
                _hud.gameObject.SetActive(true);
            }
        }
        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.FadeCompleteEvent, HandleFadeCompleteEvent);
            EventManager.Instance.AddListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.FadeCompleteEvent, HandleFadeCompleteEvent);
            EventManager.Instance.RemoveListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void HandleFadeCompleteEvent(object invoker, System.EventArgs e)
        {
            FadeCompleteEventArgs args = (FadeCompleteEventArgs)e;
            // If fading back into boot menu, unload other scenes when fade is complete
            if (!args.FadeOut)
            {
                if (GameManager.Instance.CurrentSceneName != SceneName.BOOT)
                {
                    _dummyCamera.gameObject.SetActive(true);
                }
            }
            else
            {
                _mainMenu.gameObject.SetActive(false);
            }
        }

        private void HandleGameStateUpdatedEvent(object invoker, System.EventArgs e)
        {
            GameStateUpdatedEventArgs args = (GameStateUpdatedEventArgs)e;
            // If state changed to pause, open pause canvas
            if (args.NewState == GameManager.GameState.PAUSED)
            {
                _pauseMenu.gameObject.SetActive(true);
                Debug.Log("UIManager.HandleGameStateUpdatedEvent: Opening pause canvas.");
            }
            // If state changed from pause, close pause canvas
            else if (args.PrevState == GameManager.GameState.PAUSED)
            {
                _pauseMenu.gameObject.SetActive(false);
                Debug.Log("UIManager.HandleGameStateUpdatedEvent: Closing pause canvas.");
            }
            // If state changed to pregame, open main menu canvas
            else if (args.NewState == GameManager.GameState.PREGAME)
            {
                _mainMenu.gameObject.SetActive(true);
            }
            else if (args.NewState == GameManager.GameState.RUNNING)
            {
                _hud.gameObject.SetActive(true);
                _loadingScreen.gameObject.SetActive(false);
            }
            else if (args.NewState == GameManager.GameState.LOADING)
            {
                _loadingScreen.gameObject.SetActive(true);
            }
        }
        #endregion

        //=====================================================================
        #region Canvas instantiation
        //=====================================================================
        public void InstantiateCanvas(GameObject canvasPrefab)
        {
            GameObject instance = Instantiate(canvasPrefab);
            instance.transform.SetParent(UIManager.Instance.transform);
        }
        #endregion

        //=====================================================================
        #region Dummy camera
        //=====================================================================
        public void SetDummyCameraActive(bool active)
        {
            _dummyCamera.gameObject.SetActive(active);
        }
        #endregion
    }
}
