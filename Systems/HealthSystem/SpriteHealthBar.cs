using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
public class SpriteHealthBar : MonoBehaviour
{
    //=====================================================================
    #region Notes
    //=====================================================================
    // The healthbar scaling relies on some Unity magic found in the thread:
    // http://answers.unity.com/comments/1605488/view.html.
    // It works by moving the edge of the green bar to the center of an 
    // empty GameObject so that the scaling of the empty object will 
    // counteract the recursive scaling of the bar.
    #endregion    

    //=====================================================================
    #region Instance variables
    //=====================================================================
    /// <summary>
    /// The transform of the component holding the sprite renderer.
    /// </summary>
    private Transform _healthScaler = default;
    /// <summary>
    /// The IHealth comoponent of the _health source.
    /// </summary>
    private IHealth _healthSource = default;
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
    /// Called when a Scene or game ends.
    /// </summary>
    private void OnDestroy()
    {
        Terminate();
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Called when a HealthUpdatedEvent is invoked.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleHealthUpdatedEvent(object invoker, EventArgs e)
    {
        if (invoker == _healthSource)
        {
            HealthUpdatedEventArgs args = (HealthUpdatedEventArgs) e;
            ResizeHealthBar(args.newHealth, args.maxHealth);
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
        RegisterAsListener();
        ResizeHealthBar(_healthSource.CurrentHealth, _healthSource.MaximumHealth);
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        _healthSource = transform.parent.GetComponent<IHealth>();
        foreach (Transform t in transform)
        {
            if (t.name == "HealthScaler") { _healthScaler = t; }
        }
    }
    
    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_healthSource, gameObject.name + " is missing _healthSource");
        Assert.IsNotNull(_healthScaler, gameObject.name + " is missing _healthScaler");
    }

    /// <summary>
    /// Registers HealthBar as listener to desired events.
    /// </summary>
    private void RegisterAsListener()
    {
        EventManager.AddListener(EventName.HealthUpdatedEvent, HandleHealthUpdatedEvent);
    }

    /// <summary>
    /// Unregister as listener to events.
    /// </summary>
    private void Terminate()
    {
        EventManager.RemoveListener(EventName.HealthUpdatedEvent, HandleHealthUpdatedEvent);
    }

    /// <summary>
    ///   Rescales the _health bar.
    /// </summary>
    /// <param name="newHealth">
    ///   The updated HP to be reflected in the _health bar.
    ///   Assumes 0 <= newHealth <= maximumHealth
    /// </param>
    private void ResizeHealthBar(int newHealth, int maxHealth)
    {
        _healthScaler.localScale = new Vector3((float)newHealth / _healthSource.MaximumHealth, 1f);
    }
    #endregion
}
}