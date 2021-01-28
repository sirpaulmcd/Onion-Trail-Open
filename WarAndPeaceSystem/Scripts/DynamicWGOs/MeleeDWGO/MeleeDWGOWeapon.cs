using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// Instances of Piercing DWGO Weapon that spawns MeleeDWGOs.
/// </summary>
public class MeleeDWGOWeapon : APiercingDWGOWeapon, IMeleeDWGO
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    // [Header("IMeleeObject variables")] 
    /// <summary>
    /// The offset in front of the player in which the weapon spawns. Offset 
    /// ensures swing arc is centered on facing direction rather than starting
    /// in front of the player. Note that the MeleeObject may not appear to
    /// actually spawn exactly at the angle if it is moving quickly.
    /// </summary>
    [SerializeField] protected float swingOffsetAngle = 45f;

    //=========================================================================
    // [Header("MeleeGOWeaponBase variables")] 
    /// <summary>
    /// The melee DWGO prefab instantiated by this weapon.
    /// </summary>
    [SerializeField] protected GameObject meleeObjectPrefab = default;
    /// <summary>
    /// The PlayerController component used to halt player movement when 
    /// attacking.
    /// </summary>
    protected PlayerController playerController = default;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The offset in front of the player in which the weapon spawns. Offset 
    /// ensures swing arc is centered on facing direction rather than starting
    /// in front of the player. Note that the MeleeObject may not appear to
    /// actually spawn exactly at the angle if it is moving quickly.
    /// </summary>
    public float SwingOffsetAngle 
    {
        get => swingOffsetAngle;
        set => swingOffsetAngle = value; 
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Instantiates and initilaizes a self-regulating meleeObject prefab in
    /// front of the attacker in the attack direction.
    /// </summary>
    /// <param name="direction">
    /// The direction of the attack.
    /// </param>
    public override void Attack(Vector3 direction)
    {
        if (onCooldown) { return; }
        if (playerController != null) { StartCoroutine(TemporarilyHaltPlayerController()); } 
        GameObject meleeObject = Instantiate(meleeObjectPrefab, 
            this.Attacker.transform.position + direction * this.SpawnOffsetDistance, 
            Quaternion.identity);
        meleeObject.GetComponent<MeleeDWGO>().Init(this, this.Attacker, direction);
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected override void InitOnStart()
    {
        base.InitOnStart();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    protected override void InitVars()
    {
        base.InitVars();
        if (attacker.CompareTag("Player"))
        { 
            playerController = attacker.GetComponent<PlayerController>();
        }
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected override void CheckMandatoryComponents()
    {
        base.CheckMandatoryComponents();
        Assert.IsNotNull(meleeObjectPrefab, gameObject.name + " is missing meleeObjectPrefab");
    }

    /// <summary>
    /// Halts player movement, waits for the attack to complete, then resumes
    /// player movement. Updates to animator variables are frozen to prevent
    /// the player sprite from turning while they are frozen in place.
    /// </summary>
    private IEnumerator TemporarilyHaltPlayerController()
    {
        if (this.CooldownSeconds > 0) { StartCoroutine(base.Cooldown()); }
        playerController.FreezeMovement = true;
        playerController.IsAnimated = false;
        yield return new WaitForSeconds(this.SelfDestructSeconds);
        playerController.FreezeMovement = false;
        playerController.IsAnimated = true;
    }
    #endregion
}
}