using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// This class allows objects to carry other rigidbodies with them. It allows
    /// carried objects to follow the translations/rotations of the parent object.
    /// </summary>
    /// <remarks>
    /// Vanilla Unity physics does not allow GameObjects to follow the translations
    /// or rotations of the GameObjects that they are sitting on. Typical tutorials
    /// solve this by making the player a child of the platform such that the
    /// player transform moves relative to the parent platform. However, this can
    /// cause some weird glitches/complications so I've found another solution
    /// based on this tutorial: https://www.youtube.com/watch?v=PVtf3vg8BXw
    /// </remarks>
    public class CarryRigidbodies : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// List of Rigidbodies currently carried by the parent.
        /// </summary>
        [SerializeField] private List<Rigidbody> _carriedRigidbodies;

        /// <summary>
        /// The previous position of the parent.
        /// </summary>
        private Vector3 _lastPosition;

        /// <summary>
        /// The previous angle of the parent.
        /// </summary>
        private Vector3 _lastEulerAngles;

        /// <summary>
        /// The rigidbody component of the parent.
        /// </summary>
        private Rigidbody _rigidbody;

        /// <summary>
        /// Boolean used to indicate whether moving platform uses additional
        /// trigger sensors to detect rigidbodies rather than its standard box
        /// collider. Note that, if additional triggers are not used, the carrier
        /// will pair with objects that touch it's sides/bottom which is not
        /// usually ideal.
        /// </summary>
        [SerializeField] private bool _useTriggerAsSensor = false;

        /// <summary>
        /// List of CarryRigidbodiesSensors associated with the carrier.
        /// </summary>
        [SerializeField] private List<CarryRigidbodiesSensor> _sensors;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        private void Start()
        {
            InitOnStart();
        }

        /// <summary>
        /// Called after Update().
        /// </summary>
        /// <remarks>
        /// LateUpdate() is used here because it is tracking bodies that may have
        /// moved during Update().
        /// </remarks>
        private void LateUpdate()
        {
            MoveCarriedBodies();
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
            TryPairWithoutSensors(collision);
        }

        /// <summary>
        /// Called when collider or rigidbody has stopped touching another collider
        /// or rigidbody.
        /// </summary>
        /// <param name="other">
        /// The collision information passed to the event.
        /// </param>
        private void OnCollisionExit(Collision other)
        {
            TryUnpairWithoutSensors(other);
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Adds input rigidbody to _carriedRigidbodies list.
        /// </summary>
        /// <param name="rb">
        /// The rigidbody to be added to _carriedRigidbodies.
        /// </param>
        public void TryAddRigidbody(Rigidbody rb)
        {
            // If rigidbody not in _carriedRigidbodies list and is liftable/carriable...
            if (!_carriedRigidbodies.Contains(rb) && IsLiftableOrCarriable(rb))
            {
                _carriedRigidbodies.Add(rb);
            }
        }

        /// <summary>
        /// Tries to remove a rigidbody from _carriedRigidbodies. Removal is
        /// prevented as long as rigidbody is touching one or more of the carrier's
        /// sensors.
        /// </summary>
        /// <param name="rb">
        /// The rigidbody to be removed from _carriedRigidbodies.
        /// </param>
        public void TryRemoveRigidbody(Rigidbody rb)
        {
            // If rigidbody is not detected by any of carrier's sensors and the
            // rigidbody is currently being carried...
            if (!IsDetected(rb) && _carriedRigidbodies.Contains(rb))
            {
                // Remove from list
                _carriedRigidbodies.Remove(rb);
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
            SetSensors();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _carriedRigidbodies = new List<Rigidbody>();
            _lastPosition = transform.position;
            _lastEulerAngles = transform.eulerAngles;
            _sensors = new List<CarryRigidbodiesSensor>();
        }

        /// <summary>
        /// If _useTriggerAsSensor is true, adds sensors attached to carrier to
        /// _sensors list. Currently, this method assumes that both the carrier
        /// object and the sensors only have one important collider to ignore
        /// collisions between.
        /// </summary>
        private void SetSensors()
        {
            if (_useTriggerAsSensor)
            {
                foreach (CarryRigidbodiesSensor sensor in
                    GetComponentsInChildren<CarryRigidbodiesSensor>())
                {
                    sensor.carrier = this;
                    _sensors.Add(sensor);
                    Physics.IgnoreCollision(GetComponent<Collider>(),
                        sensor.GetComponent<Collider>());
                }
                if (_sensors.Count == 0)
                {
                    Debug.LogWarning("No CarryRigidbodySensors detected on carrier: " + name);
                }
            }
        }

        /// <summary>
        /// Applies movements of carrier to _carriedRigidbodies.
        /// </summary>
        /// <remarks>
        /// Only rotation about the y-axis is considered because Unity's vanilla
        /// gravity physics already induce rotations about the x and z axes.
        /// Note that the velocity vector is applied to Space.World rather than
        /// to the rigidbody's transform. This is because the velocity is a global
        /// vector and must be applied to a global reference (Space.World) rather
        /// than a local reference (Transform). Rigidbodies are checked for null
        /// because objects are sometimes destroyed while paired to a moving
        /// platform causing a NullExceptionError.
        /// </remarks>
        private void MoveCarriedBodies()
        {
            // If carrying rigidbodies and has moved since last update
            if (_carriedRigidbodies.Count > 0 && HasMoved())
            {
                // For each carried rigidbody...
                foreach (Rigidbody rb in _carriedRigidbodies)
                {
                    if (rb != null) { ApplyPositionRotationDeltas(rb); }

                }
            }
            _lastPosition = transform.position;
            _lastEulerAngles = transform.eulerAngles;
        }

        /// <summary>
        /// Applies the position and rotation deltas of the carrier to the
        /// rigidbodies in _carriedRigidbodies. Currently only applies
        /// rotation around the Y axis because gravity will assist with
        /// rotation in other directions.
        /// </summary>
        /// <param name="rb">
        /// The rigidbody to apply position and rotation deltas to.
        /// </param>
        private void ApplyPositionRotationDeltas(Rigidbody rb)
        {
            // Find position/rotation delta vectors
            Vector3 positionDelta = transform.position - _lastPosition;
            Vector3 rotationDelta = transform.eulerAngles - _lastEulerAngles;
            // Globally translate carried objects by position delta
            rb.transform.Translate(positionDelta, Space.World);
            // Horizontally rotate objects around y-axis of carrier transform
            rb.transform.RotateAround(transform.position, Vector3.up, rotationDelta.y);
        }

        /// <summary>
        /// Checks if input rigidbody is touching one of the carrier's sensors.
        /// </summary>
        /// <param name="rb">
        /// The rigidbody that is being checked for detection.
        /// </param>
        /// <returns>
        /// True if input rigidbody is detected by any sensors, false otherwise.
        /// </returns>
        private bool IsDetected(Rigidbody rb)
        {
            foreach (CarryRigidbodiesSensor sensor in _sensors)
            {
                // Return if rigidbody is touching one of sensors...
                if (sensor.detectedRigidbodies.Contains(rb))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to pair the carrier with rigidbodies that contact its
        /// colliders. This task is outsourced to CarryRigidbodySensor components
        /// if the carrier is using sensors.
        /// </summary>
        /// <param name="other">
        /// The collision information passed to the OnCollisionEnter event.
        /// </param>
        private void TryPairWithoutSensors(Collision other)
        {
            // If using triggers to detect collision, do nothing.
            if (_useTriggerAsSensor) { return; }
            // If rigidbody of colliding object exists, add rigidbody
            Rigidbody rigidbody = other.collider.GetComponent<Rigidbody>();
            if (rigidbody) { TryAddRigidbody(rigidbody); }
        }

        /// <summary>
        /// Attempts to unpair the carrier with rigidbodies that exit its
        /// colliders. This task is outsourced to CarryRigidbodySensor components
        /// if the carrier is using sensors.
        /// </summary>
        /// <param name="other">
        /// The collision information passed to the OnCollisionExit event.
        /// </param>
        private void TryUnpairWithoutSensors(Collision other)
        {
            // If using triggers to detect collision, do nothing.
            if (_useTriggerAsSensor) { return; }
            // If rigidbody of colliding object exists, try to remove
            Rigidbody rigidbody = other.collider.GetComponent<Rigidbody>();
            if (rigidbody) { TryRemoveRigidbody(rigidbody); }
        }

        /// <summary>
        /// Checks if a gameobject attached to a rigidbody holds Liftable or
        /// CarryRigidbodiesCarriable components.
        /// </summary>
        /// <param name="rb">
        /// The rigidbody being checked for components.
        /// </param>
        /// <returns>
        /// True if input rigidbody has a Liftable or CarryRigidbodiesCarriable
        /// components, false otherwise.
        /// </returns>
        private bool IsLiftableOrCarriable(Rigidbody rb)
        {
            // If rigidbody is liftable and is not currently being lifted OR is
            // CarryRigidbodiesCarriable
            Liftable liftable = rb.GetComponent<Liftable>();
            if ((liftable != null && !liftable.IsLifted) ||
                rb.GetComponent<ICarryRigidbodiesCarriable>() != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the moving platform has moved since the previous
        /// carried bodies update.
        /// </summary>
        /// <returns>
        /// True if the carrier has moved/rotated since the last check, false otherwise.
        /// </returns>
        private bool HasMoved()
        {
            return (_lastPosition != transform.position) ||
                (_lastEulerAngles != transform.eulerAngles);
        }
        #endregion
    }
}
