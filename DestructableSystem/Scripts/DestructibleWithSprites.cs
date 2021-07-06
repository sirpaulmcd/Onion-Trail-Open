using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Adds destructibility to a gameObject with a sprite renderer. This is to
    /// be used on in-game props that need to be able to be destroyed by the
    /// player.
    /// </summary>
    public class DestructibleWithSprites : ADestructible
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The Sprites to be switched on each hit that will replace the original
        /// object such as a more broken version.
        /// </summary>
        [SerializeField] private Sprite[] _brokenSprites = default;
        /// <summary>
        /// The sprite renderer of the destructible game object.
        /// </summary>
        private SpriteRenderer _theSpriteRenderer = default;
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
            ProcessImpact(collision);
        }
        #endregion

        //=====================================================================
        #region Protected Methods
        //=====================================================================
        /// <summary>
        /// Increments the hit number and switches to the next sprite.
        /// If the hit number is above number of hits destructs.
        /// </summary>
        protected override void IncrementHit()
        {
            base.IncrementHit();
            SwitchSprite(currentHitNumber - 1);
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
            _theSpriteRenderer = GetComponent<SpriteRenderer>();
            numberOfHits = _brokenSprites.Length + 1;
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_theSpriteRenderer, gameObject.name +
                " is missing a sprite renderer.");
        }

        /// <summary>
        /// Switches the sprite in the sprite renderer to the sprite
        /// inside the broken sprites field.
        /// </summary>
        /// <param name="spriteIndex">The index of the sprite to be displayed
        /// in the broken sprites array</param>
        /// <returns>True, if the sprite has been switched. False, if the sprite
        /// cannot be swapped due to the index being out of range.</returns>
        private bool SwitchSprite(int spriteIndex)
        {
            if (spriteIndex > _brokenSprites.Length - 1)
            {
                return false;
            }
            _theSpriteRenderer.sprite = _brokenSprites[spriteIndex];
            return true;
        }
        #endregion
    }
}
