using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
public class WarAndPeace : MonoBehaviour, IWarAndPeace
{
    //==========================================================================
    // Instance Variables
    //==========================================================================
    // Unfortunately, Unity cannot expose Properties in the inspector, so we 
    // need to expose all properties using a serialized private backing field.
    // https://forum.unity.com/threads/exposing-properties-in-the-inspector.471363/
    [SerializeField] private GameObject _attacker;
    [SerializeField] private float _criticalHitMultiplier;
    [SerializeField] private float _criticalHitChance;
    [SerializeField] private bool _heal;
    [SerializeField] private float _knockback;
    [SerializeField] private GameObject _knockbackOrigin;
    [SerializeField] private uint _minimumDamage;
    [SerializeField] private uint _maximumDamage;
    [SerializeField] private string _name;

    //==========================================================================
    // Properties
    //==========================================================================   
    /// <summary>
    /// The player/enemy GameObject who created the attack/heal.
    /// </summary>
    public GameObject attacker 
    { 
        set => _attacker = value; 
        get => _attacker; 
    }

    /// <summary>
    /// The number which multiplies the damage value upon a critical hit.
    /// </summary>
    public float criticalHitMultiplier 
    { 
        set => _criticalHitMultiplier = value; 
        get => _criticalHitMultiplier;
    }

    /// <summary>
    /// The percent chance [0.0, 100.0] that an attack/heal is a critical hit.
    /// </summary>
    /// <invariants>
    /// 0.0 <= criticalHitChance <= 100.0
    /// </invariants>
    public float criticalHitChance 
    { 
        set => _criticalHitChance = value; 
        get => _criticalHitChance; 
    }

    /// <summary>
    /// A flag which indicates if this is a heal (true) or an attack (false).
    /// </summary>
    public bool heal 
    { 
        set => _heal = value; 
        get => _heal; 
    }

    /// <summary>
    /// The distance that the attack/heal will push the affected GameObject.
    /// </summary>
    /// <remarks>
    /// For reference, players are 1 unit wide.
    /// </remarks>
    public float knockback 
    { 
        set => _knockback = value; 
        get => _knockback; 
    }

    /// <summary>
    /// A GameObject which the knockback pushes away from.
    /// </summary>
    /// <remarks>
    /// This will typically be the attacker for a melee strike, and the 
    /// projectile for a ranged strike.
    /// </remarks>
    public GameObject knockbackOrigin 
    { 
        set => _knockbackOrigin = value; 
        get => _knockbackOrigin; 
    }

    /// <summary>
    /// The lower-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    public uint minimumDamage 
    { 
        set => _minimumDamage = value; 
        get => _minimumDamage; 
    }

    /// <summary>
    /// The upper-bound damage value for an attack/heal.
    /// </summary>
    /// <invariants>
    /// minimumDamage <= maximumDamage
    /// </invariants>
    public uint maximumDamage 
    { 
        set => _maximumDamage = value; 
        get => _maximumDamage; 
    }

    /// <summary>
    /// The name of the attack/heal.
    /// </summary>
    // NOTE: The 'new' keyword is used because of this warning:
    //  warning CS0108: 'WarAndPeace.name' hides inherited member 'Object.name'. 
    //                  Use the new keyword if hiding was intended.
    new public string name 
    { 
        set => _name = value; 
        get => _name; 
    }

    //==========================================================================
    // Monobehaviour
    //==========================================================================  
    /// <summary>
    /// Check invariants before the first frame update.
    /// </summary>
    private void Start()
    {
        AssertInvariants();
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
        attackOrHeal(collision);
    }

    //==========================================================================
    // Public Methods
    //==========================================================================
    /// <summary>
    /// Performs clean-up and destroys parent GameObject. 
    /// </summary>
    public void DestroyAttack()
    {
        Destroy(this.gameObject);
    }

    //==========================================================================
    // Helper Methods
    //==========================================================================
    /// <summary>
    /// Asserts that the attack/heal's invariants are all valid.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when a parameter (or parameters) is not logically valid. 
    /// For instance, when maximumDamage < minimumDamage. 
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when a parameter's input valid is outside the range of valid
    /// values. For instance, when criticalHitChance > 100.0. 
    /// </exception>
    private void AssertInvariants()
    {
        Assert.IsNotNull(attacker);
        Assert.IsTrue(0 <= criticalHitChance && criticalHitChance <= 100.0);
        Assert.IsNotNull(knockbackOrigin);
        Assert.IsTrue(minimumDamage <= maximumDamage);
    }

    /// <summary>
    /// Returns true iff the collision is which a friendly GameObject. 
    /// </summary>
    /// <param name="collision">
    /// The object which collided with the attack.
    /// </param>
    private bool isFriendly(Collision collision)
    {
        return attacker.tag == collision.gameObject.tag;
    }

    /// <summary>
    /// Computes a value for the attack's damage (including crit.)
    /// </summary>
    private int computeDamage()
    {
        System.Random rng = new System.Random();

        // Determine if an attack is a critical hit.
        // We use rng.NextDouble(), which returns a value in the range [0.0, 1.0),
        // to construct an if() statement which is always false when 
        // criticalHitChance = 0.0, and always true when criticalHitChance = 100.
        bool isCriticalHit = criticalHitChance > rng.NextDouble() * 100.0;

        int damage = rng.Next((int)minimumDamage, (int)maximumDamage+1);
        if(isCriticalHit)
        {
            damage = (int)Math.Round(damage * criticalHitMultiplier);
        }
        return damage;
    }

    /// <summary>
    /// Logic to perform the attack.  
    /// </summary>
    /// <param name="collison">
    /// The object which collided with the attack.
    /// </param>
    private void attackOrHeal(Collision collision)
    {
        // Ignore collisions with objects lacking _health systems, since they
        // aren't meant to be attacked.
        IHealth healthComponent = collision.gameObject.GetComponent<IHealth>();
        if(healthComponent == null) return;
    
        Rigidbody collisionRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        
        int damage = computeDamage(); 
        bool friendly = isFriendly(collision);

        if(friendly && heal) 
        {
            healthComponent.Heal(damage);
            DestroyAttack();    
        }
        else if(!friendly && !heal)
        {
            healthComponent.Hurt(damage);

            // Apply knockback via teleporting the victim.
            // This seems to be the only good solution for obeying the requirement
            // that a knockback of 1 pushes the player exactly one unit. 
            Vector3 victimPos = collision.gameObject.transform.position;
            Vector3 attackerPos = knockbackOrigin.transform.position;
            Vector3 knockbackDir = Vector3.Normalize(victimPos - attackerPos);
            collisionRigidbody.MovePosition(collision.gameObject.transform.position 
                                            + knockbackDir * knockback);
            
            DestroyAttack();    
        }
        else{
            // Ignore the collision. This is either a heal travelling through 
            // an enemy, or an attack travelling through a friendly unit.
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(),
                                    collision.collider, false);
        }

        // Unity resolves collisions by applying forces to push the colliding
        // bodies apart. We want to nullify this in order to have precise 
        // attack knockback, and no movement if it was a non-attack collision.
        if(collisionRigidbody != null)
        {
            collisionRigidbody.AddForce(-collision.impulse, ForceMode.Impulse);
            collisionRigidbody.velocity = Vector3.zero;
        }
    }
}
}