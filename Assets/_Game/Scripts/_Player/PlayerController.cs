using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    // Forward lock variables
    public bool isTargetLockEnabled = false; // Toggle for forward locking mode
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float doubleJumpForce = 8f;
    [SerializeField] private float fallGravityMultiplier = 2.5f; // Increases gravity when falling
    [SerializeField] private float lowJumpGravityMultiplier = 2f; // Extra gravity for shorter jumps
    [SerializeField] private float maxFallSpeed = -20f; // Terminal fall velocity
    [SerializeField] private float coyoteTime = 0.2f; // Coyote time duration
    
    [Header("Dodge")]
    [SerializeField] private float dodgeRollSpeed = 10f;
    [SerializeField] private float dodgeRollTime = 0.4f;
    [SerializeField] private float dodgeRollCooldown = 1f;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    
    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 0.3f;  // Time between attacks
    [SerializeField] private int maxComboCount = 3;        // Maximum combo chain
    [SerializeField] private float comboResetTime = 1.0f;  // Time to reset combo if no new attack
    
    [Header("Hit/Recoil")]
    [SerializeField] private float stunDuration = 1.5f;     // Duration of stun when player is stunned
    [SerializeField] private float stunChance = 0.3f;       // Probability of getting stunned (30% chance)
    [SerializeField] private float invulnerabilityTime = 1.0f; // Time during which player can't get hit after taking damage
    [SerializeField] private float stunnedInvulnerabilityTime = 1.0f; // Time during which player can't get hit after taking damage
    
    
    
    private Rigidbody _rb;
    private PlayerStats _playerStats;
    private PlayerAnimationController _animController;

    // Internal variables
    private bool isFacingRight = true;
    private float coyoteTimer = 0f;
    
    // dash/dodge roll variables
    private float lastDash = -1f;
    private float lastDodge = -1f;

    // Attack variables
    private int comboStep = 0;             // Current attack step in combo
    private float lastAttackTime = 0f;     // Tracks the last time an attack occurred
    private float comboResetTimer = 0f;    // Timer for resetting the combo if no further attacks

    // Hit variables
    private float lastHitTime = 0f;       // Time when the player was last hit

    
    // Variables for storing input
    private float moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool dodgeInput;
    private bool attackInput;
    private bool blockInput;

    
    public bool IsAttacking { get; private set; } = false;

    public bool IsGrounded { get; private set; }

    public bool CanDoubleJump { get; private set; }

    public bool IsDodging { get; private set; } = false;

    public bool IsDashing { get; private set; } = false;

    public bool IsRunning { get; private set; } = false;

    public bool IsStunned { get; private set; } = false;

    public bool IsInvulnerable { get; private set; } = false;

    public bool IsBlocking { get; private set; } = false;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerStats = GetComponent<PlayerStats>();
        _animController = GetComponent<PlayerAnimationController>(); // Reference the animation controller
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(_playerStats.IsDead) return;
        // Ground check and coyote timer logic
        CheckGround();
        CheckCayoteTimer();
        // Gather input
        CheckInput();
    }

    private void CheckInput()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
            jumpInput = true;
        if (Input.GetKeyDown(KeyCode.LeftControl))
            dashInput = true;
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            dodgeInput = true;
        if (Input.GetKeyDown(KeyCode.Mouse0))  // Left mouse button for attack
            attackInput = true;
        blockInput = Input.GetKey(KeyCode.Q);
    }

    private void ResetInputs()
    {
        jumpInput = false;
        dashInput = false;
        dodgeInput = false;
        attackInput = false;
        blockInput = false;
    }

    private void CheckGround()
    {
        bool wasGrounded = IsGrounded;
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        _animController.SetGrounded(IsGrounded);
    }

    private void CheckCayoteTimer()
    {
        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (IsStunned)
        {
            HandleBlock();
            ResetInputs();
            return;
        }
        HandleMovement();
        HandleJump();
        ApplyFallGravity(); // Adjust gravity when falling
        HandleDash();
        HandleDodgeRoll();
        HandleBlock();
        HandleAttack();
        ResetInputs();
    }

    private void HandleBlock()
    {
        IsBlocking = blockInput && !IsAttacking && IsGrounded && !IsDodging && !IsDashing && !IsStunned;
        // Debug.Log($"Blocking : {IsBlocking} --- {blockInput} -> {!IsAttacking} -> {IsGrounded} -> {!IsDodging} -> {!IsStunned} -> {!IsDashing}");
        _animController.SetBlock(IsBlocking);
    }

    private void HandleMovement()
    {
        if (IsDodging || IsDashing) return; // Disable normal movement during dodge roll or dash

        if (IsAttacking)
        {
            SetIdle();
            return;
        }
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        int animationState = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        if (moveInput == 0)
        {
            SetIdle();
            return;
        }
        
        if (isTargetLockEnabled)
        {
            // Forward lock: move forward/backward but always face right
            HandleMoveWithLock(speed, animationState);
        }
        else
        {
            var isMovingBackward = moveInput < 0;
            // Without lock: rotate and face direction
            HandleMoveWithTurn(speed, animationState, isMovingBackward);
        }
    }

    private void HandleMoveWithLock(float speed, int animationState)
    {
        float direction = moveInput < 0 ? -1f : 1f;
        _animController.SetMovement(moveInput < 0 ? -1f : animationState); // Backward or Running/Walking forward
        _rb.linearVelocity = new Vector3(direction * speed, _rb.linearVelocity.y, 0);
    }

    private void HandleMoveWithTurn(float speed, int animationState, bool isMovingBackward)
    {
        if (isMovingBackward && isFacingRight)
        {
            RotateToDirection(false);
        }
        else if (!isMovingBackward && !isFacingRight)
        {
            RotateToDirection(true);
        }

        _animController.SetMovement(animationState);
        _rb.linearVelocity = new Vector3(moveInput * speed, _rb.linearVelocity.y, 0);
    }
    
    private void SetIdle()
    {
        IsRunning = false;
        _animController.SetMovement(0); // Set idle animation
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Stop movement
    }


    private void HandleJump()
    {
        if (jumpInput)
        {
            if (IsGrounded && coyoteTimer > 0)
            {
                Jump(jumpForce, false);
            }
            else if (CanDoubleJump)
            {
                Jump(doubleJumpForce, true);
                CanDoubleJump = false;
            }
        }

        if (IsGrounded)
        {
            CanDoubleJump = true; // Reset double jump when grounded
        }
    }
    
    private void Jump(float force, bool isDoubleJump)
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, force, 0);
        if(isDoubleJump)
            _animController.TriggerDoubleJump();
        else
            _animController.TriggerJump();
    }
    
    private void ApplyFallGravity()
    {
        // When falling, apply extra gravity for a heavier feel
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
        }
        // When performing a low jump (releasing jump button early), apply extra gravity
        else if (_rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpGravityMultiplier - 1) * Time.deltaTime;
        }

        // Clamp the player's fall speed to prevent excessively fast falls
        if (_rb.linearVelocity.y < maxFallSpeed)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, maxFallSpeed, 0);
        }
    }

    private void HandleDash()
    {
        if (!AllowDashOrDodge() || !_playerStats.IsDashPossible()) return;
        // Handle dash input (forward/backward)
        if (dashInput && Time.time > lastDash + dashCooldown)
        {
            if (isTargetLockEnabled)
            {
                if (moveInput == 0)
                {
                    moveInput = -1; // Default to forward dodge if no input
                }

                if (moveInput >= 0) // Forward dash
                    StartCoroutine(Dash(Vector3.right, true));
                else // Backward dash
                    StartCoroutine(Dash(Vector3.left, false));
            }
            else
            {
                if (moveInput == 0)
                {
                    if (isFacingRight)
                        StartCoroutine(Dash(Vector3.left, false));
                    else
                        StartCoroutine(Dash(Vector3.right, false));
                }
                else
                {
                    if (moveInput >= 0) // Forward dash
                        StartCoroutine(Dash(Vector3.right, true));
                    else // Backward dash
                        StartCoroutine(Dash(Vector3.left, true));
                }
            }
        }
    }
    
    private void HandleDodgeRoll()
    {
        if (!AllowDashOrDodge() || !_playerStats.IsDodgePossible()) return;
        // Dodge roll (forward/backward)
        if (dodgeInput && Time.time > lastDodge + dodgeRollCooldown)
        {
            if (isTargetLockEnabled)
            {
                if (moveInput == 0)
                {
                    moveInput = -1; // Default to forward dodge if no input
                }

                if (moveInput >= 0) // Forward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.right, true));
                else // Backward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.left, false));
            }
            else
            {
                if (moveInput == 0)
                {
                    if (isFacingRight)
                        StartCoroutine(DodgeRoll(Vector3.left, false));
                    else
                        StartCoroutine(DodgeRoll(Vector3.right, false));
                }
                else
                {
                    if (moveInput >= 0) // Forward dodge roll
                        StartCoroutine(DodgeRoll(Vector3.right, true));
                    else // Backward dodge roll
                        StartCoroutine(DodgeRoll(Vector3.left, true));
                }
            }
        }
    }
    
    
    private IEnumerator Dash(Vector3 direction, bool isForward)
    {
        _playerStats.DashModifiers();
        IsDashing = true;
        _animController.TriggerDash(isForward);
        _rb.linearVelocity = direction * dashSpeed; // Apply dash velocity
        yield return new WaitForSeconds(dashTime); // Wait for dash duration
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Reset velocity to normal after dash
        lastDash = Time.time; // Record dash time
        IsDashing = false;
    }

    private IEnumerator DodgeRoll(Vector3 direction, bool isForward)
    {
        _playerStats.DodgeModifiers();
        IsDodging = true;
        _animController.TriggerDodgeRoll(isForward); // Trigger dodge roll animation (forward/backward)
        _rb.linearVelocity = direction * dodgeRollSpeed; // Set dodge roll velocity
        yield return new WaitForSeconds(dodgeRollTime); // Wait for dodge roll duration
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);  // Reset velocity to zero after dodge roll
        lastDodge = Time.time; // Record dodge time
        IsDodging = false;
    }

    #region Attack

    private Coroutine attackResetter;

    private void HandleAttack()
    {
        if (attackInput && !IsAttacking)
        {
            IsAttacking = true;
            lastAttackTime = Time.time; // Track last attack time
            
            // Determine if we should start or continue the combo
            if (comboStep == 0 || Time.time - comboResetTimer >= comboResetTime)
            {
                comboStep = 1; // Start the combo chain
                Debug.Log($"Attack new -> combo reset: {comboStep} -> {Time.time - comboResetTimer}");
            }
            else if (comboStep < maxComboCount)
            {
                comboStep++; // Continue combo
                Debug.Log($"Attack new -> combo continue: {comboStep}  -> {Time.time - comboResetTimer} >= {comboResetTime}");
            }
            else if(comboStep >= maxComboCount)
            {
                comboStep = 1;
                Debug.Log($"Attack new -> combo reset 2: {comboStep} -> {Time.time - comboResetTimer}");
            }
            // Trigger the appropriate attack animation based on the combo step
            _animController.TriggerAttack(comboStep);

            // Set a timer to reset the combo if no further attacks
            comboResetTimer = Time.time;
            // Delay before allowing another attack
            if(attackResetter != null)
                StopCoroutine(attackResetter);
            attackResetter = StartCoroutine(EndAttackAfterDelay(attackCooldown));
        }
    }

    private IEnumerator EndAttackAfterDelay(float delay)
    {
        // Debug.Log($"Attack almost done");
        yield return new WaitForSeconds(delay);
        
        yield return null;
        IsAttacking = false;
        _animController.ResetAttack();
        attackResetter = null;
        // Debug.Log($"Attack almost done; can reset {Time.time - comboResetTimer >= comboResetTime}? {Time.time - comboResetTimer} > {comboResetTime} seconds");
        // If the combo timeout passes, reset the combo step
        if (Time.time - comboResetTimer >= comboResetTime)
        {
            comboStep = 0; // Reset the combo
            Debug.Log($"Attack Done -> combo reset: {comboStep}");
        }
    }

    #endregion
    
    #region GetHit Logic

    [Button]
    public void TestHit()
    {
        Hit(10);
    }
    [Button]
    public void TestHitStun()
    {
        Hit(10, true);
    }
    
    
    // todo add push back on hit if too much damage
    public void Hit(int damage, bool forceStun = false, Transform hitter = null)
    {
        if (IsInvulnerable) return;
        lastHitTime = Time.time;
        Debug.Log("Player got hit! Damage: " + damage);
        _playerStats.TakeDamage(damage);
        if (_playerStats.IsDead)
        {
            _animController.TriggerDeath();
            return;
        }
        if (UnityEngine.Random.value < stunChance || forceStun)
        {
            StunPlayer();
        }
        _animController.TriggerHit();
        if(!IsStunned) HandleInvulnerability();
    }

    private void StunPlayer()
    {
        Debug.Log("Player is stunned!");
        IsStunned = true;
        _animController.SetStun(true);
        StartCoroutine(StunPlayerRoutine());
    }

    private IEnumerator StunPlayerRoutine()
    {
        yield return new WaitForSeconds(stunDuration);
        IsStunned = false;
        Debug.Log("Player recovered from stun.");
        HandleInvulnerability(true);
        _animController.SetStun(false);
    }

    private void HandleInvulnerability(bool useStunIFramesTime = false)
    {
        IsInvulnerable = true;
        Debug.Log("Player iFrames started!");
        StartCoroutine(StartInvulnerability(useStunIFramesTime));
    }

    private IEnumerator StartInvulnerability(bool useStunIFramesTime = false)
    {
        yield return new WaitForSeconds(useStunIFramesTime ? stunnedInvulnerabilityTime : invulnerabilityTime);
        IsInvulnerable = false;
        Debug.Log("Player iFrames ended");
    }
    #endregion
    
    

    
    private void RotateToDirection(bool faceRight)
    {
        isFacingRight = faceRight;
        transform.rotation = Quaternion.Euler(0, faceRight ? 90 : -90, 0);
    }
    
    
    private bool AllowDashOrDodge()
    {
        return IsGrounded && !IsDodging && !IsDashing && !IsAttacking;
    }
    
    private Vector3 GetDirection()
    {
        return moveInput == 0 ? (isFacingRight ? Vector3.right : Vector3.left) : (moveInput > 0 ? Vector3.right : Vector3.left);
    }

    private bool IsMovingForward()
    {
        return moveInput >= 0 || (moveInput == 0 && isFacingRight);
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}