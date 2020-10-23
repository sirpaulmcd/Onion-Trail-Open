using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class should be implemented by any objects that have associated 
/// dialogue.
/// </summary>
public interface ISpeakable
{
    //=========================================================================
    #region Abstract methods
    //=========================================================================
    /// <summary>
    /// Activates the Dialogue of the Speakable.
    /// </summary>
    void ActivateDialogue(GameObject interactor);
    #endregion
}
}