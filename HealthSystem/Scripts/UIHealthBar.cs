using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace EGS
{
/// <summary>
/// This class is used to control player UI _health bars. Scales _health bar
/// proportionately with linked _health source.
/// </summary>
public class UIHealthBar : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The Slider used to scale the _health bar.
    /// </summary>
    private Slider _slider;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The HealthSource used to subsribe to HealthUpdated events.
    /// </summary>
    public IHealth HealthSource { get; set;} = default;
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    {
        InitOnStart();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener(EventName.HealthUpdatedEvent, HandleHealthUpdatedEvent);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener(EventName.HealthUpdatedEvent, HandleHealthUpdatedEvent);
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Called after a HealthUpdatedEvent is invoked.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleHealthUpdatedEvent(object invoker, EventArgs e)
    {
        if (invoker == HealthSource)
        {
            HealthUpdatedEventArgs args = (HealthUpdatedEventArgs) e;
            ResizeHealthBar(args.newHealth, args.maxHealth);
            Debug.Log("UIHealthBar.HandleHealthUpdatedEvent: Health bar resized.");
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
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
        _slider = GetComponent<Slider>();
        _slider.interactable = false;
    }

    /// <summary>
    /// Checks whether mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_slider, gameObject.name + " is missing _slider");
        Assert.IsNotNull(HealthSource, gameObject.name + " is missing HealthSource");
    }

    /// <summary>
    /// Rescales the _health bar.
    /// </summary>
    /// <param name="newHealth">
    /// The updated HP to be reflected in the _health bar.
    /// Assumes 0 <= newHealth <= maximumHealth
    /// </param>
    private void ResizeHealthBar(int newHealth, int maxHealth)
    {
        _slider.maxValue = maxHealth;
        _slider.value = newHealth;
    }
    #endregion
}
}