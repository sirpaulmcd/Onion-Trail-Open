using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Th
/// </summary>
public class ReturnToRoad : MonoBehaviour, IInteractable
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// Whether or not players can interact with the GameObject. Becomes
    /// enabled when objective complete.
    /// </summary>
    private bool _enabled = true;
    /// <summary>
    /// The gameobject of the confirmation canvas.
    /// </summary>
    [SerializeField] private GameObject _confirmationCanvasPrefab = default;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    private void OnEnable()
    {
        RegisterAsListener();
    }

    private void OnDisable()
    {
        DeregisterAsListener();
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Register as listener to relevant events.
    /// </summary>
    private void RegisterAsListener()
    {
        EventManager.Instance.AddListener(EventName.ObjectiveCompleteEvent, HandleObjectiveCompleteEvent);
    }

    /// <summary>
    /// Deregister as listener to relevant events.
    /// </summary>
    private void DeregisterAsListener()
    {
        EventManager.Instance.RemoveListener(EventName.ObjectiveCompleteEvent, HandleObjectiveCompleteEvent);
    }

    private void HandleObjectiveCompleteEvent(object invoker, System.EventArgs e)
    {
        _enabled = true;
    }
    #endregion

    //=========================================================================
    #region Initialization
    //========================================================================
    /// <summary>
    /// Called when the player interacts with the object through spherecasting.
    /// </summary>
    /// <param name="interactor">
    /// The GameObject that is doing the interacting.
    /// </param>
    public void Interact(GameObject interactor)
    {
        if (_enabled)
        {
            GameObject.Instantiate(_confirmationCanvasPrefab);
            GameManager.Instance.ToggleDialogue(interactor);
        }
    }
    #endregion
}
}