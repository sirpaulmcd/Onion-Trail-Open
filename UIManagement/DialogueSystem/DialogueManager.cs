﻿// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This component must be placed on a default Canvas GameObject.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    //=========================================================================
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The current isntance of the dialogue manager (if any).
    /// </summary>
    public static DialogueManager instance = default;
    #endregion

    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The current DialogueHolder GameObject being displayed to the screen.
    /// </summary>
    private GameObject _currentDialogueHolderObject = default;
    /// <summary>
    /// The current DialogueHolder being displayed to the screen.
    /// </summary>
    private DialogueHolder _currentDialogueHolder = default;
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        SetUpSingleton();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
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
        AddDialogueEffects(interactor);
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
        RemoveDialogueEffects();
        Destroy(_currentDialogueHolderObject);
        _currentDialogueHolderObject = null;
        _currentDialogueHolder = null;
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Sets up a singleton pattern such that, if an instance of 
    /// DialogueManager already exists, no more are created. Using 
    /// DontDestroyOnLoad, the DialogueManager instance can persist through 
    /// various scenes.
    /// </summary>
    private void SetUpSingleton()
    {
        if (DialogueManager.instance == null) 
        {
            DialogueManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
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
        _currentDialogueHolderObject.transform.SetParent(transform);
        _currentDialogueHolderObject.SetActive(true);
    }

    /// <summary>
    /// Adds dialogue effects such that players can't take damage and only the
    /// initiating player has control of the dialogue sequence.
    /// </summary>
    /// <param name="interactor">
    /// The GameObject of the dialogue initiating player.
    /// </param>
    private void AddDialogueEffects(GameObject interactor)
    {
        PlayerManager.instance.MakePlayersInvulnerable();
        PlayerManager.instance.LeaderControlChange(interactor, "Dialogue");
    }

    /// <summary>
    /// Removes the dialogue effects inflicted on the players. Players can once
    /// again take damage and control themselves as usual.
    /// </summary>
    private void RemoveDialogueEffects()
    {
        PlayerManager.instance.MakePlayersVulnerable();
        PlayerManager.instance.UndoLeaderControlChange();
    }
    #endregion
}
}