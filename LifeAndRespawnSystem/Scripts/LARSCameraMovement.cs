using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class controls the main camera. Currently, the camera should be tilted
    /// by 45 degrees in the X and have an FOV of 20. This may change.
    /// </summary>
    public class LARSCameraMovement : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// List of transforms that the camera is to focus on.
        /// </summary>
        [SerializeField] private List<Transform> _focuses;
        /// <summary>
        /// The distance from the camera to the focus point.
        /// </summary>
        private float _cameraOffset = 30f;
        /// <summary>
        /// The current velocity of the camera. Used for smooth camera movement.
        /// </summary>
        private Vector3 _velocity;
        /// <summary>
        /// The approximate time it will take to reach the target. A larger value
        /// will reach the target faster.
        /// </summary>
        [SerializeField] private float _moveSpeed = 4.0f;
        /// <summary>
        /// The approximate time it will take to reach the desired zoom. A larger value
        /// will reach the target faster.
        /// </summary>
        [SerializeField] private float _zoomSpeed = 2.0f;
        /// <summary>
        /// The minimum value for _cameraOffset.
        /// </summary>
        [SerializeField] private float _minOffset = 30f;
        /// <summary>
        /// The maximum value for _cameraOffset.
        /// </summary>
        [SerializeField] private float _maxOffset = 50f;
        /// <summary>
        /// The number of player priority focuses in the scene
        /// </summary>
        private int _priorityCount = 0;
        /// <summary>
        /// Distance outside of the player bounding box that objects of interest are dynamically added/removed from focus.
        /// </summary>
        [SerializeField] private float _extraBoundingBoxSize = 20f;
        /// <summary>
        /// Where the camera focused in the previous frame
        /// </summary>
        private Vector3 _previousCenter;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            InitOnAwake();
        }

        /// <summary>
        /// Called after all Update functions have been called.
        /// </summary>
        void LateUpdate()
        {
            // Keep camera where it currently is if no center point
            if (_focuses.Count == 0) { return; }
            // If camera has one or more focuses, make bounding box
            Bounds bounds = GetBounds();
            SmoothlyMoveCamera(bounds.center);
            SmoothlyZoomCamera(Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z));
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Adds a focus to the camera.
        /// </summary>
        /// <param name="transform">
        /// Object transform to be framed inside the camera view.
        /// </param>
        public void AddFocus(Transform transform)
        {
            if (_focuses.Contains(transform)) { return; }
            if (transform.gameObject.CompareTag("Player"))
            {
                _focuses.Insert(_priorityCount, transform);
                _priorityCount++;
            }
            else
            {
                _focuses.Add(transform);
            }
        }

        /// <summary>
        /// Removes a focus from the camera.
        /// </summary>
        /// <param name="transform">
        /// Object transform to be neglected by camera view.
        /// </param>
        public void RemoveFocus(Transform transform)
        {
            if (!_focuses.Contains(transform)) { return; }
            if (_focuses.IndexOf(transform) < _priorityCount)
            {
                _priorityCount--;
            }
            _focuses.Remove(transform);
        }

        /// <summary>
        /// Resets camera focus list.
        /// </summary>
        public void RemoveAllFocuses()
        {
            _focuses = new List<Transform>();
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Awake().
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
            this.gameObject.name = "MainCamera";
            _focuses = new List<Transform>();
            _previousCenter = new Vector3(0f, 0f, 0f);
            foreach (Player player in PlayerManager.Instance.Players)
            {
                if (player != null) { player.Camera = this.gameObject; }
            }
        }

        /// <summary>
        /// Gets the bounding box encapsulating all players
        /// and focuses within _extraBoundingBoxSize of the player bounding box
        /// </summary>
        private Bounds GetBounds()
        {
            Bounds bounds = new Bounds(_focuses[0].position, Vector3.zero);
            // Add all priorities to the bounding box
            for (int i = 0; i < _priorityCount; i++)
            {
                bounds.Encapsulate(_focuses[i].position);
            }
            float playerBBSize = bounds.size.magnitude;
            // Add focus to bounding box only if it's not too far away
            for (int i = _priorityCount; i < _focuses.Count; i++)
            {
                Bounds tmpBnd = new Bounds(bounds.center, bounds.size);
                tmpBnd.Encapsulate(_focuses[i].position);
                if (tmpBnd.size.magnitude < playerBBSize + _extraBoundingBoxSize)
                {
                    bounds = tmpBnd;
                }
            }
            return bounds;
        }

        /// <summary>
        /// Smoothly moves the camera to the center of all focuses.
        /// The focus point is lerped. The camera angle is snapped instantly.
        /// </summary>
        /// <param name="centerPoint">
        /// The center point that the camera should follow.
        /// </param>
        private void SmoothlyMoveCamera(Vector3 centerPoint)
        {
            Vector3 focusPoint = Vector3.Lerp(_previousCenter, centerPoint, Time.deltaTime * _moveSpeed);
            transform.position = focusPoint + transform.rotation * new Vector3(0, 0, -_cameraOffset);
            _previousCenter = focusPoint;
        }

        /// <summary>
        /// Smoothly zooms the camera using linear interpolation.
        /// </summary>
        /// <param name="greatestDistance">
        /// The greatest axes distance between any 2 focuses.
        /// </param>
        private void SmoothlyZoomCamera(float greatestDistance)
        {
            // Find desired camera offset
            float newOffset = Mathf.Lerp(_minOffset, _maxOffset, greatestDistance / _minOffset);
            // Smooths camera zoom
            _cameraOffset = Mathf.Lerp(_cameraOffset, newOffset, Time.deltaTime * _zoomSpeed);
        }
        #endregion
    }
}
