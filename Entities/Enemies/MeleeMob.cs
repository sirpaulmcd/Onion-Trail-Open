// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents a basic enemy that follows players within their 
/// radius of sight. It allows the GameObject to dynamically select a target
/// to follow. The navigation values such as speed are determined in the
/// NavMeshAgent component. 
/// </summary>
public class MeleeMob : ANavTargeter
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The Animator used to updated animator variables.
    /// </summary>
    private Animator _animator;
    /// <summary>
    /// The SpriteRenderer used to flip the sprite.
    /// </summary>
    private SpriteRenderer _spriteRenderer;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called every physics update.
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        FollowTarget();
        UpdateAnimationVariables();
    }

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
    /// Sources and initializes component variables.
    /// </summary>
    protected override void InitVars()
    {
        base.InitVars();
        _animator = GetComponent<Animator>();
        GetSpriteBodyRenderer();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.CheckMandatoryComponents();
        Assert.IsNotNull(_animator, gameObject.name + " is missing _animator");
        Assert.IsNotNull(_spriteRenderer, gameObject.name + " is missing _spriteRenderer");
    }

    /// <summary>
    /// Attempts to update the aggro target by finding the closest player. If 
    /// the closest player is outside of the sightRadius, removes the current 
    /// target. If the closest player is inside the sightRadius but different
    /// than the current target, the closest player is targeted.
    /// </summary>
    protected override void AttemptUpdateTarget()
    {
        Player player = GetClosestPlayer();
        if (player == null) { return; } 
        float distance = Vector3.Distance(player.GameObject.transform.position, transform.position);
        // If closest player outside of lookRadius, remove target.
        if (distance >= sightRadius) { RemoveTarget(); }
        // If closest player is not the currently targeted player, swap to closest player
        else if (player.GameObject.transform != targetTransform)
        { 
            targetTransform = player.GameObject.transform; 
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Sets the NavMeshAgent destination to the position of the current target
    /// (if any).
    /// </summary>
    private void FollowTarget()
    {
        if (targetTransform != null && !isKnockedBack)
        {
            navMeshAgent.SetDestination(targetTransform.position);
        }
    }

    /// <summary>
    /// Updates the animator variables of the GameObject.
    /// </summary>
    private void UpdateAnimationVariables()
    {
        if (navMeshAgent == null) { return; }
        _animator.SetFloat("velocity", navMeshAgent.velocity.magnitude);
        if (navMeshAgent.velocity.x <= 0 && !isKnockedBack) { _spriteRenderer.flipX = true;}
        else { _spriteRenderer.flipX = false; }
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
    /// Removes the current target and resets the NavMeshAgent destination.
    /// </summary>
    private void RemoveTarget()
    {
        targetTransform = null;
        navMeshAgent.SetDestination(transform.position);
    }
    #endregion
}
}