using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instance of Piercing DWGO Weapon that spawns BulletDWGOs.
/// </summary>
public class BulletDWGOWeapon : APiercingDWGOWeapon, IBulletDWGO
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    // [Header("IBulletDWGO variables")] 
    /// <summary>
    /// The LayerMask that the raycast can detect. To set the LayerMask to 
    /// detect everything, initialize as -1. In order for DWGO to work, the 
    /// 'Ignore Raycast' layermask must be deselected through the inspector.
    /// </summary>
    [SerializeField] protected LayerMask layerMask = -1;
    /// <summary>
    /// The maximum distance that the DWGO raycast will travel.
    /// </summary>
    [SerializeField] protected float maxRaycastDistance = 100f; 
    /// <summary>
    /// The seconds between raycasts used to collect reflection information.
    /// </summary>
    [SerializeField] protected float raycastRefreshSeconds = 0.1f;
    /// <summary>
    /// The number of times the DWGO can ricochet off of objects in the 
    /// environment.
    /// </summary>
    [SerializeField] protected int ricochetCount = 0;

    //=========================================================================
    // [Header("BulletDWGOWeapon variables")] 
    /// <summary>
    /// The bullet DWGO prefab instantiated by this weapon.
    /// </summary>
    [SerializeField] private GameObject _bulletPrefab = default;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("IBulletDWGO properties")] 
    /// <summary>
    /// The LayerMask that the raycast can detect. To set the LayerMask to 
    /// detect everything, initialize as -1. In order for DWGO to work, the 
    /// 'Ignore Raycast' layermask must be deselected through the inspector.
    /// </summary>
    public LayerMask LayerMask 
    { 
        get => layerMask;
        set => layerMask = value;
    }
    /// <summary>
    /// The maximum distance that the DWGO raycast will travel.
    /// </summary>
    public float MaxRaycastDistance
    { 
        get => maxRaycastDistance;
        set => maxRaycastDistance = value;
    }
    /// <summary>
    /// The seconds between raycasts used to collect reflection information.
    /// </summary>
    public float RaycastRefreshSeconds
    { 
        get => raycastRefreshSeconds;
        set => raycastRefreshSeconds = value;
    }
    /// <summary>
    /// The number of times the DWGO can ricochet off of objects in the 
    /// environment.
    /// </summary>
    public int RicochetCount 
    { 
        get => ricochetCount;
        set => ricochetCount = value;
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Instantiates and initilaizes a self-regulating BulletDWGO prefab in 
    /// front of the attacker in the attack direction.
    /// </summary>
    /// <param name="direction">
    /// The direction that the DWGO will fly.
    /// </param>
    /// <remarks>
    /// Currently players can shoot through walls by instantiating projectiles
    /// inside of the wall. This will need to be fixed if it ends up being a
    /// problem. See Physics.CheckSphere.
    /// </remarks>
    public override void Attack(Vector3 direction)
    {
        if (onCooldown) { return; }
        if (this.CooldownSeconds > 0) { StartCoroutine(base.Cooldown()); }
        GameObject bulletDWGO = Instantiate(_bulletPrefab, 
            this.Attacker.transform.position + direction * this.SpawnOffsetDistance, 
            Quaternion.identity);
        bulletDWGO.GetComponent<BulletDWGO>().Init(this, this.Attacker, direction);
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
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.CheckMandatoryComponents();
        Assert.IsNotNull(_bulletPrefab, gameObject.name + " is missing _bulletPrefab");
    }
    #endregion
}
}