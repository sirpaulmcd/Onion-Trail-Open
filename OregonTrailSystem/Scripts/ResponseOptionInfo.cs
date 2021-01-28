using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Scriptable object assicated with a prompt option.
/// </summary>
[System.Serializable]
public class ResponseOptionInfo
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The name of the new scene to be loaded (if any).
    /// </summary>
    public string newScene = default;
    /// <summary>
    /// The text of the button associated with the option.
    /// </summary>
    public string buttonText = default;
    #endregion
}
}