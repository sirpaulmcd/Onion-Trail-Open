using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Interface that indicates whether the player can interact with an item.
    /// </summary>
    public interface IInteractable
    {
        //=====================================================================
        #region Abstract methods
        //=====================================================================
        /// <summary>
        /// Called when the player interacts with the object through spherecasting.
        /// </summary>
        /// <param name="interactor">
        /// The GameObject that is doing the interacting.
        /// </param>
        void Interact(GameObject interactor);
        #endregion
    }
}
