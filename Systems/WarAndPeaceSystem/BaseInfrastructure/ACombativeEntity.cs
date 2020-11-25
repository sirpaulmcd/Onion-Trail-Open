// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents entities that participate in the combat system. As
/// such, they are susceptible to combat effects such as knockback and
/// explosive forces.
/// </summary>
public class ACombativeEntity : MonoBehaviour, IKnockbackable
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    // [Header("IKnockbackable variables")]
    /// <summary>
    /// Whether the GameObject is currently experiencing knockback effects.
    /// </summary>
    protected bool isKnockedBack = false;
    /// <summary>
    /// TODO
    /// </summary>
    protected Vector3 knockbackDirection = default;
    /// <summary>
    /// 
    /// </summary>
    protected float knockbackMagnitude = default;
    /// <summary>
    /// Whether or not the knockbackable ignores knockback effects.
    /// </summary>
    [SerializeField] protected bool ignoreKnockback = false;

    //=========================================================================
    // [Header("ACombativeEntity")]
    /// <summary>
    /// The Rigidbody component of the CombativeEntity.
    /// </summary>
    protected new Rigidbody rigidbody = default;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    // [Header("IKnockbackable properties")]
    /// <summary>
    /// Whether the knockbackable ignores knockback effects.
    /// </summary>
    public bool IgnoreKnockback 
    { 
        get => ignoreKnockback;
        set => ignoreKnockback = value;
    }

    /// <summary>
    /// Whether the GameObject is currently experiencing knockback effects.
    /// </summary>
    public bool IsKnockedBack
    {
        get { return isKnockedBack; }
        set 
        { 
            if (isKnockedBack != value)
            {
                if (value) { AddKnockbackEffects(); }
                else { RemoveKnockbackEffects(); }
                isKnockedBack = value;
            } 
        }
    }
    #endregion

    //=========================================================================
    #region MonoBehaviour
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    protected virtual void Start()
    {
        InitOnStart();
    }

    /// <summary>
    /// Called every physics update.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        HandleKnockback(); 
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Calls the knockback coroutine that knocks the knockbackable in the 
    /// input direction by the input magnitude.
    /// </summary>
    /// <param name="direction">
    /// The normalized directional Vector3 corresponding to the direction of
    /// knockback.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude at which the knockbackable is knocked back.
    /// </param>
    public virtual void Knockback(Vector3 direction, float magnitude)
    {
        if (!IgnoreKnockback) { StartCoroutine(SimulateKnockback(direction, magnitude)); }
    }
    #endregion

    //=========================================================================
    #region Protected methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected virtual void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    protected virtual void InitVars()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    protected virtual void CheckMandatoryComponents()
    {
        Assert.IsNotNull(rigidbody, gameObject.name + " is missing rigidbody");
    }

    /// <summary>
    /// Knocks the knockbackable back in the input direction by the input 
    /// magnitude.
    /// </summary>
    /// <param name="direction">
    /// The normalized directional Vector3 corresponding to the direction of
    /// knockback.
    /// </param>
    /// <param name="magnitude">
    /// The magnitude at which the knockbackable is knocked back.
    /// </param>
    protected virtual IEnumerator SimulateKnockback(Vector3 direction, float magnitude)
    {
        knockbackDirection = direction;
        knockbackMagnitude = magnitude;
        IsKnockedBack = true;
        yield return new WaitForSeconds(Constants.KNOCKBACKSECONDS);
        IsKnockedBack = false;
    }

    /// <summary>
    /// Adds knockback effects to the NavMeshAgent.
    /// </summary>
    protected virtual void AddKnockbackEffects()
    {  
        // To be overridden by children
    }

    /// <summary>
    /// Removes knockback effects from the NavMeshAgent. A null check is
    /// required because the GameObject may have been killed before reaching
    /// their knockback destination.
    /// </summary>
    protected virtual void RemoveKnockbackEffects()
    {
        knockbackDirection = default;
        knockbackMagnitude = default;
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
    }

    protected virtual void HandleKnockback()
    {
        if (IsKnockedBack)
        {
            rigidbody.velocity = new Vector3(knockbackDirection.x, rigidbody.velocity.y, knockbackDirection.z).normalized * knockbackMagnitude * Time.fixedDeltaTime;
        }
    }
    #endregion
}
}