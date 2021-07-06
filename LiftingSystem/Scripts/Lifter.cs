using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class extends Liftable. It represents a liftable object with the
    /// additional feature of lifting other objects.
    /// </summary>
    public class Lifter : Liftable
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The Liftable component of the lifted object;
        /// </summary>
        [SerializeField] private Liftable _liftedObject = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The Liftable component of the lifted object;
        /// </summary>
        public Liftable LiftedObject
        {
            get { return _liftedObject; }
            private set
            {
                if (value != _liftedObject)
                {
                    if (value != null)
                    {
                        _liftedObject = value;
                        _liftedObject.LifterObject = this;
                    }
                    else
                    {
                        _liftedObject.LifterObject = null;
                        _liftedObject = null;
                    }
                }
            }
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
            base.InitOnStart();
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
            base.AttemptToGround(collision);
        }
        #endregion


        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Pairs a lifted object with the lifter. Wrapper methord for
        /// PairToLifter().
        /// </summary>
        /// <param name="lifter">
        /// The Liftable component of the liftable object.
        /// </param>
        public void Lift(Liftable liftable)
        {
            LiftedObject = liftable;
        }

        /// <summary>
        /// Throws liftable object up and in the 'facingDirection' of the thrower.
        /// </summary>
        /// <param name="facingDirection">
        /// Normalized Vector3 indicating direction thrower is facing.
        /// </param>
        public void Throw(Vector3 facingDirection)
        {
            Liftable liftable = LiftedObject;
            LiftedObject = null;
            liftable.ThrowSelf(facingDirection);
        }

        /// <summary>
        /// Drops liftable object currently being lifted.
        /// </summary>
        public void Drop()
        {
            LiftedObject = null;
        }
        #endregion
    }
}
