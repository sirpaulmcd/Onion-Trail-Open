using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Script to open, close, and unlock doors.
    /// </summary>
    public class Door : MonoBehaviour, IInteractable
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Whether the door is locked or unlocked, default unlocked.
        /// </summary>
        [SerializeField] private bool _isLocked = false;
        /// <summary>
        /// The facing direction of the door parent gameobject.
        /// </summary>
        private Vector3 _doorFacing;
        /// <summary>
        /// What state the door is in.
        // 0 indicates closed, 1 and -1 indicate open directions.
        /// </summary>
        private int _doorState = 0;
        /// <summary>
        /// The anamation component attached to the door gameobject that renders
        /// the door (not the door frame).
        /// </summary>
        private Animation _animation;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        public void Start()
        {
            InitOnStart();
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Called when the player interacts with the object through spherecasting.
        /// </summary>
        /// <param name="interactor">
        /// The GameObject that is interacting with the door.
        /// </param>
        public void Interact(GameObject interactor)
        {
            // Player interacting with locked door.
            if (_isLocked && interactor.tag == "Player")
            {
                return; // Do nothing.
            }
            // Non-player interacting with door.
            else if (interactor.tag != "Player")
            {
                UnlockDoor(); // Toggle door to unlocked.
            }
            else // Unlocked door being interacted by player.
            {
                ToggleDoor(interactor);
            }
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
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _animation = GetComponentInChildren<Animation>();
            _doorFacing = this.gameObject.transform.right;
            // This will take the x axis vector,
            // which should always be the doors facing direction.
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_animation, gameObject.name + " is missing _animation");
        }

        /// <summary>
        /// Toggles the door open or closed based upon the players facing direction.
        /// and always closes the same way it opens.
        /// </summary>
        /// <remarks>
        /// The players facing direction is taken from the PlayerController script.
        /// A dot product is used to to calculate the cosine of the angle between the vectors.
        /// The cosine of the angle is then used to decide which way to open the door.
        /// Cosine of an angle is positive if vectors are within 90 degrees of each other.
        /// Cosine of an angle is negative if vectors are father than 90 degrees from each other.
        /// Cosine of an angle is zero if vectors are exactly 90 degree,
        //  although it shouldn't be possible.
        /// </remarks>
        /// <param name="interactor">
        /// The GameObject that is interacting with the door.
        /// </param>
        public void ToggleDoor(GameObject interactor)
        {
            // The PlayerController check is because we will likely add to the logic leading here.
            if (!_animation.isPlaying && interactor.GetComponent<PlayerController>())
            // Interactor has to have PlayerController script and the door must not be opening/closing.
            {
                float cosineAngle = Vector3.Dot(
                    interactor.GetComponent<PlayerController>().FacingDirection, _doorFacing);
                switch (_doorState)
                {
                    // Door is closed case.
                    case 0:
                        // Open door based on player facing direction.
                        if (cosineAngle > 0)
                        {
                            _animation.Play("DoorOpen1");
                            _doorState = 1;
                        }
                        // Player facing away from door.
                        else if (cosineAngle < 0)
                        {
                            _animation.Play("DoorOpen2");
                            _doorState = -1;
                        }
                        break;
                    case 1: // Door open direction 1.
                        _animation.Play("DoorClose1");
                        _doorState = 0;
                        break;
                    case -1: // Door open direction 2.
                        _animation.Play("DoorClose2");
                        _doorState = 0;
                        break;
                }
            }
        }
        /// <summary>
        /// Unlock the door if its locked, do nothing otherwise.
        /// </summary>
        public void UnlockDoor()
        {
            _isLocked = false; // Unlock the door.
        }
        #endregion
    }
}
