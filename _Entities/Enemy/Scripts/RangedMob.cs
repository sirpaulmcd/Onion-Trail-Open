// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents a basic enemy that shoots players within their 
/// radius of sight. It allows the GameObject to dynamically select a target
/// to shoot.
/// </summary>
public class RangedMob : ATargeter
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The SpriteRenderer used to flip the sprite.
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    /// <summary>
    /// The weapon used by the ranged mob.
    /// </summary>
    private BulletDWGOWeapon _weapon = default;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called when a GameObject is selected in the inspector.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected override void InitOnStart()
    {
        base.InitOnStart();
        InvokeRepeating("ShootTarget", 0f, _weapon.CooldownSeconds);
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    protected override void InitVars()
    {
        base.InitVars();
        GetSpriteBodyRenderer();
        GetWeapon();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.CheckMandatoryComponents();
        Assert.IsNotNull(_spriteRenderer, gameObject.name + " is missing _spriteRenderer");
        Assert.IsNotNull(_weapon, gameObject.name + " is missing _weapon");
    }

    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Shoots towards the current target. Prevents vertical bullets. However,
    /// this restriction may not be necessary.
    /// </summary>
    private void ShootTarget()
    {
        if (targetTransform == null) { return; }
        Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
        _weapon.Attack(new Vector3(directionToTarget.x, 0, directionToTarget.z));
    }

    /// <summary>
    /// Sources the SpriteRenderer component from the child named "SpriteBody".
    /// Since the current health bar also uses a SpriteRenderer, this was
    /// necessary. May be updated later.
    /// </summary>
    private void GetSpriteBodyRenderer()
    {
        foreach (Transform child in transform)
        {
            if (child.name == "SpriteBody") { _spriteRenderer = child.GetComponent<SpriteRenderer>(); }
        }
    }

    /// <summary>
    /// Sources the ranged weapon of the ranged mob.
    /// </summary>
    private void GetWeapon()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child.transform)
            {
                BulletDWGOWeapon weapon = grandChild.gameObject.GetComponent<BulletDWGOWeapon>();
                if (weapon != null) { _weapon = weapon; return; }
            }
        }
        Debug.LogWarning("No weapon detected.", this.gameObject);
    }
    #endregion
}
}