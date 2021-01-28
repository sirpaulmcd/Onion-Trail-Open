using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class is responsible for adding functionality to the the loading
/// screen canvas. (TODO: Expand on later)
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    private void Start()
    {
        InitOnStart();
    }
    #endregion

    //=========================================================================
    #region Initialization
    //=========================================================================
    private void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    private void InitVars()
    {
        // _myVariable = GetComponent<MyClass>();
    }

    private void CheckMandatoryComponents()
    {
        // Assert.IsNotNull(_myVariable, gameObject.name + " is missing _myVariable");
    }
    #endregion
}
}