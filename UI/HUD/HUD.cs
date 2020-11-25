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
    #region Class variables
    //=========================================================================
    /// <summary>
    /// The current instance of the HUD, if any.
    /// </summary>
    public static HUD instance = default;
    #endregion

    //=========================================================================
    #region Inspector linked variables
    //=========================================================================
    /// <summary>
    /// The text displaying number of lives.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _livesText = default;
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
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake() 
    {
        SetUpSingleton();
    }

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
    /// Called after a LivesUpdatedEvent is invoked. If lives are below zero, 
    /// the HUD will just display zero.
    /// </summary>
    /// <param name="invoker">The event invoker.</param>
    /// <param name="e">The event arguments/message.</param>
    private void HandleLivesUpdatedEvent(object invoker, EventArgs e)
    {
        LivesUpdatedEventArgs args = (LivesUpdatedEventArgs) e;
        string newLives = args.newLives.ToString();
        if (int.Parse(newLives) < 0) { newLives = "0"; }
        _livesText.text = args.newLives.ToString();
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
        _livesText.text = GameSession.instance.SquadLives.ToString();
    }

    /// <summary>
    /// Sets up a singleton pattern such that, if an instance of HUD already 
    /// exists, no more are created. Using DontDestroyOnLoad, the HUD instance 
    /// can persist through various scenes.
    /// </summary>
    private void SetUpSingleton()
    {
        if (HUD.instance == null) 
        {
            HUD.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    /// <summary>
    /// Registers HUD as listener to relevant events.
    /// </summary>
    private void RegisterAsListener()
    {
        // EventManager.AddListener(EventName.GameOverEvent, HandleGameOverEvent);
        EventManager.AddListener(EventName.LivesUpdatedEvent, HandleLivesUpdatedEvent);
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_livesText, gameObject.name + " is missing _livesText");
        Assert.IsNotNull(_player0Card, gameObject.name + " is missing _player0Card");
        Assert.IsNotNull(_player1Card, gameObject.name + " is missing _player1Card");
        Assert.IsNotNull(_player2Card, gameObject.name + " is missing _player2Card");
        Assert.IsNotNull(_player3Card, gameObject.name + " is missing _player3Card");
    }

    /// <summary>
    /// Unsubscribes from events so garbage collector can do its thing.
    /// </summary>
    private void Terminate()
    {
        EventManager.RemoveListener(EventName.LivesUpdatedEvent, HandleLivesUpdatedEvent);
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