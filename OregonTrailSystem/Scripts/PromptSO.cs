using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// Scriptable object assicated with a prompt option.
    /// </summary>
    [CreateAssetMenu(fileName = "PromptSO", menuName = "ScriptableObjects/PromptSO")]
    public class PromptSO : ScriptableObject
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The dialogue of the prompt.
        /// </summary>
        [TextArea(5, 10)] public string text = default;
        /// <summary>
        /// The delay in seconds between displaying each letter in the dialogue
        /// block (type-writer effect).
        /// </summary>
        public float typeDelaySeconds = 0f;
        /// <summary>
        /// The options associated with the prompt.
        /// </summary>
        public PromptOptionInfo[] optionInfo;
        #endregion
    }
}
