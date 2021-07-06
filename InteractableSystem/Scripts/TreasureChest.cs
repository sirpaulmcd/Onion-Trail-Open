// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Script for Giftboxes & Treasure Chests.
    /// </summary>
    public class TreasureChest : MonoBehaviour, IInteractable
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The loot dropped by the treasure chest.
        /// </summary>
        [SerializeField] private GameObject _lootObject = default;
        /// <summary>
        /// The effect for when the box is destroyed.
        /// </summary>
        [SerializeField] private GameObject _openParticleEffect = default;
        /// <summary>
        /// The sound effect for when the chest is opened.
        /// </summary>
        [SerializeField] private float _openEffectSeconds = 1;
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
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Called when the player interacts with the object through spherecasting.
        /// </summary>
        /// <param name="interactor">
        /// The GameObject that is doing the interacting.
        /// </param>
        public void Interact(GameObject interactor)
        {
            ActivateChest();
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
            CheckMandatoryComponents();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_lootObject, gameObject.name + " is missing _lootObject");
        }

        /// <summary>
        /// Spawns loot object, plays destruction effect, and destroys game object.
        /// </summary>
        private void ActivateChest()
        {
            Instantiate(_lootObject, transform.position, Quaternion.identity);
            if (_openParticleEffect != null)
            {
                GameObject effect = Instantiate(_openParticleEffect, transform.position, transform.rotation);
                Destroy(effect, _openEffectSeconds);
            }
            Destroy(gameObject);
        }
        #endregion
    }
}
