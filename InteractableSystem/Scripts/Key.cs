using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    public class Key : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// A Vector that is set to the initial position of the key. This will be
        /// used to return the key here if it is dropped.
        /// <summary>
        private Vector3 _startPosition;

        /// <summary>
        /// The distance the key can be moved from initial spawn without resetting.
        /// Calculated as a linear distace from the starting position.
        /// <summary>
        [SerializeField] private int _tolerance = 1;

        /// <summary>
        /// A boolean that will cause the key to reset if outside the tolerance
        /// limits when true.
        /// <summary>
        [SerializeField] private bool _resetOnDrop = false;

        /// <summary>
        /// The Liftable component required to lift the key.
        /// </summary>
        private Liftable _liftable;

        /// <summary>
        /// The Rigidbody component required to interact with physics.
        /// </summary>
        private Rigidbody _rigidbody;
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

        /// <summary>
        /// Called when this collider/rigidbody has begun touching another
        /// rigidbody/collider.
        /// </summary>
        /// <param name="collision">
        /// Collision holding information about colliding object.
        /// </param>
        private void OnCollisionEnter(Collision collision)
        {
            AttemptKeyLocationReset(collision);
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
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _liftable = GetComponent<Liftable>();
            _rigidbody = GetComponent<Rigidbody>();
            _startPosition = this.transform.position;
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_liftable, gameObject.name + " is missing _liftable");
            Assert.IsNotNull(_rigidbody, gameObject.name + " is missing _rigidbody");
        }

        /// <summary>
        /// Checks a gameObject to see if it has a Jumpable script.
        /// </summary>
        /// <param name="gameObject">
        /// The GameObject that is being checked for the jumpable component.
        /// </param>
        /// <returns>
        /// Returns true if there is a Jumpable component, false otherwise.
        /// </returns>
        private bool IsJumpable(GameObject gameObject)
        {
            return gameObject.GetComponent<Jumpable>();
        }

        /// <summary>
        /// Determines the distance between Position1 and startingPosition and
        /// checks if it is within the tolerances.
        /// </summary>
        /// <returns>
        /// Returns true if distance is greater than or equal to the tolerance
        /// limit and false otherwise.
        /// </returns>
        private bool CheckTolerances()
        {
            Vector3 distanceVector = this.transform.position - _startPosition;
            return distanceVector.magnitude >= _tolerance;
        }

        /// <summary>
        /// Checks to see if the gameObject is currently being lifted.
        /// </summary>
        private bool IsLifted()
        {
            return this.gameObject.GetComponent<Liftable>().IsLifted;
        }

        /// <summary>
        /// Teleports the key back to its starting position. Used when the key is
        /// dropped.
        /// </summary>
        private void TeleportToStartPosition()
        {
            this.gameObject.SetActive(false);
            this.transform.position = _startPosition;
            this.transform.rotation = Quaternion.identity;
            _rigidbody.freezeRotation = true;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called when the key collides with any object. Key checks to see if it
        /// should reset based on three conditions:
        /// - See isLifted()
        /// - CheckTolerances()
        /// - IsJumpable() for details
        /// </summary>
        /// <param name="collision">
        /// Collision holding information about colliding object.
        /// </param>
        private void AttemptKeyLocationReset(Collision collision)
        {
            if (IsJumpable(collision.gameObject) && !IsLifted() && CheckTolerances() && _resetOnDrop)
            {
                TeleportToStartPosition();
            }
        }
        #endregion
    }
}
