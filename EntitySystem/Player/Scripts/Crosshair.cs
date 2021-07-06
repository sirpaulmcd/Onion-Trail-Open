using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// This script is used to control the player crosshair.
    /// </summary>
    public class Crosshair : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The distance the crosshair should be from the player.
        /// </summary>
        [SerializeField] private float _distanceFromPlayer = 50;
        /// <summary>
        /// The RectTransform of the crosshair texture.
        /// </summary>
        private RectTransform _rectTransform;
        /// <summary>
        /// The CanvasRenderer of the crosshair texture.
        /// </summary>
        private CanvasRenderer _canvasRenderer;
        /// <summary>
        /// True if the crosshair is visible.
        /// </summary>
        private bool _crosshairEnabled = false;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        private void Start()
        {
            InitOnStart();
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Sets 100% alpha on crosshair (comletely visible).
        /// </summary>
        public void EnableCrosshair()
        {
            _canvasRenderer.SetAlpha(1);
            _crosshairEnabled = true;
        }

        /// <summary>
        /// Sets 0% alpha on crosshair (comletely invisible).
        /// </summary>
        public void DisableCrosshair()
        {
            _canvasRenderer.SetAlpha(0);
            _crosshairEnabled = false;
        }

        /// <summary>
        /// Sets 0% alpha on crosshair (comletely invisible).
        /// </summary>
        public void toggleCrosshair()
        {
            if (_crosshairEnabled) { DisableCrosshair(); }
            else { EnableCrosshair(); }
        }

        /// <summary>
        /// Moves crosshair when player uses mouse
        /// </summary>
        /// <param name="pos">Position input of the mouse.</param>
        public void PositionCrosshairOnMouseMove(Vector2 pos)
        {
            _rectTransform.transform.position = new Vector3(pos.x, pos.y, 0.0f);
        }

        /// <summary>
        /// Moves crosshair when player uses controller.
        /// </summary>
        /// <param name="playerPos">Position of the player.</param>
        /// <param name="pos">Position input of the mouse or right stick.</param>
        public void PositionCrosshairOnStickMove(Vector3 playerPos, Vector2 pos)
        {
            Vector2 player_screen_pos = Camera.main.WorldToScreenPoint(playerPos);
            pos = player_screen_pos + pos * _distanceFromPlayer;
            _rectTransform.transform.position = new Vector3(pos.x, pos.y, 0.0f);
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        private void InitOnStart()
        {
            InitVars();
            DisableCrosshair();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _rectTransform = GetComponentInChildren<RectTransform>();
            _canvasRenderer = GetComponentInChildren<CanvasRenderer>();
        }
        #endregion
    }
}
