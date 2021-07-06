using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class is a sensor used to add or remove rigidbodies to a carrier.
    /// </summary>
    public class CarryRigidbodiesSensor : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The CarryRigidbodies object corresponding to the sensor.
        /// </summary>
        [SerializeField] private CarryRigidbodies _carrier = null;

        /// <summary>
        /// List of Rigidbodies currently detected by the sensor.
        /// </summary>
        [SerializeField] private List<Rigidbody> _detectedRigidbodies;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The CarryRigidbodies object corresponding to the sensor.
        /// </summary>
        public CarryRigidbodies carrier
        {
            get { return _carrier; }
            set { _carrier = value; }
        }

        /// <summary>
        /// List of Rigidbodies currently detected by the sensor.
        /// </summary>
        public List<Rigidbody> detectedRigidbodies
        {
            get { return _detectedRigidbodies; }
            private set { _detectedRigidbodies = value; }
        }
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        /// <summary>
        /// Called when trigger collider has begun touching a collider or
        /// rigidbody.
        /// </summary>
        /// <param name="other">
        /// The collider of the entering object.
        /// </param>
        private void OnTriggerEnter(Collider other)
        {
            TryAddRigidbody(other);
        }

        /// <summary>
        /// Called when trigger collider has stopped touching a collider or
        /// rigidbody.
        /// </summary>
        /// <param name="other">
        /// The collider of the exiting object.
        /// </param>
        private void OnTriggerExit(Collider other)
        {
            TryRemoveRigidbody(other);
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Checks entering collider for rigidbody. If one exists, tries to pair it
        /// with the carrier rigidbody.
        /// </summary>
        /// <param name="other">
        /// The collider entering the trigger.
        /// </param>
        public void TryAddRigidbody(Collider other)
        {
            // If rigidbody of colliding object exists and has not already been detected...
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb && !_detectedRigidbodies.Contains(rb))
            {
                // Add to sensor _detectedRigidbodies list
                _detectedRigidbodies.Add(rb);
                // Add to carrier _carriedRigidbodies list
                if (_carrier != null) { _carrier.TryAddRigidbody(rb); }
                else { Debug.LogWarning("RigidbodySensor has no assigned parent.", gameObject); }
            }
        }

        /// <summary>
        /// Checks entering collider for rigidbody. If one exists, tries to pair
        /// it with the carrier rigidbody.
        /// </summary>
        /// <param name="other">
        /// The collider exiting the trigger.
        /// </param>
        public void TryRemoveRigidbody(Collider other)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            // If rigidbody of colliding object exists and has already been detected...
            if (rb && _detectedRigidbodies.Contains(rb))
            {
                // Remove from sensor list
                _detectedRigidbodies.Remove(rb);
                // Remove from carrier list (unless detected by another sensor)
                if (_carrier != null) { _carrier.TryRemoveRigidbody(rb); }
                else { Debug.LogWarning("RigidbodySensor has no assigned parent.", gameObject); }
            }
        }
        #endregion
    }
}
