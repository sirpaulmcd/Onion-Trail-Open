using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/// <summary>
/// This interface represents the attributes of a weapon.
/// </summary> 
public interface IWeapon
{
    //=========================================================================
    // Abstract methods
    //=========================================================================
    /// <summary>
    /// Activates the main attack of the weapon.
    /// </summary>
    void Attack(Vector3 direction);
}
}