using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace EGS
{
    /// <summary>
    /// This class is responsible for adding functionality to the the main menu
    /// canvas.
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Default button selected upon opening menu.
        /// </summary>
        /// <remarks>
        /// A default button needs to be selected since the menu may be controlled
        /// with the keys of a keyboard or a gamepad (not just clicked).
        /// </remarks>
        [SerializeField] private GameObject _defaultButton = default;

        //=====================================================================
        // [Header("Animation")]
        [SerializeField] private Animation _mainMenuAnimator = default;
        [SerializeField] private AnimationClip _fadeOutAnimation = default;
        [SerializeField] private AnimationClip _fadeInAnimation = default;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void Start()
        {
            InitOnStart();
        }

        private void OnEnable()
        {
            RegisterAsListener();
            SetDefaultSelection();
            FadeIn();
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
        }

        private void InitVars()
        {
            _mainMenuAnimator = GetComponent<Animation>();
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_defaultButton, gameObject.name + " is missing _defaultButton");
            Assert.IsNotNull(_mainMenuAnimator, gameObject.name + " is missing _mainMenuAnimator");
            Assert.IsNotNull(_fadeOutAnimation, gameObject.name + " is missing _fadeOutAnimation");
            Assert.IsNotNull(_fadeInAnimation, gameObject.name + " is missing _fadeInAnimation");
        }

        /// <summary>
        /// Sets a default selected button such that controllers can navigate the
        /// menu.
        /// </summary>
        private void SetDefaultSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_defaultButton);
        }
        #endregion

        //=====================================================================
        #region Event invokers
        //=====================================================================
        private void InvokeFadeCompleteEvent(bool fadeOut)
        {
            Debug.Log("MainMenu.InvokeFadeCompleteEvent: Fade" +
            (fadeOut ? " out " : " in ") + "complete.");
            EventManager.Instance.Invoke(EventName.FadeCompleteEvent,
                new FadeCompleteEventArgs(fadeOut),
                this);
        }

        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void HandleGameStateUpdatedEvent(object invoker, System.EventArgs e)
        {
            GameStateUpdatedEventArgs args = (GameStateUpdatedEventArgs)e;
            // If leaving pregame to running, fade out
            if (args.PrevState == GameManager.GameState.PREGAME && args.NewState == GameManager.GameState.RUNNING)
            {
                FadeOut();
                Debug.Log("MainMenu.HandleGameStateUpdatedEvent: Fading out.");
            }
        }
        #endregion

        //=====================================================================
        #region Button functionality
        //=====================================================================
        /// <summary>
        /// To be called when the start button is pressed. Loads the first scene.
        /// </summary>
        public void HandleStartButtonPress()
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.PREGAME)
            {
                GameManager.Instance.LoadScene(SceneName.ROADTRIP);
            }
        }

        /// <summary>
        /// To be called when the quit button is pressed. Closes application
        /// (does not work in editor).
        /// </summary>
        public void HandleQuitButtonPress()
        {
            Application.Quit();
        }
        #endregion

        //=====================================================================
        #region Animation
        //=====================================================================
        /// <summary>
        /// Plays the fade in animation.
        /// </summary>
        public void FadeIn()
        {
            _mainMenuAnimator.Stop();
            _mainMenuAnimator.clip = _fadeInAnimation;
            _mainMenuAnimator.Play();
        }

        /// <summary>
        /// Called when fade in animation complete.
        /// </summary>
        public void OnFadeInComplete()
        {
            InvokeFadeCompleteEvent(false);
            UIManager.Instance.SetDummyCameraActive(true);
        }

        /// <summary>
        /// Plays the fade out animation.
        /// </summary>
        public void FadeOut()
        {
            UIManager.Instance.SetDummyCameraActive(false);
            _mainMenuAnimator.Stop();
            _mainMenuAnimator.clip = _fadeOutAnimation;
            _mainMenuAnimator.Play();
        }

        /// <summary>
        /// Called when fade out animation complete.
        /// </summary>
        public void OnFadeOutComplete()
        {
            InvokeFadeCompleteEvent(true);
        }
        #endregion
    }
}
