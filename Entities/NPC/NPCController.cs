using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Helps to control how the NPC acts, and how players interact with the NPC.
/// </summary>
public class NPCController: MonoBehaviour, IInteractable, ISpeakable
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The dialogue prefab of the NPC.
    /// </summary>
    [SerializeField] private GameObject _dialoguePrefab = default;
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Called when the player interacts with the NPC through spherecasting.
    /// </summary>
    /// <param name="interactor">
    /// The GameObject that is interacting with the NPC.
    /// </param>
    public void Interact(GameObject interactor)
    {
        ActivateDialogue(interactor);
    }
    #endregion

    /// <summary>
    /// Activates the Dialogue of the NPC.
    /// </summary>
    public void ActivateDialogue(GameObject interactor)
    {
        DialogueCanvas.instance.ActivateDialogue(interactor, _dialoguePrefab);
    }

    //=========================================================================
    #region Private methods
    //=========================================================================
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
        // Check TextMeshProUGUI components
        Assert.IsNotNull(_dialoguePrefab, gameObject.name + 
            " is missing _dialoguePrefab");
    }
    #endregion
}
}