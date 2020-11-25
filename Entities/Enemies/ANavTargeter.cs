// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents an instance of ATargeter that also uses a NavMesh to
/// follow its targets. AI that uses navigational meshes needed to be handled
/// differently than typical objects. For example, NavMeshAgents need different
/// knockback mechanics because knocking a navigator off of their NavMesh can 
/// result in a variety of bugs.
/// </summary>
public class ANavTargeter : ATargeter
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    // [Header("ANavTargeter variables")]
    /// <summary>
    /// The NavMeshAgent component for basic AI pathfinding.
    /// </summary>
    protected NavMeshAgent navMeshAgent;
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
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.InitVars();
        Assert.IsNotNull(navMeshAgent, gameObject.name + " is missing navMeshAgent");
    }

    /// <summary>
    /// Attempts to update the aggro target by finding the closest player. If 
    /// the closest player is outside of the sightRadius, removes the current 
    /// target. If the closest player is inside the sightRadius but different
    /// than the current target, the closest player is targeted.
    /// </summary>
    /// <remarks>
    /// The only difference made in this override was to not update the target
    /// while the NavMeshAgent is experiencing knockback.
    /// </remarks>
    protected override void AttemptUpdateTarget()
    {
        if (this.IsKnockedBack) { return; }
        Player player = base.GetClosestPlayer();
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
    /// Adds knockback effects to the NavMeshAgent.
    /// </summary>
    protected override void AddKnockbackEffects()
    {
        navMeshAgent.enabled = false;
        rigidbody.isKinematic = false;
    }

    /// <summary>
    /// Removes knockback effects from the NavMeshAgent. A null check is
    /// required because the GameObject may have been killed before reaching
    /// their knockback destination.
    /// </summary>
    protected override void RemoveKnockbackEffects()
    {
        navMeshAgent.enabled = true;
        rigidbody.isKinematic = true;
        knockbackDirection = default;
        knockbackMagnitude = default;
    }
    #endregion
}
}