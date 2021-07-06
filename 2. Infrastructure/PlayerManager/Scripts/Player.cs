using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // For EventSystem
using UnityEngine.InputSystem; // For PlayerInput
using UnityEngine.InputSystem.UI; // InputSystemUIInputModule

namespace EGS
{
    /// <summary>
    /// This class represents a Player. Anything specific to a player can be stored
    /// here.
    /// </summary>
    public class Player
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The GameObject of the player.
        /// </summary>
        public GameObject GameObject { get; set; }
        /// <summary>
        /// The health component of the player.
        /// </summary>
        public PlayerHealth Health { get; set; }
        /// <summary>
        /// The PlayerInput of the player.
        /// </summary>
        public PlayerInput Input { get; set; }
        /// <summary>
        /// The index of the player.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// The state of the player.
        /// </summary>
        public PlayerState State { get; set; }
        /// <summary>
        /// The crosshair used by the player.
        /// </summary>
        public Crosshair Crosshair { get; set; }
        /// <summary>
        /// The camera used by the player.
        /// </summary>
        public GameObject Camera { get; set; }
        #endregion

        //=====================================================================
        #region Constructors
        //=====================================================================
        /// <summary>
        /// Constructor for a Player object. Initializes properties and UI.
        /// </summary>
        /// <remarks>
        /// PlayerConfig does not inherit from MonoBehaviour because it requires
        /// a constructor. Therefore, MonoBehaviour methods like Destroy() do not
        /// work on this class. This could be implemented with MonoBehaviour class
        ///
        /// </remarks>
        /// <param name="pi">
        /// The PlayerInput of the player.
        /// </param>
        /// <param name="ui">
        /// The info UI GameObject of the player.
        /// </param>
        public Player(PlayerInput pi)
        {
            InitProperties(pi);
            InitGameObject();
            UIManager.Instance.HUD.ActivatePlayerCard(this.Index, this.Health, this.GameObject.GetComponentInChildren<WeaponSelector>());
        }
        #endregion

        //=====================================================================
        #region Deconstruction
        //=====================================================================
        /// <summary>
        /// Disables and unlinks the info UI of the player. Used when player
        /// leaves.
        /// </summary>
        public void Terminate()
        {
            UIManager.Instance.HUD.DeactivatePlayerCard(this.Index);
            RemoveFromCameraFocus();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        /// <summary>
        /// Initializes the properties of the PlayerConfig and changes the name of
        /// the player GameObject to "Player[index]".
        /// </summary>
        private void InitProperties(PlayerInput pi)
        {
            // Initialize properties
            this.GameObject = pi.transform.gameObject;
            this.Health = GameObject.GetComponent<PlayerHealth>();
            this.Input = pi;
            this.Index = pi.playerIndex;
            this.State = PlayerState.Default;
            this.Camera = GameObject.Find("MainCamera");
            InitCrosshair();
        }

        /// <summary>
        /// Initializes the player's crosshair.
        /// </summary>
        private void InitCrosshair()
        {
            // Initialize crosshair component
            GameObject crosshairObject = GameObject.Instantiate((GameObject)Resources.Load("PlayerCrosshair"));
            crosshairObject.name = "Player " + this.Index + " Crosshair";
            this.Crosshair = crosshairObject.GetComponent<Crosshair>();
            // Set crosshair gameobject instance as child of HUD
            crosshairObject.transform.SetParent(UIManager.Instance.HUD.transform);
        }

        /// <summary>
        /// Initializes the GameObject of the player.
        /// </summary>
        private void InitGameObject()
        {
            this.GameObject.name = "Player" + this.Index;
            AddToCameraFocus();
        }

        /// <summary>
        /// Adds the player to the main camera's focus if in a Game scene.
        /// </summary>
        private void AddToCameraFocus()
        {
            if (Camera != null && Camera.GetComponent<LARSCameraMovement>() != null)
            {
                Camera.GetComponent<LARSCameraMovement>().AddFocus(this.GameObject.transform);
            }
        }

        /// <summary>
        /// Removes player from the camera focus.
        /// </summary>
        private void RemoveFromCameraFocus()
        {
            if (Camera != null && Camera.GetComponent<LARSCameraMovement>() != null)
            {
                Camera.GetComponent<LARSCameraMovement>().RemoveFocus(this.GameObject.transform);
            }
        }
        #endregion
    }
}
