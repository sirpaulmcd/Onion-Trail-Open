using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// A dailogue holder. Requires one or many children gameobjects that have
    /// dialogue block components each. This class is responsible for initiating
    /// the entire dialogue and ending it.
    /// </summary>
    public class DialogueHolder : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The dialogue sequence IEnumerator for use in a Coroutine.
        /// </summary>
        private IEnumerator _dialogueSeq;
        /// <summary>
        /// Bool used to continue the dialogue sequence.
        /// </summary>
        private bool _receivedContinueInput = false;
        /// <summary>
        /// The currently active block of dialogue being written.
        /// </summary>
        private DialogueBlock _currentBlock = default;
        /// <summary>
        /// The GameObject containing the UI that displays the word "Next" to
        /// indicate that a page of dialogue is complete.
        /// </summary>
        private GameObject _nextUI = default;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called when an object is Enabled. Starts the dialogue sequence.
        /// </summary>
        private void OnEnable()
        {
            InitOnEnable();
            StartCoroutine(DialogueSequence());
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Called by the player upon using a NextLine input action. If the
        /// current block of dialogue is not done writing, skips the remainder
        /// of the writing animation. If the current block has finished
        /// writing, continues to the next block.
        /// </summary>
        public void ContinueDialogue()
        {
            // Process skip writing input
            _currentBlock.SkipWriting = true;
            // Process next block input
            _receivedContinueInput = true;
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in OnEnable().
        /// </summary>
        private void InitOnEnable()
        {
            InitVars();
            CheckMandatoryComponents();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.GetComponent<DialogueBlock>() == null)
                {
                    _nextUI = transform.GetChild(i).gameObject;
                    return;
                }
            }
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_nextUI, gameObject.name + " is missing _nextUI");
        }

        /// <summary>
        /// Creates a dialogue sequence which activates each child
        /// DialogueBlock object in order. For each block, it waits until the
        /// dialogue is finished before displaying the next.
        /// </summary>
        private IEnumerator DialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                ResetDialogueBlocks();
                if (IsDialogueBlock(i))
                {
                    yield return StartCoroutine(ActivateNextChild(i));
                    yield return StartCoroutine(RecieveContinueInput());
                }
            }
            TerminateDialogue();
        }

        /// <summary>
        /// Resets components such that the holder is ready to process the next
        /// DialogueBlock.
        /// </summary>
        private void ResetDialogueBlocks()
        {
            // Deactivate all children
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            // Deactivate "Next" text
            _nextUI.SetActive(false);
            // Reset indicators
            _receivedContinueInput = false;
        }

        /// <summary>
        /// Activates the next DialogueBlock in queue such that it writes
        /// itself to screen.
        /// </summary>
        /// <param name="i">
        /// The index of the DialogueBlock to be activated.
        /// </param>
        private IEnumerator ActivateNextChild(int i)
        {
            // Activate next child dialogue block
            _currentBlock = transform.GetChild(i).GetComponent<DialogueBlock>();
            _currentBlock.gameObject.SetActive(true);
            // Wait until block is finished
            yield return new WaitUntil(() => _currentBlock.Finished);
        }

        /// <summary>
        /// Activates "Next" UI text to indicate to the user that the dialogue
        /// block has completed writing and waits for user continuation input.
        /// </summary>
        private IEnumerator RecieveContinueInput()
        {
            // Activate "Next" UI text
            _nextUI.SetActive(true);
            // Wait for user continuation input
            _receivedContinueInput = false;
            yield return new WaitUntil(() => _receivedContinueInput);
        }

        /// <summary>
        /// Terminates the dialogue sequence. Deactivates dialogue UI and
        /// returns previous input action maps to the players.
        /// </summary>
        private void TerminateDialogue()
        {
            ResetDialogueBlocks();
            transform.parent.gameObject.GetComponent<DialogueCanvas>().DeactivateDialogue();
        }

        /// <summary>
        /// Checks if the indexed child has a DialogueBlock component.
        /// </summary>
        /// <param name="i">
        /// The index of the child object.
        /// </param>
        /// <returns>
        /// True if indexed child object has a DialogueBlock component, false otherwise.
        /// </returns>
        private bool IsDialogueBlock(int i)
        {
            return transform.GetChild(i).GetComponent<DialogueBlock>() != null;
        }
        #endregion
    }
}
