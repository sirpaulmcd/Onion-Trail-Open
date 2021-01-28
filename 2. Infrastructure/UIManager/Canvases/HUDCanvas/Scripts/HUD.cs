using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace EGS
{
/// <summary>
/// This class manages the Heads Up Display responsible for displaying game
/// related information to the screen.
/// </summary>
public class HUD : MonoBehaviour
{
    //=========================================================================
    #region Inspector linked variables
    //=========================================================================
    /// <summary>
    /// The text displaying amount of food.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _foodText = default;
    /// <summary>
    /// The text displaying amount of food.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _fuelText = default;
    /// <summary>
    /// The text displaying amount of food.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _medText = default;
    /// <summary>
    /// The UI object for player 0 info.
    /// </summary>
    [SerializeField] private GameObject _player0Card = default;
    /// <summary>
    /// The UI card for player 1 info.
    /// </summary>
    [SerializeField] private GameObject _player1Card = default;
    /// <summary>
    /// The UI card for player 2 info.
    /// </summary>
    [SerializeField] private GameObject _player2Card = default;
    /// <summary>
    /// The UI card for player 3 info.
    /// </summary>
    [SerializeField] private GameObject _player3Card = default;
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

    /// <summary>
    /// Called when a Scene or game ends.
    /// </summary>
    private void OnDestroy()
    {
        DeregisterAsListener();
    }
    #endregion

    //=========================================================================
    #region Event handlers
    //=========================================================================
    /// <summary>
    /// Called after a FoodUpdatedEvent is invoked. If food is below zero, the 
    /// HUD will just display zero.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleFoodUpdatedEvent(object invoker, EventArgs e)
    {
        FoodUpdatedEventArgs args = (FoodUpdatedEventArgs) e;
        string newFood = args.newFood.ToString();
        if (int.Parse(newFood) < 0) { newFood = "0"; }
        _foodText.text = args.newFood.ToString();
        Debug.Log("HUD.HandleFoodUpdatedEvent: Updating HUD food value.");
    }

    /// <summary>
    /// Called after a FuelUpdatedEvent is invoked. If fuel is below zero, the 
    /// HUD will just display zero.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleFuelUpdatedEvent(object invoker, EventArgs e)
    {
        FuelUpdatedEventArgs args = (FuelUpdatedEventArgs) e;
        string newFuel = args.newFuel.ToString();
        if (int.Parse(newFuel) < 0) { newFuel = "0"; }
        _fuelText.text = args.newFuel.ToString();
        Debug.Log("HUD.HandleFuelUpdatedEvent: Updating HUD fuel value.");
    }

    /// <summary>
    /// Called after a MedUpdatedEvent is invoked. If med is below zero, the 
    /// HUD will just display zero.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleMedUpdatedEvent(object invoker, EventArgs e)
    {
        MedUpdatedEventArgs args = (MedUpdatedEventArgs) e;
        string newMed = args.newMed.ToString();
        if (int.Parse(newMed) < 0) { newMed = "0"; }
        _medText.text = args.newMed.ToString();
        Debug.Log("HUD.HandleMedUpdatedEvent: Updating HUD med value.");
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Initializes and acitvates the player card associated with the input
    /// index.
    /// </summary>
    /// <param name="index">The index of the player.</param>
    /// <param name="health">The Health component of the player.</param>
    public void ActivatePlayerCard(int index, Health health)
    {
        switch (index)
        {
            case 0:
                InitCard(_player0Card, health);
                break;
            case 1:
                InitCard(_player1Card, health);
                break;
            case 2:
                InitCard(_player2Card, health);
                break;
            case 3:
                InitCard(_player3Card, health);
                break;
            default:
                Debug.Log("Invalid player card index: " + index);
                break;
        }
    }

    /// <summary>
    /// Resets and deactives the player card associated with the input index.
    /// </summary>
    /// <param name="index">The index of the player</param>
    public void DeactivatePlayerCard(int index)
    {
        switch (index)
        {
            case 0:
                TerminateCard(_player0Card);
                break;
            case 1:
                TerminateCard(_player1Card);
                break;
            case 2:
                TerminateCard(_player2Card);
                break;
            case 3:
                TerminateCard(_player3Card);
                break;
            default:
                Debug.Log("Invalid player card index: " + index);
                break;
        }
    }
    #endregion;

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    private void InitOnStart()
    {
        InitVars();
        RegisterAsListener();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        if (OTGameSession.Instance != null)
        {
            _foodText.text = OTGameSession.Instance.Food.ToString();
            _fuelText.text = OTGameSession.Instance.Fuel.ToString();
            _medText.text = OTGameSession.Instance.Med.ToString();
        }
    }

    /// <summary>
    /// Registers HUD as listener to relevant events.
    /// </summary>
    private void RegisterAsListener()
    {
        EventManager.Instance.AddListener(EventName.FoodUpdatedEvent, HandleFoodUpdatedEvent);
        EventManager.Instance.AddListener(EventName.FuelUpdatedEvent, HandleFuelUpdatedEvent);
        EventManager.Instance.AddListener(EventName.MedUpdatedEvent, HandleMedUpdatedEvent);
    }

    /// <summary>
    /// Unsubscribes from events so garbage collector can do its thing.
    /// </summary>
    private void DeregisterAsListener()
    {
        EventManager.Instance.RemoveListener(EventName.FoodUpdatedEvent, HandleFoodUpdatedEvent);
        EventManager.Instance.RemoveListener(EventName.FuelUpdatedEvent, HandleFuelUpdatedEvent);
        EventManager.Instance.RemoveListener(EventName.MedUpdatedEvent, HandleMedUpdatedEvent);
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_foodText, gameObject.name + " is missing _foodText");
        Assert.IsNotNull(_fuelText, gameObject.name + " is missing _fuelText");
        Assert.IsNotNull(_medText, gameObject.name + " is missing _medText");
        Assert.IsNotNull(_player0Card, gameObject.name + " is missing _player0Card");
        Assert.IsNotNull(_player1Card, gameObject.name + " is missing _player1Card");
        Assert.IsNotNull(_player2Card, gameObject.name + " is missing _player2Card");
        Assert.IsNotNull(_player3Card, gameObject.name + " is missing _player3Card");
    }

    /// <summary>
    /// Initializes and activates the input player card.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="health"></param>
    private void InitCard(GameObject card, Health health)
    {
        card.SetActive(true);
        card.GetComponentInChildren<UIHealthBar>().HealthSource = health;
    }

    /// <summary>
    /// Resets and deactives the input player card.
    /// </summary>
    /// <param name="card"></param>
    private void TerminateCard(GameObject card)
    {
        if (card != null)
        {
            card.GetComponentInChildren<UIHealthBar>().HealthSource = null;
            card.SetActive(false);
        }
    }
    #endregion
}
}