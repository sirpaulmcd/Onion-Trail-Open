using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents objects that can be lifted, carried, thrown. The
/// objects are responsible for throwing themselves because different throwing
/// forces may be required depending on the object.
/// </summary>
public class Liftable : MonoBehaviour, ILiftable
{
    //==========================================================================
    #region Instance variables
    //==========================================================================
    // [Header("Lifting")] 
    /// <summary>
    /// The height above the lifter centroid that the object origin is lifted.
    /// </summary>
    [SerializeField] protected float liftHeight = 1.1f;
    /// <summary>
    /// The magnitude of the impulse used to throw objects horizontally.
    /// </summary>
    [SerializeField] protected float throwSpeed = 100f;
    /// <summary>
    /// The magnitude of the impulse used to throw objects vertically.
    /// </summary>
    [SerializeField] protected float verticalThrowLift = 3.0f;
    /// <summary>
    /// Float used to control the inertial effects that a lifted object has
    /// on the player.
    /// </summary>
    [SerializeField] protected float connectedMassScale = 0.1f;

    //==========================================================================
    // [Header("Indicators")]
    /// <summary>
    /// Boolean that indicates whether an object is currently being lifted.
    /// </summary>
    protected bool isLifted = false;
    /// <summary>
    /// Indicates if object has been thrown and hasn't yet touched the ground.
    /// Used to lock player movement while thrown.
    /// </summary>
    protected bool isThrown = false;

    //==========================================================================
    // [Header("Components")] 
    /// <summary>
    /// The Rigidbody of this object. The new keyword is used to overwrite the
    /// obsolete Component.rigidbody.
    /// </summary>
    protected new Rigidbody rigidbody;

    /// <summary>
    /// The Lifter component of the lifter object.
    /// </summary>
    protected Lifter lifterObject;
    #endregion

    //==========================================================================
    #region Properties
    //==========================================================================
    /// <summary>
    /// Indicates if object is currently being lifted.
    /// </summary>
    public bool IsLifted
    {
        get { return lifterObject != null; }
    }

    /// <summary>
    /// Indicates if object is currently being thrown.
    /// </summary>
    public bool IsThrown
    {
        get { return isThrown; }
        set { isThrown = value; }
    }

    /// <summary>
    /// The Lifter component of the object lifting this object.
    /// </summary>
    public Lifter LifterObject
    {
        get { return lifterObject; }
        set 
        {
            if (value != lifterObject)
            {
                lifterObject = value; 
                if (value != null) { ApplyLiftingEffects(); }
                else { RemoveLiftingEffects(); }
            }
        }
    }
    #endregion
    
    //==========================================================================
    #region MonoBehaviour
    //==========================================================================
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
        AttemptToGround(collision);
    }
    #endregion

    //==========================================================================
    #region Public methods
    //==========================================================================
    /// <summary>
    /// Throws the gameObject up and in the direction the thrower is facing.
    /// </summary>
    /// <param name="facingDirection"> 
    /// Normalized Vector3 indicating direction thrower is facing.
    /// </param> 
    public void ThrowSelf(Vector3 facingDirection)
    {
        IsThrown = true;
        Vector3 throwDirection = facingDirection + 
                                 new Vector3(0, verticalThrowLift, 0);
        rigidbody.AddForce(throwDirection * throwSpeed, ForceMode.Impulse);
    }
    #endregion

    //==========================================================================
    #region Protected methods
    //==========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    protected void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Grounds the object if it touches a jumpable surface.
    /// </summary>
    /// <param name="collision">
    /// Collision holding information about colliding object.
    /// </param> 
    /// <remarks>
    /// This reliably grounds the player after they have been thrown in most
    /// cases. However, if the player is thrown but never leaves the floor, 
    /// isThrown will remain true. For players, IsJumpable() in 
    /// PlayerController is used to compensate for these cases.
    /// </remarks>
    protected void AttemptToGround(Collision other)
    {
        if (other.gameObject.GetComponent<Jumpable>()) { IsThrown = false; }
    }

    /// <summary>
    /// Applies lifting effects to the lifted object.
    /// </summary>
    protected void ApplyLiftingEffects()
    {
        HandlePlayerRevival();
        PrepForFixedJoint();
        CreateFixedJoint();
    }

    /// <summary>
    /// Removes lifting effects from the lifted object.
    /// </summary>
    protected void RemoveLiftingEffects()
    {
        // Toggle gravity on
        rigidbody.useGravity = true;
        // Reset velocity
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        // If lifted object does not have a sprite, unlock rotation
        if (!transform.Find("SpriteBody")) { rigidbody.constraints = RigidbodyConstraints.None; }
        // Destory fixedjoint
        Destroy(gameObject.GetComponent<FixedJoint>());  
    }
    #endregion

    //==========================================================================
    #region Private methods
    //==========================================================================
    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(rigidbody, gameObject.name + " is missing rigidbody");
    }

    /// <summary>
    /// If the object being lifted is a player, revives them from incapacitation.
    /// </summary>
    private void HandlePlayerRevival()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null && playerHealth.IsIncapacitated) { playerHealth.Revive(); }
    }

    /// <summary>
    /// Preps the lifted object accordingly before it is attached to the lifter
    /// via fixed joint.
    /// </summary>
    /// <remarks>
    /// In order to ensure that the lifted object is not picked up in a strange 
    /// orientation, the rotation of the object is reset upon lifting. An 
    /// unexpected rotation could allow the box collider of the lifted object 
    /// to touch that of the player resulting in bugs. Interpolation is enabled
    /// in the rigidbody to smooth movement while carrying.
    /// </remarks> 
    private void PrepForFixedJoint()
    {
        // Prep rigidbody
        transform.rotation = Quaternion.identity;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.useGravity = false;
        // Set transform liftHeight above player transform
        transform.position = lifterObject.transform.position + new Vector3(0f, liftHeight, 0f);
        // Set velocity to zero
        rigidbody.velocity = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Creates a FixedJoint connecting the lifted object to the lifter.
    /// </summary>
    /// <remarks>
    /// When the FixedJoint is created, the "connectedMassScale" of the joint 
    /// is set to a low number. This is to reduce the inertia effects that the 
    /// lifted object has on the lifter. Note that, if this value is too low, 
    /// collisions of the lifted object no longer limit the player's movement. 
    /// Gravity is also removed so that the lifted object does not weigh the 
    /// player down.
    /// </remarks>
    private void CreateFixedJoint()
    {
        // If lifted object is not already attached to something by fixed joint...
        if (!gameObject.GetComponent<FixedJoint>())
        {            
            // Connect bodies with a FixedJoint
            gameObject.AddComponent<FixedJoint>();
            FixedJoint fixedJoint = gameObject.GetComponent<FixedJoint>();
            fixedJoint.connectedBody = lifterObject.GetComponent<Rigidbody>();
            fixedJoint.connectedMassScale = connectedMassScale;
        }
    }
    #endregion
}
}