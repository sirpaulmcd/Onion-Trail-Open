using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents a basic melee weapon.
/// </summary>
public class BasicRanged : MonoBehaviour, IWeapon
{
    //=========================================================================
    #region Instance variables
    //========================================================================= 
    // [Header("WAP variables")]
    /// <summary>
    /// The player GameObject that initiated the attack.
    /// </summary>
    private GameObject _attacker = default;
    /// <summary>
    /// The number which multiplies the damage value upon a critical hit.
    /// </summary>
    [SerializeField] private float _criticalHitMultiplier = 1f;
    /// <summary>
    /// The percent chance [0.0, 100.0] that an attack is a critical hit.
    /// </summary>
    [SerializeField] private float _criticalHitChance = 0f;
    /// <summary>
    /// A flag which indicates if this is a heal (true) or an attack (false).
    /// </summary>
    [SerializeField] private bool _heal = false;
    /// <summary>
    /// The distance that the attack will push the affected GameObject.  
    /// </summary>
    [SerializeField] private float _knockback = 0.5f;
    /// <summary>
    /// The lower-bound damage value for an attack.
    /// </summary>
    [SerializeField] private uint _minimumDamage = 10;
    /// <summary>
    /// The upper-bound damage value for an attack.
    /// </summary>
    [SerializeField] private uint _maximumDamage = 50;
    /// <summary>
    /// The velocity of the weaponized object.
    /// </summary>
    [SerializeField] private float _velocity = 800f;
    /// <summary>
    /// The name of the attack.
    /// </summary>
    [SerializeField] private string _name = "BasicRanged";
    /// <summary>
    /// The name of the GameObject that will parent all objects generated
    /// by this weapon. This keeps the GameObject hierarchy tidy.
    /// </summary>
    [SerializeField] private string _parentContainerName = "Projectiles";

    //=========================================================================
    // [Header("Weapon Specific Variables")]
    /// <summary>
    /// The seconds before the object destroys itself.
    /// </summary>
    [SerializeField] private float _selfDestructSeconds = 2f;
    /// <summary>
    /// The minimum velocity the projectile can have before self destructing.
    /// </summary>
    [SerializeField] private float _minimumVelocity = 5f;
    /// <summary>
    /// The number of seconds between velocity checks for self destruct.
    /// </summary>
    [SerializeField] private float _velocityCheckRefreshSeconds = 0.1f;
    /// <summary>
    /// Boolean indicating whether weapon can currently fire. 
    /// </summary>
    private bool _onCooldown = false;
    /// <summary>
    /// The minimum number of seconds between weapon uses.
    /// </summary>
    private float _cooldownSeconds = 0.2f;
    #endregion

    //=========================================================================
    #region MonoBehaviour 
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Launches a basic ranged projectile in the input direction.
    /// </summary>
    /// <param name="direction">
    /// The direction that the projectile will fly.
    /// </param>
    public void Attack(Vector3 direction)
    {
        if (!_onCooldown)
        {
            StartCoroutine(Cooldown());
            GameObject projectile = CreateObject();
            Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();
            ApplyWAP(projectile);
            LaunchObject(projectile, rigidbody, direction);
            StartCoroutine(SelfDestruct(projectile, rigidbody));
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
        SetAttackerGameObject();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_attacker, gameObject.name + " is missing _attacker");
    }

    /// <summary>
    /// Sets the GameObject belonging to the player holding the weapon.
    /// </summary>
    private void SetAttackerGameObject()
    {
        Transform t = transform;
        while (t.parent != null)
        {
            if (t.parent.CompareTag("Player"))
            {
                _attacker = t.parent.gameObject; 
                return;
            }
            t = t.parent.transform;
        }
        Debug.Log("Attacker not found!");
    }

    /// <summary>
    /// Creates a GameObject to hold the attack.
    /// </summary>
    private GameObject CreateObject()
    {
        GameObject parentContainer = GameObject.Find(_parentContainerName);
        if(parentContainer == null) 
        {
            parentContainer = new GameObject(_parentContainerName);
        }
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.parent = parentContainer.transform;
        projectile.AddComponent<Rigidbody>();
        projectile.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        return projectile;
    }

    /// <summary>
    /// Applies the WAP system to the current working object such that it
    /// inflicts damage accordingly.
    /// </summary>
    private void ApplyWAP(GameObject projectile)
    {
        WarAndPeace wap = projectile.AddComponent<WarAndPeace>();
        wap.attacker = _attacker;
        wap.criticalHitMultiplier = _criticalHitMultiplier;
        wap.criticalHitChance = _criticalHitChance;
        wap.heal = _heal;
        wap.knockback = _knockback;
        // Since this is a ranged weapon, the projectile should be the origin 
        // of the knockback force
        wap.knockbackOrigin = projectile;
        wap.minimumDamage = _minimumDamage;
        wap.maximumDamage = _maximumDamage;
        wap.name = _name;
    }

    /// <summary>
    /// Launches the current weaponized object in the input direction.
    /// </summary>
    /// <param name="direction">
    /// The direction that the object will fly.
    /// </param>
    private void LaunchObject(GameObject projectile, Rigidbody rigidbody, Vector3 direction)
    {
        projectile.transform.position = transform.position + direction;
        rigidbody.useGravity = false;
        rigidbody.velocity = direction * _velocity * Time.deltaTime;
    }

    /// <summary>
    /// Destroys the GameObject associated with the attack after 
    /// _selfDestructSeconds assuming the attack does not hit anything.
    /// If it hits a wall and slows, it will self destruct.
    /// </summary>
    /// <param name="gameObject">
    /// The weaponized object to be destroyed.
    /// </param>
    private IEnumerator SelfDestruct(GameObject projectile, Rigidbody rigidbody)
    {
        float i = 0;
        while (i < _selfDestructSeconds)
        {
            if (rigidbody != null && rigidbody.velocity.magnitude < _minimumVelocity)
            {
                break;
            }
            yield return new WaitForSeconds(_velocityCheckRefreshSeconds);
            i += _velocityCheckRefreshSeconds;
        }
        Destroy(projectile);
    }

    /// <summary>
    /// Toggles _onCooldown boolean for _cooldownSeconds.
    /// </summary>
    private IEnumerator Cooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_cooldownSeconds);
        _onCooldown = false;
    }
    #endregion
}
}