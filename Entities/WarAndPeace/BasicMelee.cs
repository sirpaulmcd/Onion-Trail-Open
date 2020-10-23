using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This class represents a basic melee weapon.
/// </summary>
public class BasicMelee : MonoBehaviour, IWeapon
{
    //=========================================================================
    #region Instance variables
    //========================================================================= 
    // [Header("WAP Variables")]
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
    [SerializeField] private float _velocity = 360f;
    /// <summary>
    /// The name of the attack.
    /// </summary>
    [SerializeField] private string _name = "BasicMelee";
    /// <summary>
    /// The name of the GameObject that will parent all objects generated
    /// by this weapon. This keeps the GameObject hierarchy tidy.
    /// </summary>
    [SerializeField] private string _parentContainerName = "MeleeObjects";

    //=========================================================================
    // [Header("Weapon Specific Variables")]
    /// <summary>
    /// The current weaponized object being constructed.
    /// </summary>
    private GameObject _currentObject = default;
    /// <summary>
    /// The Rigidbody component of the current weaponized object.
    /// </summary>
    private Rigidbody _currentRigidbody = default;
    /// <summary>
    /// The PlayerController component used to halt player movement when 
    /// attacking.
    /// </summary>
    private PlayerController _playerController = default;
    /// <summary>
    /// The seconds before the object destroys itself.
    /// </summary>
    [SerializeField] private float _selfDestructSeconds = 0.25f;
    /// <summary>
    /// The offset in front of the player in which the weapon spawns. Offset 
    /// ensures swing arc is centered on facing direction rather than starting
    /// in front of the player. In this case, the object spawns at on offset of
    /// 45 degrees to the left of the players facing direction.
    /// </summary>
    private float _swingOffset = 45f;
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

    /// <summary>
    /// Called once per frame.
    /// </summary>
    private void Update()
    {
        RotateMeleeObject();
    }
    #endregion

    //=========================================================================
    #region Public methods
    //=========================================================================
    /// <summary>
    /// Swings a basic melee weapon in front of the player. Only one melee 
    /// attack can be used at a time and player movement is halted while
    /// the attack is swinging.
    /// </summary>
    /// <param name="direction">
    /// The direction of the attack.
    /// </param>
    public void Attack(Vector3 direction)
    {
        if (_currentObject == null)
        {
            CreateObject();
            ApplyWAP();
            SwingObject(direction);
            StartCoroutine(SelfDestruct(_currentObject));
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
        SetAttackerGameObject();
        _playerController = _attacker.GetComponent<PlayerController>();
    }
    
    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        Assert.IsNotNull(_attacker, gameObject.name + " is missing _attacker");
        Assert.IsNotNull(_playerController, gameObject.name + " is missing _playerController");
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
    private void CreateObject()
    {
        GameObject parentContainer = GameObject.Find(_parentContainerName);
        if(parentContainer == null) 
        {
            parentContainer = new GameObject(_parentContainerName);
        }
        _currentObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _currentObject.transform.parent = parentContainer.transform;
        _currentRigidbody = _currentObject.AddComponent<Rigidbody>();
        _currentObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    /// <summary>
    /// Applies the WAP system to the current working object such that it
    /// inflicts damage accordingly.
    /// </summary>
    private void ApplyWAP()
    {
        WarAndPeace wap = _currentObject.AddComponent<WarAndPeace>();
        wap.attacker = _attacker;
        wap.criticalHitMultiplier = _criticalHitMultiplier;
        wap.criticalHitChance = _criticalHitChance;
        wap.heal = _heal;
        wap.knockback = _knockback;
        // Since this is a melee weapon, the attacker should be the origin of
        // the knockback force. 
        wap.knockbackOrigin = _attacker;
        wap.minimumDamage = _minimumDamage;
        wap.maximumDamage = _maximumDamage;
        wap.name = _name;
    }

    /// <summary>
    /// Swings the current weaponized object in the input direction.
    /// </summary>
    /// <param name="direction">
    /// The direction that the object will swing.
    /// </param>
    private void SwingObject(Vector3 direction)
    {
        StartCoroutine(TemporarilyHaltPlayerController());
        // Teleport object in front of player offset by 45 degrees
        direction = Quaternion.Euler(0, -_swingOffset, 0) * direction;
        _currentObject.transform.position = transform.position + direction;
        _currentRigidbody.useGravity = false;
        // Once teleported to the proper location, object swings as per the
        // RotateMeleeObject method.
    }

    /// <summary>
    /// Rotates weaponized object (if any) radially around the y-axis of the 
    /// attacker.
    /// </summary>
    private void RotateMeleeObject()
    {
        if (_currentObject != null)
        {
            _currentObject.transform.RotateAround(_attacker.transform.position, Vector3.up, _velocity * Time.deltaTime);
        }
    }

    /// <summary>
    /// Halts player movement, waits for the attack to complete, then resumes
    /// player movement. Updates to animator variables are frozen to prevent
    /// the player sprite from turning while they are frozen in place.
    /// </summary>
    private IEnumerator TemporarilyHaltPlayerController()
    {
        _playerController.FreezeMovement = true;
        _playerController.IsAnimated = false;
        yield return new WaitForSeconds(_selfDestructSeconds);
        _playerController.FreezeMovement = false;
        _playerController.IsAnimated = true;
    }

    /// <summary>
    /// Destroys the GameObject associated with the attack after 
    /// _selfDestructSeconds assuming the attack does not hit anything.
    /// </summary>
    /// <param name="gameObject">
    /// The weaponized object to be destroyed.
    /// </param>
    private IEnumerator SelfDestruct(GameObject gameObject)
    {
        yield return new WaitForSeconds(_selfDestructSeconds);
        Destroy(gameObject);
    }
    #endregion
}
}