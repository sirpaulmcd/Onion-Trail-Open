using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This interface represents objects that can be lifted, carried, thrown.
/// </summary>
public interface ILiftable
{
    //==========================================================================
    #region Properties
    //==========================================================================
    /// <summary>
    /// Indicates if object is currently being lifted.
    /// </summary>
    bool IsLifted { get; }

    /// <summary>
    /// Indicates if object is currently being thrown.
    /// </summary>
    bool IsThrown { get; set; }

    /// <summary>
    /// The Lifter component of the object lifting this object.
    /// </summary>
    Lifter LifterObject { get; set; }
    #endregion

    //==========================================================================
    #region Public methods
    //==========================================================================
    /// <summary>
    /// Throws self in facingDirection.
    /// </summary>
    /// <param name="facingDirection">
    /// The facing direction of the thrower.
    /// </param>
    void ThrowSelf(Vector3 facingDirection);
    #endregion
}
}