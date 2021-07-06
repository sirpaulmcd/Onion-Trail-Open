using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class holds prefab references to generalized objects that do not
    /// belong to any particular object.
    /// </summary>
    public class GameAssets : Singleton<GameAssets>
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private Transform _textPopup = default;
        [SerializeField] private Transform _damageTextPopup = default;
        [SerializeField] private Transform _incrementationTextPopup = default;
        [SerializeField] private Transform _flamesParticleEffect = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// Prefab for text popup without an animation.
        /// </summary>
        public Transform TextPopup
        {
            get { return _textPopup; }
            private set { _textPopup = value; }
        }

        /// <summary>
        /// Prefab for text popup with damage animation.
        /// </summary>
        public Transform DamageTextPopup
        {
            get { return _damageTextPopup; }
            private set { _damageTextPopup = value; }
        }

        /// <summary>
        /// Prefab for text popup with incrementation animation.
        /// </summary>
        public Transform IncrementationTextPopup
        {
            get { return _incrementationTextPopup; }
            private set { _incrementationTextPopup = value; }
        }

        /// <summary>
        /// Prefab for flame particle effect.
        /// </summary>
        public Transform FlamesParticleEffect
        {
            get { return _flamesParticleEffect; }
            private set { _flamesParticleEffect = value; }
        }
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        private void Start()
        {
            CheckMandatoryComponents();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_textPopup, gameObject.name + " is missing _textPopup");
        }
        #endregion
    }
}
