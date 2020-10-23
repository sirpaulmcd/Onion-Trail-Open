using UnityEngine;
using UnityEngine.AI;

namespace EGS
{
/// <summary>
///   Simple script which allows for a NavMeshAgent to follow the nearest player.
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    //============================================================================
    // Dependencies
    //============================================================================
    public PlayerPositionSystem playerPositionSystem;
    public NavMeshAgent navMeshAgent;

    //============================================================================
    // Unity Methods
    //============================================================================
    void Update()
    {
        if (GetComponent<Liftable>().IsLifted)
        {
            // Disable the NavMeshAgent so that the enemy does not immediately
            // snap onto the NavMesh surface after being thrown. Re-enabling is
            // done by the OnCollisionEnter() code. 
            navMeshAgent.enabled = false;
        }
        else if (navMeshAgent.enabled)
        {
            try
            {
                Vector3 pos = navMeshAgent.transform.position;
                navMeshAgent.destination = playerPositionSystem.FindClosest(pos);
            }
            catch
            {
                // If all players are dead, do nothing.
            }
        }
    }
    
    /// <summary>
    ///   Logic to handle the AI touching the ground. This reenables the
    ///   NavMeshAgent which then snaps the AI to the NavMesh surface again.
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        // Check that we are on ground (Jumpable) but do not trigger on
        // the player, which is also Jumpable.
        if (other.gameObject.GetComponent<Jumpable>() != null &&
            other.gameObject.GetComponent<PlayerController>() == null)
        {            
            navMeshAgent.enabled = true;

            // Undo any rotation caused by physics interactions.
            navMeshAgent.transform.rotation = Quaternion.identity;
        }
    }
}
}