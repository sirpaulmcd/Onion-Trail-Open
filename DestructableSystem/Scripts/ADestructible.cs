
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Adds destructibility to a gameObject. This is to be used on in-game 
/// props that need to be able to be destroyed by the player.
/// </summary>
public abstract class ADestructible : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The object to spawn when the destructible is destroyed. Can be null.
    /// </summary>
    [SerializeField] private GameObject _dropSpawn = null;
    /// <summary>
    /// Whether or not this object is supposed to break when attacked.
    /// </summary>
    [SerializeField] private bool _brokenOnAttack = true;
    /// <summary>
    /// Whether or not this object is supposed to break when thrown.
    /// </summary>
    [SerializeField] private bool _brokenOnThrow = true;
    /// <summary>
    /// Number of hits (drops or attacks) before the object destructs.
    /// </summary>
    protected int numberOfHits = 2;
    /// <summary>
    /// Number of hits the object has experienced.
    /// </summary>
    protected int currentHitNumber = 0;
    /// <summary>
    /// Keeping track if the object has already been destructed.
    /// </summary>
    protected bool hasDestructed = false;
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called when this collider/rigidbody has begun touching another 
    /// rigidbody/collider. 
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    private void OnCollisionEnter(Collision collision)
    {
        ProcessImpact(collision);
    }
    #endregion

    //=========================================================================
    #region Protected Methods
    //=========================================================================
    /// <summary>
    /// Responsible for destructing the object at the 
    /// appropriate time and incrementing the hit count.
    /// </summary>
    /// <param name="col"></param>
    protected virtual void ProcessImpact(Collision col)
    {
        if (IsHitByThrow(col) || IsHitByAttack(col))
        {
            IncrementHit();
        }
    }

    /// <summary>
    /// Destroys the object and spawns the drop.
    /// </summary>
    protected virtual void Destruct()
    {
        if (!hasDestructed)
        {
            SpawnDrop();
            hasDestructed = true;
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Increments the hit number. If the hit number is above number of 
    /// hits destructs.
    /// </summary>
    protected virtual void IncrementHit()
    {
        currentHitNumber++;
        if (currentHitNumber >= numberOfHits)
        {
            Destruct();
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Checks whether the impact is an attack or not.
    /// </summary>
    /// <param name="collision">
    /// Collision associated with the impact.</param>
    /// <returns>
    /// True, if collision object is an attack. False otherwise.</returns>
    private bool IsAttackImpact(Collision collision)
    {
        //TO-DO: Update condition
        return collision.gameObject.GetComponent<IWarAndPeace>() != null;
    }

    /// <summary>
    /// Checks whether the impact is a throw or not.
    /// </summary>
    /// <param name="collision">
    /// Collision associated with the impact.</param>
    /// <returns>
    /// True, if collision object is the ground. False otherwise.</returns>
    private bool IsThrowImpact(Collision collision)
    {
        return collision.gameObject.CompareTag("Ground");
    }

    /// <summary>
    /// Spawns the loot dropped by the broken object.
    /// </summary>
    private void SpawnDrop()
    {
        if (_dropSpawn != null)
        {
            Instantiate(_dropSpawn, transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Determines if the destructible is hit by the throw.
    /// </summary>
    /// <param name="col"> The collision incurred. </param>
    /// <returns> True if the object can be damaged by throws and 
    /// if the impact was a throw. False otherwise.</returns>
    private bool IsHitByThrow(Collision col)
    {
        return IsThrowImpact(col) && _brokenOnThrow;
    }

    /// <summary>
    /// Determines if the destructible is hit by the attack.
    /// </summary>
    /// <param name="col"> The collision incurred. </param>
    /// <returns> True if the object can be damaged by attacks and 
    /// if the impact was an attack. False otherwise.</returns>
    private bool IsHitByAttack(Collision col)
    {
        return IsAttackImpact(col) && _brokenOnAttack;
    }
    #endregion
}
}
