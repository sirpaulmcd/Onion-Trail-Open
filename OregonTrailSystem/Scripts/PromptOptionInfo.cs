using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// The information associated with the prompt option.
    /// </summary>
    [System.Serializable]
    public class PromptOptionInfo
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The text displayed on the option button.
        /// </summary>
        public string buttonText = default;
        /// <summary>
        /// The change in food upon selecting the option.
        /// </summary>
        public int foodDelta = 0;
        /// <summary>
        /// The change in fuel upon selecting the option.
        /// </summary>
        public int fuelDelta = 0;
        /// <summary>
        /// The change in med upon selecting the option.
        /// </summary>
        public int medDelta = 0;
        #endregion
    }
}
