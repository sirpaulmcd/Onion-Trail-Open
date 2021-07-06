// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This component must be placed on a default Canvas GameObject.
    /// </summary>
    public class DialogueCanvas : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The current DialogueHolder GameObject being displayed to the screen.
        /// </summary>
        private GameObject _currentDialogueHolderObject = default;
        /// <summary>
        /// The current DialogueHolder being displayed to the screen.
        /// </summary>
        private DialogueHolder _currentDialogueHolder = default;
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Activates the dialogue sequence by activating the input dialogue
        /// prefab. Induces dialogue effects and initializes dialogue.
        /// </summary>
        /// <param name="interactor">
        /// The GameObject of the dialogue initiating player.
        /// </param>
        /// <param name="dialoguePrefab">
        /// The GameObject containing the DialougeHolder and related DialogueLines.
        /// </param>
        public void ActivateDialogue(GameObject interactor, GameObject dialoguePrefab)
        {
            GameManager.Instance.ToggleDialogue(interactor);
            InitDialogue(dialoguePrefab);
        }

        /// <summary>
        /// Continues the current dialogue sequence, if it exists.
        /// </summary>
        public void ContinueDialogue()
        {
            _currentDialogueHolder.ContinueDialogue();
        }

        /// <summary>
        /// Deactivates the dialogue sequence. Resets/Deactivates the
        /// DialogueHolder.
        /// </summary>
        public void DeactivateDialogue()
        {
            GameManager.Instance.ToggleDialogue(null);
            Destroy(_currentDialogueHolderObject);
            _currentDialogueHolderObject = null;
            _currentDialogueHolder = null;
        }

        /// <summary>
        /// Initiates the input dialogue prefab. Makes DialogueHolder child of self
        /// whose GameObject is a canvas.
        /// </summary>
        /// <param name="dialogueHolderObject">
        /// The GameObject containing the DialougeHolder and related DialogueLines.
        /// </param>
        private void InitDialogue(GameObject dialoguePrefab)
        {
            _currentDialogueHolderObject = GameObject.Instantiate(dialoguePrefab);
            _currentDialogueHolder = _currentDialogueHolderObject.GetComponent<DialogueHolder>();
            _currentDialogueHolderObject.gameObject.transform.SetParent(transform);
            _currentDialogueHolderObject.SetActive(true);
        }
        #endregion
    }
}
