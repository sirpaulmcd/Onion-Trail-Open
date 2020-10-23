using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // For EventSystem
using UnityEngine.InputSystem; // For PlayerInput 
using UnityEngine.InputSystem.UI; // InputSystemUIInputModule

namespace EGS
{
/// <summary>
/// This class represents a Player. Anything specific to a player can be stored
/// here.
/// </summary>
public class Player
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The GameObject of the player.
    /// </summary>
    public GameObject GameObject { get; set; }
    /// <summary>
    /// The health component of the player.
    /// </summary>
    public PlayerHealth Health { get; set; }
    /// <summary>
    /// The PlayerInput of the player.
    /// </summary>
    public PlayerInput Input { get; set; }
    /// <summary>
    /// The index of the player.
    /// </summary>
    public int Index { get; set; }
    /// <summary>
    /// The state of the player.
    /// </summary>
    public PlayerState State { get; set; }
    /// <summary>
    /// The crosshair used by the player.
    /// </summary>
    public Crosshair Crosshair { get; set; }
    #endregion
    
    //=========================================================================
    #region Constructors
    //=========================================================================
    /// <summary>
    /// Constructor for a Player object. Initializes properties and UI.
    /// </summary>
    /// <remarks>
    /// PlayerConfig does not inherit from MonoBehaviour because it requires
    /// a constructor. Therefore, MonoBehaviour methods like Destroy() do not 
    /// work on this class. This could be implemented with MonoBehaviour class
    /// 
    /// </remarks>
    /// <param name="pi"> 
    /// The PlayerInput of the player.
    /// </param> 
    /// <param name="ui"> 
    /// The info UI GameObject of the player.
    /// </param> 
    public Player(PlayerInput pi)
    {
        InitProperties(pi);
        InitGameObject();
        HUD.instance.ActivatePlayerCard(this.Index, this.Health);
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Disables and unlinks the info UI of the player. Used when player
    /// leaves.
    /// </summary>
    public void Terminate()
    {
        HUD.instance.DeactivatePlayerCard(this.Index);
        if (this.GameObject != null && Camera.main != null)
        {
            Camera.main.GetComponent<CameraMovement>().RemoveFocus(this.GameObject.transform);
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initializes the properties of the PlayerConfig and changes the name of
    /// the player GameObject to "Player[index]".
    /// </summary>
    private void InitProperties(PlayerInput pi)
    {
        // Initialize properties
        this.GameObject = pi.transform.gameObject;
        this.Health = GameObject.GetComponent<PlayerHealth>();
        this.Input = pi;
        this.Index = pi.playerIndex;
        this.State = PlayerState.Default;
        InitCrosshair();
    }

    /// <summary>
    /// Initializes the player's crosshair.
    /// </summary>
    private void InitCrosshair()
    {
        // Initialize crosshair component
        GameObject crosshairObject = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UIManagement/" + "Player Crosshair"));
        crosshairObject.name = "Player " + this.Index + " Crosshair";
        this.Crosshair = crosshairObject.GetComponent<Crosshair>();
        // Set crosshair gameobject instance as child of HUD
        crosshairObject.transform.SetParent(HUD.instance.transform);
    }

    /// <summary>
    /// Initializes the GameObject of the player.
    /// </summary>
    private void InitGameObject()
    {
        // Change GameObject name to "Player[index]"
        this.GameObject.name = "Player" + this.Index;
        // Add new player to camera focus
        Camera.main.GetComponent<CameraMovement>().AddFocus(this.GameObject.transform);
    }
    #endregion
}
}