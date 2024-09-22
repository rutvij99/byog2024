using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-99)]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    
    // Forward lock variables
    [Header("Target Locking")]
    [FormerlySerializedAs("tg")] public CinemachineTargetGroup cinemachineTargetGroup;
    public PlayerTarget currentTarget;
    public bool isTargetLockEnabled = false; // Toggle for forward locking mode
    [SerializeField] private float maxTargetDistance = 25f; // Maximum distance to search for targets

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
    [SerializeField] private float comboAllowedTime = 0.3f;  // Time between attacks
    
    [Header("Block")]
    [SerializeField] private float blockChance = 0.7f;
    [SerializeField] private float damageNegation = 0.7f;
    
    [Header("Hit/Recoil")]
    [SerializeField] private float stunDuration = 1.5f;     // Duration of stun when player is stunned
    [SerializeField] private float stunChance = 0.3f;       // Probability of getting stunned (30% chance)
    [SerializeField] private float pushBackForce = 5f; // Strength of the push-back force
    [SerializeField] private float pushBackDuration = 0.2f;  // Duration of the push-back effect
    
    
    [Header("IFrames")]
    [SerializeField] private float hitIFrames = 1.0f; // Time during which player can't get hit after taking damage
    [SerializeField] private float stunnedIFramesTime = 1.0f; // Time during which player can't get hit after taking damage
    [SerializeField] private float dashIFramesTime = 0.2f;
    [SerializeField] private float dodgeIFramesTime = 0.3f;
    
    private Rigidbody _rb;
    private PlayerStats _playerStats;
    private PlayerAnimationController _animController;
    private PlayerAttackModule _attackModule;
    private Collider _collider;
    
    // Internal variables
    private float coyoteTimer = 0f;
    
    // dash/dodge roll variables
    private float lastDash = -1f;
    private float lastDodge = -1f;

    // Attack variables
    private bool isComboActive = false;  // True when the player is in an active combo
    private int lightComboStep = 0;             // Current attack step in combo
    private int heavyComboStep = 0;             // Current attack step in combo
    private float lastAttackTime = 0f;     // Tracks the last time an attack occurred
    private float comboResetTimer = 0f;    // Timer for resetting the combo if no further attacks

    // Hit variables
    private float lastHitTime = 0f;       // Time when the player was last hit

    
    // Variables for storing input
    private float moveInput;
    private bool jumpInput;
    private bool dashInput;
    private bool dodgeInput;
    private bool lightAttackInput;
    private bool heavyAttackInput;
    private bool blockInput;
    private bool targetLockInput;

    

    public bool IsFacingRight { get; private set; } = true;
    public bool IsGrounded { get; private set; }
    public bool CanDoubleJump { get; private set; }
    public bool IsDodging { get; private set; } = false;
    public bool IsDashing { get; private set; } = false;
    public bool IsAttacking { get; private set; } = false;
    public bool IsStunned { get; private set; } = false;
    public bool IsInvulnerable { get; private set; } = false;
    public bool IsBlocking { get; private set; } = false;
    
    public bool IsSwitchingWeapons { get; private set; } = false;

    
    private bool CanChangeState()
    {
        return !IsDodging && !IsDashing && !IsAttacking && !IsStunned && !IsSwitchingWeapons;
    }
    
    private void Awake()
    {
        _attackModule = GetComponent<PlayerAttackModule>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _playerStats = GetComponent<PlayerStats>();
        _animController = GetComponent<PlayerAnimationController>(); // Reference the animation controller
    }

    private void Start()
    {
        SwitchWeapon(WeaponType.SwordShield);
    }

    private void Update()
    {
        if (_playerStats.IsDead)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Stop movement
            return;
        }
        CheckGround();
        CheckCayoteTimer();
        CheckInput();
        CheckLockTarget();
        if (CanChangeState() && moveInput == 0)
        {
            // temp hack which doesn't seme to do much 
            SetIdle();
        }
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
            lightAttackInput = true;
        if (Input.GetKeyDown(KeyCode.Mouse1))  // Left mouse button for attack
            heavyAttackInput = true;
        blockInput = Input.GetKey(KeyCode.E);
        if (Input.GetKeyDown(KeyCode.Q))
            targetLockInput = true;
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(WeaponType.SwordShield);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(WeaponType.Wand);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(WeaponType.GreatSword);
    }

    public void SwitchWeapon(WeaponType weaponType)
    {
        if(!CanChangeState()) return;
        if(_playerStats.currentWeaponType == weaponType) return;
        _playerStats.SwitchWeapon(weaponType);
        walkSpeed = _playerStats.currentWeaponData.maxWalkSpeed;
        runSpeed = _playerStats.currentWeaponData.maxRunSpeed;
        _animController.SwitchWeapon(_playerStats.currentWeaponData);
        StartCoroutine(SwitchWeaponRoutine());
    }

    private IEnumerator SwitchWeaponRoutine()
    {
        IsSwitchingWeapons = true;
        yield return new WaitForSeconds(1f);
        IsSwitchingWeapons = false;
    }

    public void CheckLockTarget()
    {
        if(!targetLockInput) return;
        targetLockInput = false;
        isTargetLockEnabled = !isTargetLockEnabled;
        if (!isTargetLockEnabled)
        {
            if (currentTarget != null)
            {
                cinemachineTargetGroup.RemoveMember(currentTarget.transform);
                currentTarget.SetLock(false);
            }
            return;
        }
        // Find the closest target
        PlayerTarget closestTarget = FindClosestTarget();
        // find target
        if (closestTarget == null)
        {
            isTargetLockEnabled = false;
            return;
        }
        currentTarget = closestTarget;
        cinemachineTargetGroup.AddMember(currentTarget.transform, 1, 0.5f);
        currentTarget.SetLock(true);
    }
    
    private PlayerTarget FindClosestTarget()
    {
        PlayerTarget closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        // Assuming you have a way to get a list of all potential targets, like:
        PlayerTarget[] allTargets = FindObjectsOfType<PlayerTarget>(); 

        foreach (var target in allTargets)
        {
            // Calculate the squared distance (to avoid unnecessary square roots)
            float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;

            // Check if the target is within the max distance and closer than the previous closest target
            Debug.Log($"{target} - {distanceSqr} -> {maxTargetDistance * maxTargetDistance}");
            if (distanceSqr < closestDistanceSqr && distanceSqr <= maxTargetDistance * maxTargetDistance)
            {
                closestTarget = target;
                closestDistanceSqr = distanceSqr;
            }
        }

        return closestTarget;
    }

    private void ResetInputs()
    {
        jumpInput = false;
        dashInput = false;
        dodgeInput = false;
        lightAttackInput = false;
        heavyAttackInput = false;
        blockInput = false;
        targetLockInput = false;
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
    
    private bool IsBlockedInDirection(float moveInput)
    {
        Vector3 direction;
        if (isTargetLockEnabled)
        {
            direction = moveInput > 0 ? transform.forward : -transform.forward;
        }
        else
        {
            direction = IsFacingRight ? Vector3.right : Vector3.left;
        }
        Vector3 boxHalfExtents = new Vector3(_collider.bounds.extents.x, _collider.bounds.extents.y / 3, 0.1f);  // Adjust the depth (Z) as needed

        // Perform a Boxcast in the direction of movement
        RaycastHit hit;
        if (Physics.BoxCast(headTransform.position + new Vector3(0,0,0.5f), boxHalfExtents, direction, out hit, transform.rotation, 0.1f, groundLayer))
        {
            Debug.Log("Blocked by: " + hit.collider.name);
            return true;  // There's something blocking the player's movement
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (_playerStats.IsDead)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Stop movement
            return;
        }
        
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
        HandleAttacks();
        ResetInputs();
    }

    private void HandleBlock()
    {
        IsBlocking = blockInput && IsGrounded && !IsAttacking && !IsDodging && !IsDashing && !IsStunned &&
                     _playerStats.currentWeaponType != WeaponType.Wand;
        // Debug.Log($"Blocking : {IsBlocking} --- {blockInput} -> {!IsAttacking} -> {IsGrounded} -> {!IsDodging} -> {!IsStunned} -> {!IsDashing}");
        _animController.SetBlock(IsBlocking);
    }

    #region Movement
    private void HandleMovement()
    {
        if (IsDodging || IsDashing) return; // Disable normal movement during dodge roll or dash
        if (IsAttacking || moveInput == 0)
        {
            SetIdle();
            return;
        }
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        int animationState = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        // if (!IsGrounded && IsBlockedInDirection(moveInput))
        // {
        //     SetIdle();
        //     return;
        // }

        if (isTargetLockEnabled)
        {
            RotateTowardsTarget();
            HandleMoveWithLock(speed, animationState);
        }
        else
        {
            var isMovingBackward = moveInput < 0;
            RotateBaseOnInput(isMovingBackward);
            HandleMoveWithTurn(speed, animationState); // Without lock: rotate and face direction
        }
    }

    private void HandleMoveWithLock(float speed, int animationState)
    {
        float direction = moveInput < 0 ? -1f : 1f;
        if (IsFacingRight)
            _animController.SetMovement(moveInput < 0 ? -1f : animationState); // Backward or Running/Walking forward
        else
            _animController.SetMovement(moveInput > 0 ? -1f : animationState); // Backward or Running/Walking forward
        _rb.linearVelocity = new Vector3(direction * speed, _rb.linearVelocity.y, 0);
    }

    private void HandleMoveWithTurn(float speed, int animationState)
    {
        _animController.SetMovement(animationState);
        _rb.linearVelocity = new Vector3(moveInput * speed, _rb.linearVelocity.y, 0);
    }
    
    private void SetIdle()
    {
        _animController.SetMovement(0); // Set idle animation
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0); // Stop movement
    }
    #endregion
    
    #region Rotation
    private void RotateTowardsTarget()
    {
        if (currentTarget == null) return;
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;

        if (directionToTarget.x > 0)
        {
            RotateToDirection(true); // Face right (90°)
        }
        else if (directionToTarget.x < 0)
        {
            RotateToDirection(false); // Face left (-90°)
        }
    }

    private void RotateBaseOnInput(bool isMovingBackward)
    {
        if (isMovingBackward && IsFacingRight)
        {
            RotateToDirection(false); // Face left (-90°)
        }
        else if (!isMovingBackward && !IsFacingRight)
        {
            RotateToDirection(true); // Face right (90°)
        }
    }
    
    private void RotateToDirection(bool faceRight)
    {
        IsFacingRight = faceRight;
        transform.rotation = Quaternion.Euler(0, faceRight ? 90 : -90, 0);
    }
    #endregion

    #region Jump
    private void HandleJump()
    {
        if (jumpInput)
        {
            if (IsGrounded && coyoteTimer > 0)
            {
                Jump(false);
            }
            else if (CanDoubleJump)
            {
                Jump(true);
                CanDoubleJump = false;
            }
        }

        if (IsGrounded)
        {
            CanDoubleJump = true; // Reset double jump when grounded
        }
    }
    
    private void Jump(bool isDoubleJump)
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, isDoubleJump ? doubleJumpForce : jumpForce, 0);
        _animController.TriggerJump(isDoubleJump);
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
    #endregion

    #region Dash/Dodge
    private void HandleDash()
    {
        if(!CanChangeState() || !IsGrounded) return;
        if (!_playerStats.IsDashPossible()) return;
        if (!dashInput || Time.time <= lastDash + dashCooldown) return;
        
        if (isTargetLockEnabled)
        {
            if (moveInput == 0)
            {
                moveInput = IsFacingRight? -1 : 1; // Default to forward dodge if no input
            }
            if (IsFacingRight)
            {
                if (moveInput >= 0) // Forward dash
                    StartCoroutine(Dash(Vector3.right, true));
                else // Backward dash
                    StartCoroutine(Dash(Vector3.left, false));
            }
            else
            {
                if (moveInput >= 0) // Forward dodge roll
                    StartCoroutine(Dash(Vector3.right, false));
                else // Backward dodge roll
                    StartCoroutine(Dash(Vector3.left, true));
            }
        }
        else
        {
            if (moveInput == 0)
            {
                if (IsFacingRight)
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
    
    private void HandleDodgeRoll()
    {
        if(!CanChangeState() || !IsGrounded) return;
        if (!_playerStats.IsDodgePossible()) return;
        if (!dodgeInput || Time.time <= lastDodge + dodgeRollCooldown) return;
        
        if (isTargetLockEnabled)
        {
            if (moveInput == 0)
            {
                moveInput = IsFacingRight? -1 : 1; // Default to forward dodge if no input
            }
            if (IsFacingRight)
            {
                if (moveInput >= 0) // Forward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.right, true));
                else // Backward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.left, false));
            }
            else
            {
                if (moveInput >= 0) // Forward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.right, false));
                else // Backward dodge roll
                    StartCoroutine(DodgeRoll(Vector3.left, true));
            }
        }
        else
        {
            if (moveInput == 0)
            {
                if (IsFacingRight)
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
    
    
    private IEnumerator Dash(Vector3 direction, bool isForward)
    {
        GrantIFrames(dashIFramesTime);
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
        GrantIFrames(dodgeIFramesTime);
        _playerStats.DodgeModifiers();
        IsDodging = true;
        _animController.TriggerDodgeRoll(isForward); // Trigger dodge roll animation (forward/backward)
        _rb.linearVelocity = direction * dodgeRollSpeed; // Set dodge roll velocity
        yield return new WaitForSeconds(dodgeRollTime); // Wait for dodge roll duration
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);  // Reset velocity to zero after dodge roll
        lastDodge = Time.time; // Record dodge time
        IsDodging = false;
    }
    #endregion

    #region Attack
    private void HandleAttacks()
    {
        if (!CanChangeState()) return;

        if (lightAttackInput)
        {
            if (!IsGrounded)
            {
                HandleAirAttack();
                return;
            }
            HandleMainAttack(ref lightComboStep, _playerStats.currentWeaponData.lightMeleeAttacks);
            return;
        }

        if (heavyAttackInput)
        {
            HandleMainAttack(ref heavyComboStep, _playerStats.currentWeaponData.heavyMeleeAttacks);
            return;
        }
    }

    private void HandleAirAttack()
    {
        if(_playerStats.currentWeaponData.airAttacks == null || _playerStats.currentWeaponData.airAttacks.Count <= 0) return;
        lightComboStep = 1;
        var currentAttakInfo = _playerStats.currentWeaponData.airAttacks[Random.Range(0, _playerStats.currentWeaponData.airAttacks.Count)];           
        StartCoroutine(HandleAttackRoutine(currentAttakInfo));
    }
    private void HandleMainAttack(ref int stepCombo, List<AttackInfo> attackLib)
    {
        if(attackLib == null || attackLib.Count <= 0) return;
        // Check if it's a new combo or continuation of an existing combo
        if (stepCombo == 0 || Time.time - lastAttackTime >= comboAllowedTime || lightComboStep >= attackLib.Count)
        {
            stepCombo = 1;
            Debug.Log($"Combo reset: {stepCombo}");
        }
        else if (stepCombo < attackLib.Count)
        {
            stepCombo++;
            Debug.Log($"Combo continued: {stepCombo}");
        }
        var currentAttakInfo = attackLib[stepCombo - 1];
        // Start a coroutine to manage the combo and cooldown for the current step
        StartCoroutine(HandleAttackRoutine(currentAttakInfo));
    }
    
    private IEnumerator HandleAttackRoutine(AttackInfo currentAttakInfo)
    {
        if(!_playerStats.IsAttackPossible(currentAttakInfo)) yield break;
        _playerStats.AttackModifiers(currentAttakInfo);
        IsAttacking = true;
        _animController.TriggerAttack(currentAttakInfo);
        isComboActive = true;
        _attackModule.SetAttackInfo(currentAttakInfo);
        yield return new WaitForSeconds(currentAttakInfo.attackDuration);
        lastAttackTime = Time.time;
        IsAttacking = false;
    }
    #endregion
    
    #region Get Hit Logic
    public void InstantKill()
    {
        _playerStats.TakeDamage(_playerStats.currentHealth + 10);
        if (_playerStats.IsDead)
        {
            _animController.TriggerDeath();
        }
    }
    public void TakeDamage(int damage, bool forceStun = false, Transform hitter = null)
    {
        if (IsInvulnerable) return;
        lastHitTime = Time.time;
        Debug.Log("Player got hit! Damage: " + damage);
        if (IsBlocking)
        {
            var blocked = Random.value <= blockChance;
            if (blocked)
                damage = (int)(damage * damageNegation);
        }
        _playerStats.TakeDamage(damage);
        if (_playerStats.IsDead)
        {
            _animController.TriggerDeath();
            return;
        }
        // Apply push-back force if hitter exists
        var isStunned = UnityEngine.Random.value < stunChance || forceStun;
        if (hitter != null)
        {
            ApplyPushBack(hitter, damage, isStunned);
        }
        if (!IsStunned)
        {
            if (isStunned)
            {
                StunPlayer();
            }
            GrantIFrames(hitIFrames);
        }
        _animController.TriggerHit();
    }
    
    private void ApplyPushBack(Transform hitter, int damage, bool actualStun)
    {
        IsStunned = true;
        Vector3 directionAwayFromHitter = (transform.position - hitter.position).normalized;
        float scaledPushBackForce = pushBackForce + (damage * 0.1f); // Scale push-back with damage
        _rb.linearVelocity = directionAwayFromHitter * scaledPushBackForce;
        StartCoroutine(StopPushBackAfterDelay(actualStun));
    }

    private IEnumerator StopPushBackAfterDelay( bool actualStun)
    {
        yield return new WaitForSeconds(pushBackDuration);
        if (!actualStun)
        {
            IsStunned = false;
        }
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);  // Only stop horizontal movement, retain vertical velocity
    }

    private void StunPlayer()
    {
        Debug.Log("Player is stunned!");
        IsStunned = true;
        _animController.SetStun(IsStunned);
        StartCoroutine(StunPlayerRoutine());
    }

    private IEnumerator StunPlayerRoutine()
    {
        yield return new WaitForSeconds(stunDuration);
        IsStunned = false;
        Debug.Log("Player recovered from stun.");
        GrantIFrames(stunnedIFramesTime);
        _animController.SetStun(IsStunned);
    }
    #endregion
    
    #region IFrame
    private Coroutine iFrameRoutine;
    private void GrantIFrames(float iframeDuration, bool useStunIFramesTime = false)
    {
        IsInvulnerable = true;
        Debug.Log("Player iFrames started!");
        if (iFrameRoutine != null)
        {
            StopCoroutine(iFrameRoutine);
        }
        iFrameRoutine = StartCoroutine(StartIFrames(iframeDuration));
    }

    private IEnumerator StartIFrames(float iframeDuration)
    {
        yield return new WaitForSeconds(iframeDuration);
        IsInvulnerable = false;
        Debug.Log("Player iFrames ended");
        iFrameRoutine = null;
    }
    #endregion
    
    #region Testing
    private void OnDrawGizmos()
    {
        // Only draw the gizmo if the player controller is active
        // if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    
    [Button]
    public void TestHit()
    {
        TakeDamage(10);
    }
    [Button]
    public void TestHitStun()
    {
        TakeDamage(10, true);
    }
    [Button]
    public void TestHitPushback()
    {
        TakeDamage(10, hitter: currentTarget.transform);
    }

    [Button]
    public void TestHitPushbackStun()
    {
        TakeDamage(10, true, currentTarget.transform);
    }
    #endregion

}