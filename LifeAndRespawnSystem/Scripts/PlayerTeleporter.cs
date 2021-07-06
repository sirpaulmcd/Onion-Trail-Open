using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class is used to manage teleporting the player after they have died or
    /// have fallen off an edge. If _periodicallyUpdateLocation is true, this class
    /// continually updates the cliff respawn location of a player such that they
    /// can teleport to stable ground after they fall off of the map. Otherwise,
    /// the teleportation location is chosen on a case-by-case basis using the
    /// Teleport(Vector3) method.
    /// </summary>
    public class PlayerTeleporter : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Boolean used to indicate if the teleport location of the player
        /// should be periodically updated.
        /// </summary>
        [SerializeField] private bool _periodicallyUpdateLocation = true;

        /// <summary>
        /// The time between updating teleport location.
        /// </summary>
        [SerializeField] private int _teleportLocationUpdateSeconds = 2;

        /// <summary>
        /// The delay between the teleport method being called and when the player
        /// is actually teleported.
        /// </summary>
        [SerializeField] private int _teleportDelaySeconds = 1;

        /// <summary>
        /// Height above teleport position that a player will teleport. Used to
        /// ensure the player is not teleported half way in the floor.
        /// </summary>
        [SerializeField] private float _heightOffset = 1;

        //=====================================================================
        // [Header("Components")]
        /// <summary>
        /// The SphereCaster used to update cliff respawn location on solid ground.
        /// </summary>
        private SphereCaster _sphereCaster;

        /// <summary>
        /// The PlayerController used to change the player input map and access the
        /// _maxFloorDistance variable.
        /// </summary>
        private PlayerController _playerController;

        /// <summary>
        /// The PlayerStats component of the player.
        /// </summary>
        private PlayerStats _playerStats;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The position where a player is to be teleported.
        /// </summary>
        private Vector3 TeleportLocation { get; set; }

        /// <summary>
        /// Boolean used to indicate if the teleport location of the player
        /// should be periodically updated. If set to true, starts coroutine that
        /// routinely updates the teleport location. Stops if set to false.
        /// </summary>
        private bool PeriodicallyUpdateLocation
        {
            get { return _periodicallyUpdateLocation; }
            set { _periodicallyUpdateLocation = value; }
        }
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
        /// Teleports the player to the input position.
        /// </summary>
        /// <param name="position">
        /// Vector3 corresponding with the desired teleportation position.
        /// </param>
        public void Teleport(Vector3 position)
        {
            this.TeleportLocation = position;
            TeleportPlayer();
        }

        /// <summary>
        /// Teleports the player to the teleport location after a delay. Wrapper
        /// method for WaitAndTeleport(). Used for respawning to ensure players
        /// do not respawn immediately after they go out of bounds.
        /// </summary>
        public void DelayedTeleport()
        {
            StartCoroutine(WaitAndTeleport());
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
            CheckMandatoryComponents();
            StartCoroutine(ContinuallyUpdateLocation());
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _sphereCaster = GetComponent<SphereCaster>();
            _playerController = GetComponent<PlayerController>();
            _playerStats = GetComponent<PlayerStats>();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_sphereCaster, gameObject.name + " is missing _sphereCaster");
            Assert.IsNotNull(_playerController, gameObject.name + " is missing _playerController");
        }

        /// <summary>
        /// Periodically updates the teleport location of the player if
        /// _periodicallyUpdateLocation is set to true. Location is updated as long
        /// as the player is standing on a GameObject with the tag "Ground".
        /// </summary>
        /// <returns>
        /// IEnumerator associated with the coroutine.
        /// </returns>
        private IEnumerator ContinuallyUpdateLocation()
        {
            while (_periodicallyUpdateLocation)
            {
                Vector3? updatedLocation = GetCurrentGroundedLocation();
                if (updatedLocation != null)
                {
                    this.TeleportLocation = (Vector3)updatedLocation +
                        new Vector3(0, _heightOffset, 0);
                }
                yield return new WaitForSeconds(_teleportLocationUpdateSeconds);
            }
        }

        /// <summary>
        /// Returns the current position of the player if they are standing on a
        /// GameObject with the "Ground" tag.
        /// </summary>
        /// <returns>
        /// Current position of player if they are standing on a GameObject with
        /// the "Ground" tag. Otherwise, returns null.
        /// </returns>
        /// <remarks>
        /// Note that the '?' after the Vector3 type allows the non-nullable
        /// Vector3 object to be temporarily set to null and then typecast
        /// back into a normal Vector3 later.
        /// <remarks>
        private Vector3? GetCurrentGroundedLocation()
        {
            GameObject floor = _sphereCaster.SphereCast(
                Vector3.down, _playerController.FloorMaxDistance);
            if (floor != null && floor.CompareTag("Ground"))
            { return gameObject.transform.position; }
            return null;
        }

        /// <summary>
        /// The default coroutine to teleport a living player. Waits
        /// _teleportDelaySeconds, then teleports the player.
        /// </summary>
        /// <returns>
        /// IEnumerator associated with the coroutine.
        /// </returns>
        private IEnumerator WaitAndTeleport()
        {
            yield return new WaitForSeconds(_teleportDelaySeconds);
            TeleportPlayer();
            _playerStats.IsIncapacitated = false;
        }

        /// <summary>
        /// Teleports the player GameObject to the teleport location.
        /// </summary>
        private void TeleportPlayer()
        {
            gameObject.transform.position = this.TeleportLocation;
        }
        #endregion
    }
}
