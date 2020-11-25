using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This script is used to control a switch that the player can interact with.
/// </summary>
public class Switch : MonoBehaviour, IInteractable
{
	//==========================================================================
    #region Instance variables
    //==========================================================================
    /// <summary>
    /// The GameObject that the switch will interact with. This needs to be linked in the inspector.
    /// </summary>
    [SerializeField] private GameObject _linkedObject = null;
    /// <summary>
    /// The animation component for turning the lever on and off.
    /// </summary>
 	private Animation _animation;
 	/// <summary>
    /// A bool that inicated whether ther switch is currently toggled or not.
    /// </summary>
    private bool _isToggled = false;
    #endregion

    //==========================================================================
    #region MonoBehaviour
    //==========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }
    #endregion
    
    //==========================================================================
    #region Public methods
    //==========================================================================
    /// <summary>
    /// Called when the player interacts with the object through spherecasting.
    /// </summary>
    /// <param name="interactor">
    /// The GameObject that is interacting with the switch.
    /// </param>
    public void Interact(GameObject interactor)
    {
    	ActivateObject();
    }
    #endregion

    //==========================================================================
    #region Private methods
    //==========================================================================
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
        _animation = GetComponentInChildren<Animation>();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_animation, gameObject.name + " is missing _animation");
    }

    /// <summary>
    /// Makes the lever visually switch on and off, as well as flips the 
    /// isToggled to indicate the switch is active.
    /// </summary>
    private void ActivateObject()
    {
    	if(IsInteractable(_linkedObject))
	    {
	        _linkedObject.GetComponent<IInteractable>().Interact(this.gameObject);
	    }
	    if (_isToggled)
       	{
        	// if switch is on, turn off switch.
            _animation.Play("LeverOFF");
            _isToggled = false;
		}
		else
		{
           	// if switch is off, turn it on.
            _animation.Play("LeverON");
           	_isToggled = true;
        }

    }

    /// <summary>
    /// Checks to see if GameObject is interactable.
    /// </summary>
    /// <param name="obj">
    /// The GameObject being checked for Interactable component.
    /// </param>
    private bool IsInteractable(GameObject obj)
    {
    	return obj.GetComponent<IInteractable>() != null;
    }
    #endregion
}
}