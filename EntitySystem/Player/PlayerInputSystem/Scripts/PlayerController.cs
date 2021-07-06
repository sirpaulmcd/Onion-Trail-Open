using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace EGS
{
    /// <summary>
    /// This script is used to control the player. In order to listen to the events
    /// invoked by PlayerInput, this script must be placed on the same GameObject
    /// holding the PlayerInput component.
    /// </summary>
    public class PlayerController : AEntityController
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("Move action")]
        /// <summary>
        /// The direction of movement input (i.e. WASD or left stick input).
        /// </summary>
        private Vector2 _moveInputDirection;

        //=====================================================================
        // [Header("Jump action")]
        /// <summary>
        /// The PlayerInput system used to control the player.
        /// </summary>
        private PlayerInput _playerInput;
        /// <summary>
        /// The magnitude of the impulse used to jump the player.
        /// </summary>
        [SerializeField] protected float jumpMagnitude = 280f;
        /// <summary>
        /// Multiplier applied to player gravity when falling. I.e. whenever the
        /// player's vertical velocity is below zero. This provides a more
        /// "gamey" feel to the physics.
        /// </summary>
        [SerializeField] private float _fallMultiplier = 1.5f;
        /// <summary>
        /// Multiplier applied to player gravity when jump button is released
        /// prematurely. I.e. when the player's vertical velocity is positive but
        /// jump button is no longer being held. This lowers the peak of the
        /// player's jump and allows users to control the height of their jump with
        /// the duration that the jump button is held.
        /// </summary>
        [SerializeField] private float _prematureFallMultiplier = 0.5f;
        /// <summary>
        /// Whether the player is grounded for animation purposes.
        /// </summary>
        private bool _isGrounded = true;

        //=====================================================================
        // [Header("Point and look actions")]
        /// <summary>
        /// The Crosshair used to incidate where the player is aiming.
        /// </summary>
        private Crosshair _crosshair;
        /// <summary>
        /// The direction of right stick input.
        /// </summary>
        private Vector2 _rightStickInputDirection;
        /// <summary>
        /// The direction from the player to the on-screen mouse position.
        /// </summary>
        private Vector2 _playerToMouseInputDirection;
        /// <summary>
        /// The Vector3 direction that the player is facing.
        /// </summary>
        private Vector3 _facingDirection;
        /// <summary>
        /// Whether or not the player is being controlled with a gamepad.
        /// </summary>
        private bool _usingController = false;
        /// <summary>
        /// Whether or not the player is currently aiming. When the player is
        /// aiming, their facing direction is overwritten with the aiming direction.
        /// </summary>
        private bool _isAiming = false;
        /// <summary>
        /// The minimum value the stick needs to be moved to be recognized as input.
        /// </summary>
        [SerializeField] private float _minInputMagnitude = 0.1f;

        //=====================================================================
        // [Header("Scroll action")]
        /// <summary>
        /// The WeaponSelector used to select weapons.
        /// </summary>
        private WeaponSelector _weaponSelector;

        //=====================================================================
        // [Header("General action (lifting/interacting)")]
        /// <summary>
        /// The Lifter component used for carrying/throwing liftable objects.
        /// </summary>
        private Lifter _lifter;
        /// <summary>
        /// The SphereCaster used to perform spherecasting.
        /// </summary>
        private SphereCaster _sphereCaster;
        /// <summary>
        /// The maximum distance that the spherecast travels to check for lift.
        /// </summary>
        [SerializeField] private float _liftMaxDistance = 0.5f;
        /// <summary>
        /// The maximum distance that the spherecast travels to check for ground.
        /// </summary>
        [SerializeField] private float _floorMaxDistance = 0.5f;
        /// <summary>
        /// The amount of time after throwing the player that CarryRigidbodySensors
        /// are disabled. This prevents lifted players from "pairing" with their
        /// carrier while being thrown.
        /// </summary>
        private float _carryRigidbodySensorDownTime = 0.25f;
        /// <summary>
        /// The CarryRigidBodiesSensor child component of the player. Necessary
        /// since the GameObject must be enabled/disabled in under specific
        /// conditions. Simply enabling/disabling the CarryRigidBodies script is
        /// not enough because that only disabled MonoBehaviour methods.
        /// </summary>
        private GameObject _carryRigidbodiesSensorGameObject = default;

        //=====================================================================
        // [Header("Attack action")]
        /// <summary>
        /// If the player is pressing/holding the attack button.
        /// </summary>
        private bool _isAttacking = false;

        //=====================================================================
        // [Header("Animations")]
        /// <summary>
        /// The Animator used to update animator variables.
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// The angular direction (degrees on the x/z plane) that the player is
        /// facing. Used by the animator. The directions are as follows:
        /// - Up: 0
        /// - Left: -90
        /// - Right: 90
        /// - Down: +-180
        /// </summary>
        private float _animatorFacingDirection;
        /// <summary>
        /// Whether or not animation variables are refreshed each frame. Used to
        /// enable/disable animation updates.
        /// </summary>
        private bool _isAnimated = true;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("Jump action")]
        /// <summary>
        /// The magnitude of the impulse used to jump the player.
        /// </summary>
        public float JumpMagnitude
        {
            get => jumpMagnitude;
            set => jumpMagnitude = value;
        }

        /// <summary>
        /// Boolean used to indicate whether the player is grounded for animation
        /// purposes.
        /// </summary>
        private bool IsGrounded
        {
            get { return _isGrounded; }
            set
            {
                if (value != _isGrounded)
                {
                    if (value)
                    {
                        _isGrounded = true;
                        _lifter.IsThrown = false;
                    }
                    _isGrounded = value;
                }
            }
        }

        //=====================================================================
        // [Header("Point and look actions")]
        /// <summary>
        /// Vector3 for the _facingDirection variable that indicates the
        /// facing direction of the player
        /// </summary>
        public Vector3 FacingDirection
        {
            get { return _facingDirection; }
        }

        /// <summary>
        /// Boolean used to tell if the right stick is resting or not.
        /// </summary>
        private bool IsAiming
        {
            get { return _isAiming; }
            set
            {
                if (_isAiming != value)
                {
                    _crosshair.toggleCrosshair();
                    _isAiming = value;
                }
            }
        }

        //=====================================================================
        // [Header("General action (lifting/interacting)")]
        /// <summary>
        /// The maximum distance that the spherecast travels to check for ground.
        /// </summary>
        public float FloorMaxDistance
        {
            get { return _floorMaxDistance; }
        }

        //=====================================================================
        // [Header("Animations")]
        /// <summary>
        /// Boolean used to enable/disable sprite animation updates.
        /// </summary>
        public bool IsAnimated
        {
            set { _isAnimated = value; }
        }

        //=====================================================================
        // [Header("Movement freezing")]
        /// <summary>
        /// Boolean used to prevent the player from moving horizontally.
        /// </summary>
        public override bool FreezeMovement
        {
            get { return base.freezeMovement; }
            set
            {
                if (value != base.freezeMovement)
                {
                    if (value) { ZeroPlayerVelocity(); }
                    base.freezeMovement = value;
                }
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void Update()
        {
            UpdateAnimatorVariables();
            AdjustFacingDirection();
        }

        protected virtual void FixedUpdate()
        {
            MovePlayer();
            AdjustJumpingGravity();
            AttackOnButtonHold();
        }

        private void OnCollisionEnter(Collision collision)
        {
            AttemptToGround(collision);
        }
        #endregion

        //=====================================================================
        #region InputSystem event handlers
        //=====================================================================
        /// <summary>Called when InputSystem detects Move action.</summary>
        /// <param name="value">The input detected by the input system.</param>
        private void OnMove(InputValue value) { ProcessMoveInput(value); }

        /// <summary>Called when InputSystem detects Point action.</summary>
        /// <param name="value">The input detected by the input system.</param>
        private void OnPoint(InputValue value) { ProcessPointInput(value); }

        /// <summary>Called when InputSystem detects Look action.</summary>
        /// <param name="value">The input detected by the input system.</param>
        private void OnLook(InputValue value) { ProcessLookInput(value); }

        /// <summary>Called when InputSystem detects Scroll action.</summary>
        /// <param name="value">The input detected by the input system.</param>
        private void OnScrollWeapon(InputValue value) { ProcessScrollWeaponInput(value); }

        /// <summary>Called when InputSystem detects Attack action.</summary>
        /// <param name="value">The input detected by the input system.</param>
        private void OnAttack() { ProcessAttack(); }

        /// <summary>Called when InputSystem detects NextWeapon action.</summary>
        private void OnNextWeapon() { IncrementWeaponIndex(); }

        /// <summary>Called when InputSystem detects PreviousWeapon action.</summary>
        private void OnPreviousWeapon() { DecrementWeaponIndex(); }

        /// <summary>Called when InputSystem detects Jump action.</summary>
        private void OnJump() { JumpPlayer(); }

        /// <summary>Called when InputSystem detects Aim action.</summary>
        private void OnAim() { ToggleAim(); }

        /// <summary>Called when InputSystem detects Action action.</summary>
        private void OnAction() { Action(); }

        /// <summary>Called when InputSystem detects Drop action.</summary>
        private void OnDrop() { DropLiftedObjects(); }

        /// <summary>Called when InputSystem detects Pause action.</summary>
        private void OnPause() { PauseGame(); }

        /// <summary>Called when InputSystem detects NextLine action.</summary>
        private void OnNextLine() { SkipDialogueLine(); }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        protected override void InitVars()
        {
            base.InitVars();
            // Initialize GameObject components
            _animator = GetComponent<Animator>();
            _lifter = GetComponent<Lifter>();
            _playerInput = GetComponent<PlayerInput>();
            _sphereCaster = GetComponent<SphereCaster>();
            // Initialize child components
            _carryRigidbodiesSensorGameObject = GetComponentInChildren<CarryRigidbodiesSensor>().gameObject;
            _crosshair = PlayerManager.Instance.GetPlayerInstance(gameObject).Crosshair;
            _weaponSelector = GetComponentInChildren<WeaponSelector>();
            // Initialize control scheme
            _usingController = _playerInput.currentControlScheme != "KeyboardMouse";
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        protected override void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_animator, gameObject.name + " is missing _animator");
            Assert.IsNotNull(_carryRigidbodiesSensorGameObject, gameObject.name + " is missing _carryRigidbodiesSensorGameObject");
            Assert.IsNotNull(_crosshair, gameObject.name + " is missing _crosshair");
            Assert.IsNotNull(_lifter, gameObject.name + " is missing _lifter");
            Assert.IsNotNull(_playerInput, gameObject.name + " is missing _playerInput");
            Assert.IsNotNull(_sphereCaster, gameObject.name + " is missing _sphereCaster");
            Assert.IsNotNull(_weaponSelector, gameObject.name + " is missing _weaponSelector");
        }
        #endregion

        //=====================================================================
        #region Input zeroing
        //=====================================================================
        /// <summary>
        /// Zeros player inputs such that they do not get locked into movement
        /// when their input action map is changed.
        /// </summary>
        public void ZeroPlayerInputs()
        {
            _moveInputDirection = new Vector2(0, 0);
        }

        /// <summary>
        /// Zeros the player's velocity. To be called when horizontal movement is
        /// to be halted.
        /// </summary>
        private void ZeroPlayerVelocity()
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
        }
        #endregion

        //=====================================================================
        #region Move action
        //=====================================================================
        /// <summary>
        /// Processes Move input such that the player moves with respect to the
        /// camera's current perspective. For example, when a player walks left,
        /// they should walk left with respect to the camera.
        /// </summary>
        /// <param name="value">
        /// The value provided by the InputSystem to represent WASD or left stick
        /// movement.
        /// </param>
        private void ProcessMoveInput(InputValue value)
        {
            // Get Vector2 movement input
            Vector2 input = value.Get<Vector2>();
            // Rotate input with respect to camera perspective and store
            float yrad = Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            _moveInputDirection = new Vector2(
                input.x * Mathf.Cos(yrad) + input.y * Mathf.Sin(yrad),
                input.y * Mathf.Cos(yrad) - input.x * Mathf.Sin(yrad));
        }

        /// <summary>
        /// Moves player in input direction if they are not lifted, thrown, frozen,
        /// or being knocked back.
        /// </summary>
        /// <remarks>
        /// Movement direction is not normalzied so that people using sticks can
        /// vary their speed.
        /// </remarks>
        private void MovePlayer()
        {
            if (!_lifter.IsLifted && !_lifter.IsThrown && !FreezeMovement)
            {
                Vector3 moveVelocity = new Vector3(_moveInputDirection.x, 0, _moveInputDirection.y) * base.moveSpeed * Time.deltaTime;
                rigidbody.velocity = new Vector3(moveVelocity.x, rigidbody.velocity.y, moveVelocity.z);
            }
        }
        #endregion

        //=====================================================================
        #region Scroll action
        //=====================================================================
        /// <summary>
        /// Processes Scroll input to determine whether to increment or decrement
        /// the weapon index.
        /// </summary>
        /// <param name="value">
        /// The value provided by the InputSystem to mouse scroll movement.
        /// </param>
        private void ProcessScrollWeaponInput(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            if (input.y > 0) { IncrementWeaponIndex(); }
            else if (input.y < 0) { DecrementWeaponIndex(); }
        }

        /// <summary>
        /// Increments the weapon index. Cannot be changed while the weapon is
        /// being fired.
        /// </summary>
        private void IncrementWeaponIndex()
        {
            if (!FreezeMovement) { _weaponSelector.IncrementWeaponIndex(); }
        }

        /// <summary>
        /// Decrements the weapon index. Cannot be changed while the weapon is
        /// being fired.
        /// </summary>
        private void DecrementWeaponIndex()
        {
            if (!FreezeMovement) { _weaponSelector.DecrementWeaponIndex(); }
        }
        #endregion

        //=====================================================================
        #region Attack action
        //=====================================================================
        /// <summary>
        /// Inverts _isAttacking on button press and release such that _isAttacking
        /// is true when button is held down.
        /// </summary>
        private void ProcessAttack()
        {
            _isAttacking = !_isAttacking;
            if (_isAttacking && _weaponSelector.SingleFire)
            {
                Attack();
            }
        }

        private void AttackOnButtonHold()
        {
            if (_isAttacking && !_weaponSelector.SingleFire) { Attack(); }
        }

        /// <summary>
        /// Attacks using the currently selected weapon. Attacks can only be made
        /// when the player is not lifting anything.
        /// </summary>
        private void Attack()
        {
            if (!FreezeMovement && _lifter.LiftedObject == null)
            {
                _weaponSelector.UseSelectedWeapon(_facingDirection);
            }
        }
        #endregion

        //=====================================================================
        #region Jump action
        //=====================================================================
        /// <summary>
        /// Adds vertical impulse to the player causing them to jump.
        /// </summary>
        /// <remarks>
        /// The player's velocity is reset before jumping so that they cannot spam
        /// the jump button and "multi-jump" to get a larger jumping impulse. It
        /// also helps to prevent players from using FixedJoints as a spring to
        /// build up platforming momentum.
        /// </remarks>
        private void JumpPlayer()
        {
            if (!_lifter.IsLifted && !_lifter.IsThrown && !this.FreezeMovement && IsJumpableSurface())
            {
                rigidbody.velocity = new Vector3(0, 0, 0);
                rigidbody.AddForce(Vector3.up * jumpMagnitude, ForceMode.Impulse);
                IsGrounded = false;
            }
        }

        /// <summary>
        /// Sends a spherecast directly below the player to see if they are on a
        /// jumpable surface. If they are, _isGrounded is set to true.
        /// </summary>
        /// <remarks>
        /// This prevents players from walking off an edge and then jumping. It
        /// also prevents players from getting stuck without a jump on a jumpable
        /// surface. Some people check for _isGrounded by sending a vertical
        /// spherecast on every frame but, for our purposes, that seems a little
        /// overkill.
        /// </remarks>
        /// <returns>
        /// Returns true if spherecast touches a gameObject holding a Jumpable
        /// component, false otherwise.
        /// </returns>
        private bool IsJumpableSurface()
        {
            GameObject floor = _sphereCaster.SphereCast(Vector3.down, _floorMaxDistance);
            if (floor != null && floor.GetComponent<Jumpable>() != null) { return IsGrounded = true; }
            else { return IsGrounded = false; }
        }

        /// <summary>
        /// Toggles IsGrounded if the player collides with an object considered as
        /// a jumpable surface.
        /// </summary>
        /// <param name="other">
        /// Collision associated with GameObject touching the player.
        /// </param>
        /// <remarks>
        /// This reliably grounds the player after they have jumped in most cases.
        /// However, if the player presses the jump input but never leaves the
        /// floor, IsGrounded will remain false. IsJumpable() is used to
        /// compensate for these cases.
        /// </remarks>
        private void AttemptToGround(Collision other)
        {
            if (other.gameObject.GetComponent<Jumpable>() != null) { IsGrounded = true; }
        }

        /// <summary>
        /// Increases the gravity of the player so they fall faster during a jump.
        /// Also increases gravity if jump button is released prematurely to allow
        /// the player to better control the height of their jump.
        /// </summary>
        /// <remarks>
        /// Following vanilla Unity physics, when they player jumps, they simulate
        /// real world physics and travel in a parabola with the peak of the jump
        /// being in the center. This is cool. However, in terms of gameplay, it
        /// results in a "floaty" jump that doesn't feel good. This method fixes
        /// that problem by dynamically changing how gravity acts on a player as
        /// they jump to get a better "gamey" feel with more accurate control.
        /// </remarks>
        private void AdjustJumpingGravity()
        {
            if (!_lifter.IsThrown)
            {
                // If player is falling...
                if (rigidbody.velocity.y < 0)
                {
                    // Increase player gravity by _fallMultiplier (i.e. increase fall speed)
                    rigidbody.AddForce(Physics.gravity * rigidbody.mass * _fallMultiplier);
                }
                // If player is jumping upward but the jump button is no longer held...
                else if (rigidbody.velocity.y > 0 && _playerInput.actions["Jump"].ReadValue<float>() == 0)
                {
                    // Increase player gravity by _prematureFallMultiplier (i.e. lower jump peak)
                    rigidbody.AddForce(Physics.gravity * rigidbody.mass * _prematureFallMultiplier);
                }
            }
        }
        #endregion

        //=====================================================================
        #region General action (lifting/interacting)
        //=====================================================================
        /// <summary>
        /// Attempts to perform an action with object if player not holding
        /// anything. Otherwise, throws object that player is holding.
        /// </summary>
        private void Action()
        {
            // If player is not lifting something, attempt object action.
            if (_lifter.LiftedObject == null) { AttemptAction(); }
            // If player is lifting something, throw.
            else { Throw(); }
        }

        /// <summary>
        /// Attempts to perform an action with object in front of player. A
        /// spherecast is sent out in front of the player. If it collides with a
        /// Liftable object, the player lifts. If it collides with an Interactable
        /// object, the player interacts. Otherwise, the player does nothing.
        /// </summary>
        private void AttemptAction()
        {
            GameObject other = _sphereCaster.SphereCast(_facingDirection, _liftMaxDistance);
            if (other != null && !AttemptLift(other)) { AttemptInteract(other); }
        }

        /// <summary>
        /// Lifts the input GameObject if it is liftable and has not already been
        /// lifted.
        /// </summary>
        /// <param name="other">
        /// The GameObject to be lifted.
        /// </param>
        /// <returns>
        /// True if the object was liftable and was not already lifted, false
        /// otherwise.
        /// </returns>
        private bool AttemptLift(GameObject other)
        {
            Liftable liftable = other.GetComponent<Liftable>();
            if (liftable != null && !liftable.IsLifted)
            {
                // Disable CarryRigidbodies sensor so lifted player does not get paired
                _carryRigidbodiesSensorGameObject.SetActive(false);
                // Lift object
                _lifter.Lift(other.GetComponent<Liftable>());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to interact with the input GameObject.
        /// </summary>
        /// <param name="other">
        /// The GameObject to interact with.
        /// </param>
        /// <returns>
        /// True if the GameObject was interactable, false otherwise.
        /// </returns>
        private bool AttemptInteract(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this.gameObject);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws the object that the player is currently lifting. Also starts
        /// coroutine to re-enable CarryRigidbodySensor.
        /// </summary>
        private void Throw()
        {
            StartCoroutine(EnableCarryRigidbodySensor());
            _lifter.Throw(_facingDirection);
        }

        /// <summary>
        /// Waits for a moment before enabling _carryRigidbodiesSensor. Used to
        /// prevent lifted players from "pairing" with their carriers while being
        /// thrown.
        /// </summary>
        private IEnumerator EnableCarryRigidbodySensor()
        {
            yield return new WaitForSeconds(_carryRigidbodySensorDownTime);
            _carryRigidbodiesSensorGameObject.SetActive(true);
        }
        #endregion

        //=====================================================================
        #region Drop action
        //=====================================================================
        /// <summary>
        /// Makes player drop what they are carrying, if anything.
        /// </summary>
        public void DropLiftedObjects()
        {
            if (_lifter != null)
            {
                // Drop lifted object
                _lifter.Drop();
                // Start coroutine to re-enable CarryRigidbodySensor
                StartCoroutine(EnableCarryRigidbodySensor());
            }
        }
        #endregion

        //=====================================================================
        #region Pause action
        //=====================================================================
        /// <summary>
        /// Pauses the game.
        /// </summary>
        private void PauseGame()
        {
            GameManager.Instance.TogglePause(gameObject);
        }
        #endregion

        //=====================================================================
        #region UI actions
        //=====================================================================
        /// <summary>
        /// Attempts to progress the dialogue sequence to the next line. Part of
        /// the "Dialogue" input action map.
        /// </summary>
        private void SkipDialogueLine()
        {
            UIManager.Instance.DialogueCanvas.ContinueDialogue();
        }
        #endregion

        //=====================================================================
        #region Animations
        //=====================================================================
        /// <summary>
        /// Updates variables necessary for the animator to determine appropriate
        /// animations.
        /// </summary>
        private void UpdateAnimatorVariables()
        {
            if (_isAnimated)
            {
                _animator.SetBool("isGrounded", _isGrounded);
                _animator.SetFloat("movementMagnitude", _moveInputDirection.magnitude);
                _animator.SetFloat("facingDirection", _animatorFacingDirection);
            }
        }
        #endregion

        //=====================================================================
        #region Point (keyboard) and look (controller) actions
        //=====================================================================
        /// <summary>
        /// Process Point input to determine which direction the player must face
        /// in order to face towards the mouse. Aslo positions the crosshair
        /// over the mouse position.
        /// </summary>
        /// <param name="value">
        /// The value provided by the InputSystem to represent mouse movement.
        /// </param>
        private void ProcessPointInput(InputValue value)
        {
            Vector2 input = value.Get<Vector2>();
            Vector3 mousePos = Camera.main.ScreenToViewportPoint(new Vector3(input.x, input.y, 0));
            Vector3 playerPos = Camera.main.WorldToViewportPoint(this.transform.position);
            _playerToMouseInputDirection = Vector3.Normalize(mousePos - playerPos);
            _crosshair.PositionCrosshairOnMouseMove(input);
        }

        /// <summary>
        /// Processes Look input to determine which direction the player must face
        /// to mirror right stick movement.
        /// </summary>
        /// <param name="value">
        /// The value provided by the InputSystem to represent right stick movement.
        /// </param>
        private void ProcessLookInput(InputValue value)
        {
            _rightStickInputDirection = value.Get<Vector2>();
            if (_rightStickInputDirection.magnitude > _minInputMagnitude)
            {
                IsAiming = true;
                _crosshair.PositionCrosshairOnStickMove(transform.position, _rightStickInputDirection);
            }
            else { IsAiming = false; }
        }

        /// <summary>
        /// Toggles _isAiming to true when shift is held down. Sets it back when
        /// shift is released.
        /// </summary>
        private void ToggleAim()
        {
            IsAiming = !IsAiming;
        }

        /// <summary>
        /// Faces player in the appropriate direction based on user input.
        /// </summary>
        private void AdjustFacingDirection()
        {
            // If aiming on a controller, face right stict direction
            if (_usingController && IsAiming) { FaceDirection(_rightStickInputDirection); }
            // If aiming on a keyboard/mouse, face mouse direction
            else if (IsAiming) { FaceDirection(_playerToMouseInputDirection); }
            // Otherwise, face direction of movement
            else if (_moveInputDirection.sqrMagnitude > _minInputMagnitude)
            {
                FaceDirection(_moveInputDirection);
            }
        }

        /// <summary>
        /// Faces player in direction of input vector with respect to the main
        /// camera. For example, if a left input is recieved, the player should face
        /// left with respect to the camera.
        /// </summary>
        /// <param name="value">The direction input for the player.</param>
        /// <remarks>
        /// Facing direction is normalized because we do not want it to change
        /// with varying input magnitude. Only input direction matters.
        /// </remarks>
        private void FaceDirection(Vector3 input)
        {
            // Rotate input with respect to camera perspective and store
            float yrad = Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
            _facingDirection = new Vector3(
                input.x * Mathf.Cos(yrad) + input.y * Mathf.Sin(yrad),
                0,
                input.y * Mathf.Cos(yrad) - input.x * Mathf.Sin(yrad)).normalized;
            // Update animation variables
            _animatorFacingDirection = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
        }
        #endregion
    }
}
