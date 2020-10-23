using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Adds destrucibility to a gameObject. This is to be used on in-game props
/// that need to be able to be destroyed by the player.
/// </summary>
public class Destructible : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The object that will replace the original object such as explosion or 
    /// broken version of object.
    /// </summary>
    [SerializeField] private GameObject _destroyedSpawn = default;
    /// <summary>
    /// The object to spawn when the destructible is destroyed.
    /// </summary>
    [SerializeField] private GameObject _dropSpawn = default;
    /// <summary>
    /// Whether or not this object is supposed to break when thrown.
    /// </summary>
    [SerializeField] private bool _brokenOnThrow = false;
    /// <summary>
    /// Whether or not this object is supposed to break when attacked.
    /// </summary>
    [SerializeField] private bool _brokenOnAttack = false;
    /// <summary>
    /// The max impact velocity the object can handle before breaking.
    /// </summary>
    [SerializeField]
    private float _maxVelocityBeforeBreak = 10f;
    /// <summary>
    /// The offset from the transform position to apply the explosive force 
    /// when the object is broken.
    /// </summary>
    [SerializeField]
    private Vector3 _explosiveForceOffset = default;
    /// <summary>
    /// The magnitude of the explosive force to apply to the destroyedSpawn 
    /// when the object is broken.
    /// </summary>
    [SerializeField]
    private float _explosiveForce = 200.0f;
    /// <summary>
    /// The radius of the explosive force to apply to the destroyedSpawn 
    /// when the object is broken.
    /// </summary>
    [SerializeField]
    private float _explosionRadius = 10.0f;
    /// <summary>
    /// The _health of the object. This is used to make destructibles more 
    /// realistic.
    /// </summary>
    private Health _health;
    /// <summary>
    /// Bool indicating whether or not object has already began destructing.
    /// This is used because OnCollisionEnter can be called numerous times
    /// when colliding with other GameObjects.
    /// </summary>
    private bool _hasDestructed = false;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }

    /// <summary>
    /// Called when this collider/rigidbody has begun touching another 
    /// rigidbody/collider.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    private void OnCollisionEnter(Collision collision)
    {
        if (IsDestroyedOnImpact(collision) || IsDestroyedOnAttack(collision))
        {
            Destruct();
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    private void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        _health = GetComponent<Health>();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_health, gameObject.name + " is missing health component");
    }

    /// <summary>
    /// Destroys the object and instantiates the _destroyedSpawn and 
    /// _dropSpawn.
    /// </summary>
    private void Destruct()
    {
        if (_hasDestructed == true) { return; }
        _hasDestructed = true;
        SpawnBrokenObject();
        SpawnDroppedLoot();
        Destroy(gameObject);
    }

    /// <summary>
    /// Checks whether the object is destroyed on impact.
    /// </summary>
    /// <param name="collision">
    /// Collision associated with the impact.
    /// </param>
    /// <returns>
    /// True if the object is hit with greater than _maxVelocityBeforeBreak and 
    /// it is meant to be broken on throw, false otherwise.
    /// </returns>
    private bool IsDestroyedOnImpact(Collision collision)
    {
        return collision.relativeVelocity.y > _maxVelocityBeforeBreak && _brokenOnThrow;
    }

    /// <summary>
    /// Checks whether the object is destroyed on attack.
    /// </summary>
    /// <param name="coliision">
    /// Collision associated with the impact.
    /// </param>
    /// <returns>
    /// True if the current health of the object is below zero and it is meant to
    /// be broken on attack, false otherwise.
    /// </returns>
    private bool IsDestroyedOnAttack(Collision coliision)
    {
        return _brokenOnAttack && _health.CurrentHealth <= 0;
    }

    /// <summary>
    /// Spawns the broken version of the object with an explosive force.
    /// </summary>
    private void SpawnBrokenObject()
    {
        if (_destroyedSpawn == null) { return; }
        GameObject go = Instantiate(_destroyedSpawn, transform.position, transform.rotation);
        foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(_explosiveForce, transform.position + _explosiveForceOffset, 
                _explosionRadius);
        }
    }

    /// <summary>
    /// Spawns the loot dropped by the broken object.
    /// </summary>
    private void SpawnDroppedLoot()
    {
        if (_dropSpawn != null)
        {
            Instantiate(_dropSpawn, transform.position, transform.rotation);
        }
    }
    #endregion
}
}
