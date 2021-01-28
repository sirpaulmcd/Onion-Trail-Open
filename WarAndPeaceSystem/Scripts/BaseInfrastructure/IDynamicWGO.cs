using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// Interface that corresponds with Dynamic WGOs. See ADynamicWGO.cs.
/// </summary>
public interface IDynamicWGO : IWarAndPeace
{
    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The speed of the DWGO.
    /// </summary>
    float MoveSpeed { get; set; }
    /// <summary>
    /// The number of seconds before an instantiated DWGO is to self destruct. 
    /// Necessary if the DWGO never hits anything.
    /// </summary>
    float SelfDestructSeconds { get; set; }
    /// <summary>
    /// The distance from the attacker transform that the DWGO should be 
    /// spawned.
    /// </summary>
    float SpawnOffsetDistance { get; set; }
    #endregion
}
}