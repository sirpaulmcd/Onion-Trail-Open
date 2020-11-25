// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This abstract class represents a basic enemy that targets the player 
/// closest to them. Implementations of this abstract class will determine
/// what the monster does once the target is set.
/// </summary>
public abstract class ATargeter : ACombativeEntity
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The sight radius of the enemy. Players within this radius will be 
    /// spotted. Raycasting is required if we want line of sight navigation.
    /// </summary>
    [SerializeField] protected float sightRadius = 10f;
    /// <summary>
    /// The transform of the current player target.
    /// </summary>
    protected Transform targetTransform;
    /// <summary>
    /// The number of seconds between attempts to update the current target.
    /// </summary>
    private float targetRefreshSeconds = 1.0f;
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
        InvokeRepeating("AttemptUpdateTarget", 0, targetRefreshSeconds);
    }

    /// <summary>
    /// Attempts to update the aggro target by finding the closest player. If 
    /// the closest player is outside of the sightRadius, removes the current 
    /// target. If the closest player is inside the sightRadius but different
    /// than the current target, the closest player is targeted.
    /// </summary>
    protected virtual void AttemptUpdateTarget()
    {
        Player player = GetClosestPlayer();
        if (player == null) { return; } 
        float distance = Vector3.Distance(player.GameObject.transform.position, transform.position);
        // If closest player outside of lookRadius, remove target.
        if (distance >= sightRadius) { targetTransform = null; }
        // If closest player is not the currently targeted player, swap to closest player
        else if (player.GameObject.transform != targetTransform)
        { 
            targetTransform = player.GameObject.transform; 
        }
    }

    /// <summary>
    /// Gets the closest player to the GameObject (if any).
    /// </summary>
    /// <returns>
    /// The closest player to the GameObject (if any), null otherwise.
    /// </returns>
    protected virtual Player GetClosestPlayer()
    {
        Player closestPlayer = null;
        float minDistance = float.MaxValue;
        foreach (Player player in PlayerManager.instance.Players)
        {
            if (player != null)
            {
                float distance = Vector3.Distance(player.GameObject.transform.position, transform.position);
                if (distance < minDistance) { closestPlayer = player; minDistance = distance; }
            }
        }
        return closestPlayer;
    }

    /// <summary>
    /// Draws a wire sphere that represents the sight of the GameObject.
    /// </summary>
    protected virtual void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    #endregion
}
}