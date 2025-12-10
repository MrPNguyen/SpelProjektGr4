using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFlying = Animator.StringToHash("isFlying");
    private static readonly int IsHarddropping = Animator.StringToHash("isHarddropping");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");
    private static readonly int HasFallen = Animator.StringToHash("hasFallen");

    
    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
    public ParticleSystem ps;
    private CapsuleCollider2D cc;
    [HideInInspector] public bool hasPlayed;
    
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool isKnockedBack = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    public float horizontalMovement;

    [Header("Running")]
    [SerializeField] private float runSpeed = 10f;

    public bool isRunning { get; set; }
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] public float HardDropPower = 4;
    [HideInInspector]public bool isHardDropping;
    [HideInInspector]public bool hasHardDropped;
    [HideInInspector]public bool isJumping;
    
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform CeilingCheck;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 ceilingCheckSize;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private Vector2 originalWallCheckSize;
    [SerializeField] private LayerMask whatIsGround;
    public bool IsGrounded;
    
    
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [HideInInspector] public bool canMove;

    [Header("Dash")]
    [SerializeField] private float DashPower = 20f;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isHoldingDash;
    private bool canDash = true;
    private bool Dashed;
    private float DashDuration = 0.10f;
    private TrailRenderer tr;
    
    [Header("Flying")]
    [SerializeField] private float flyingPower = 15f;
    private float flyingDuration;
    private bool isFlying;
    private bool GroundedBeforeFlying;

    [Header("Stamina")] 
    [SerializeField] public Image StaminaBar;
    [SerializeField] public float CurrentStamina, MaxStamina;
    [SerializeField] private float RunCost;
    [SerializeField] private float DashCost;
    [SerializeField] private float FlyingCost;
    [SerializeField] private float ChargeRate;
    
    private Coroutine recharge;
    
    [NonSerialized] public Vector2 velocity;
    [NonSerialized] public float multiplier;
    private Vector3 SafePosition = Vector3.zero;
    private Vector3 SafeHardDropPosition = Vector3.zero;
    private Vector3 SafeCeilingPosition = Vector3.zero;
    private Vector3 SafeWallPosition = Vector3.zero;

    [SerializeField] private Vector3 originalOffset;
    [SerializeField] Vector3 dashOffset;
    [SerializeField] private Vector3 headOriginalOffset;
    [SerializeField] Vector3 headDashOffset;
    
    [SerializeField] private float CoyoteTime;
    bool CoroutineStart;
   

    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         animator = GetComponent<Animator>();
         tr = GetComponent<TrailRenderer>();
         cc = GetComponent<CapsuleCollider2D>();
         CurrentStamina = MaxStamina;
        originalWallCheckSize = wallCheckSize;
        
    }

    void Update()
    {
        UpdateAnimations();
        Vector3 temporaryOffset = isDashing ? dashOffset : originalOffset;
        Vector3 headTemporaryOffset = isDashing ? headDashOffset : headOriginalOffset;
        
        if (isFacingRight)
        {
            temporaryOffset.x = temporaryOffset.x;
            headTemporaryOffset.x = headTemporaryOffset.x;
        }
        else
        {
            temporaryOffset.x = -temporaryOffset.x;
            headTemporaryOffset.x = -headTemporaryOffset.x;
        }

        if (isDashing)
        {
            hasPlayed = false;
            
            WallCheck.position = transform.position + temporaryOffset;
            CeilingCheck.position = transform.position + headTemporaryOffset;
          
            Dashed = true;
            
        }
        else if(Dashed)
        {
            WallCheck.position = transform.position + temporaryOffset;
            CeilingCheck.position = transform.position + headTemporaryOffset;
            
            wallCheckSize.y = originalWallCheckSize.y;
           
            Dashed = false;
        }
       
        if (!canMove)
        {
            horizontalMovement = 0;
            isRunning = false;
            isFlying = false;
            isHardDropping = false;
            return;
        }
        
        if (velocity.y <= 0)
        {
            isJumping = false;
        }

        if (!isDashing && canMove)
        {
            if (horizontalMovement < 0)
            {
                isFacingRight = false;
            }
            else if (horizontalMovement > 0)
            {
                isFacingRight = true;
            }
        }

        ApplyFlip();

        GradualStaminaUse(RunCost, isRunning);
        
        GradualStaminaUse(FlyingCost, isFlying);
        
        GradualStaminaUse(DashCost, isDashing);
        
        if (IsGrounded)
        {
            multiplier = 4;
        }
        else
        {
            if (isHardDropping)
            {
                multiplier = HardDropPower;
            }
            else
            {
                multiplier = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            velocity = Vector2.zero;
            return;
        }
        
        rb.linearVelocity = velocity;
        
        if (!isKnockedBack && CurrentStamina != 0 && canMove)
        {
            /*if (isRunning)
            {
                velocity = new Vector2(horizontalMovement * runSpeed, velocity.y);
            }*/
            velocity = new Vector2(horizontalMovement * moveSpeed, velocity.y);
        }
        
       
        if (isFlying)
        {
            flyingDuration -= Time.fixedDeltaTime;

            if (flyingDuration <= 0)
            {
                isFlying = false;
            }
            else
            {
                velocity.y = flyingPower;
            }
        }
        
        if (isDashing)
        {
            if (CurrentStamina <= 0 || !isHoldingDash)
            {
                isDashing = false;
                isHoldingDash = false;
                velocity.x = 0;
                if (tr != null)
                { 
                    tr.emitting = false;
                }
               
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            
            DashDuration -= Time.deltaTime;

            if (DashDuration <= 0 && !isHoldingDash)
            {
                isDashing = false;
                if (tr != null)
                {
                    tr.emitting = false;
                }
                velocity.x = 0;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                return;
            }
            if (isFacingRight)
            {
                velocity.x = DashPower;
            }
            else
            {
                velocity.x = -DashPower;
            }
        }

        if (!CoroutineStart)
        {
            StartCoroutine(isGrounded() );
        }
        
        ApplyGravity();
        rb.linearVelocity = velocity;

        
        IsWalled();
        
      
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    
    /*public void Run(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        if (context.performed)
        {
            isRunning = true;
        }

        if (context.canceled)
        {
            isRunning = false;
        }
    }*/


    public void Jump(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
        if (isFlying) return;

        if (CurrentStamina == 0) return;

        // Prevent jumping in the air
        if (!IsGrounded && context.performed)
        {
            return;
        }

        // Normal jump
        if (context.performed)
        {
            multiplier = 1;
            velocity = new Vector2(velocity.x, jumpForce);
            isJumping = true;
            hasPlayed = false;
            CreateDust();
        }
        
        if (context.canceled)
        {
            if (velocity.y > 0)
            {
                isJumping = false;
            }
        }
    }

    public void Fly(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
        if (CurrentStamina == 0) return;
        
        if (context.started)
        {
            GroundedBeforeFlying = IsGrounded;
        }

        if (context.performed)
        {
            if(!GroundedBeforeFlying) return;

            isFlying = true;
            flyingDuration = 0.8f;
        }

        if (context.canceled)
        {
            isFlying = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        if (CurrentStamina == 0)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            isDashing = false;
            return;
        }
            
        if (context.performed && canDash)
        {
            DashDuration = 0.1f;
            float dashDirection = isFacingRight ? 1 : -1;

            if (dashDirection == -1)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 60);
            }
            else if (dashDirection == 1)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -60);
            }
            isDashing = true;
            isHoldingDash = true;
            if (tr != null)
            {
                tr.emitting = true;
            }
            
        }

        if (context.canceled)
        {
            isHoldingDash = false;
            if (tr != null)
            {
                tr.emitting = false;
            }

            DashDuration = 0.1f;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    public void HardDrop(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        if (CurrentStamina == 0) return;

        if (context.performed)
        {
            hasHardDropped = true;
            isHardDropping = true;
            hasPlayed = false;
        }

        if (context.canceled)
        {
            isHardDropping = false;
        }
    }
    private IEnumerator RechargeStamina()
    {
        if (!canMove) yield break;
        if (CurrentStamina == 0)
        {
            yield return new WaitForSeconds(4f);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        while (CurrentStamina < MaxStamina)
        {
            CurrentStamina += ChargeRate /10f;
            if (CurrentStamina >= MaxStamina)
            {
                CurrentStamina = MaxStamina;
            }
            StaminaBar.fillAmount = CurrentStamina / MaxStamina;
            yield return new WaitForSeconds(.1f);

        }
    }

    private void GradualStaminaUse(float cost, bool isAction)
    {
        if (isAction  && CurrentStamina > 0)
        {
            CurrentStamina -= cost * Time.deltaTime;
            if (CurrentStamina < 0)
            {
                CurrentStamina = 0;
            }
            StaminaBar.fillAmount = CurrentStamina / MaxStamina;

            StartRecharge();
        }
    }
    public IEnumerator isGrounded()
    {
        CoroutineStart = true;
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
        {
            IsGrounded = true;
            if (velocity.x != 0 && !isJumping)
            {
                yield return new WaitForSeconds(CoyoteTime);
            }
        }
        else
        {
             IsGrounded = false;
        }

        CoroutineStart = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        if (WallCheck != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube( WallCheck.position, wallCheckSize);
        }

        if (CeilingCheck != null)
        { 
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube( CeilingCheck.position, ceilingCheckSize);
        }
        
    }

    void CreateDust()
    {
        ps.Play();
    }
    

    private void StartRecharge()
    {
        if (recharge != null)
        {
            StopCoroutine(recharge);
        }
        recharge = StartCoroutine(RechargeStamina());
    }
    
    private void ApplyFlip()
    {
        transform.localScale = new Vector3(
            isFacingRight ? 0.8f : -0.8f, 
            0.8f, 
            1.6f
        );
    }

    private void ApplyGravity()
    {   
        Vector3 pos = transform.position;
        if (!IsGrounded)
        {
            SafeHardDropPosition = transform.position;
        }
        if (IsGrounded && hasHardDropped)
        {
            hasPlayed = false;
            velocity.y = 0;
            pos.y = SafeHardDropPosition.y;
            transform.position = pos;
            isHardDropping = false;
            StartCoroutine(ResetHardDropFlag());
        }
        else if (IsGrounded && !isJumping && !isKnockedBack)
        {
            if (velocity.y < -5)
            {
                pos.y = SafeHardDropPosition.y;
                transform.position = pos;
            }

            velocity.y = 0;
        }
        else
        {
            velocity.y += Physics2D.gravity.y * multiplier * Time.deltaTime;
        }
    }

    private IEnumerator ResetHardDropFlag()
    {
        yield return null;
        hasHardDropped = false;
    }

    private void IsWalled()
    {
        Vector3 CeilingPosition = transform.position;
        Vector3 Wallpos = transform.position;
        if (Physics2D.OverlapBox(WallCheck.position, wallCheckSize, 0, whatIsGround))
        {
            velocity.x = 0;
                
            Wallpos.x = SafeWallPosition.x;
            transform.position = Wallpos;
        }
        else
        {
            SafeWallPosition = transform.position;
        }
        
       
        if (Physics2D.OverlapBox(CeilingCheck.position, ceilingCheckSize, 0, whatIsGround))
        {
            
            CeilingPosition.y = SafeCeilingPosition.y;
            transform.position = CeilingPosition;
        }
        else
        {
            SafeCeilingPosition = transform.position;
        }
        
        Vector3 position = transform.position;
        if (cc.IsTouchingLayers(whatIsGround))
        {
           
            position.x = SafePosition.x;
            transform.position = position;
        }
        else
        {
            SafePosition = transform.position;
        }
        
    }

    private void UpdateAnimations()
    {
        bool grounded = IsGrounded;
        

        if (isFlying)
        {
            animator.SetBool(IsFlying, true);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsHarddropping, false);
            animator.SetBool(IsDashing, false);
            return;
        }
        animator.SetBool(IsFlying, false);
        
        if (isHardDropping)
        {
            animator.SetBool(IsFlying, false);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsHarddropping, true);
            animator.SetBool(IsDashing, false);
            return;
        }
        animator.SetBool(IsHarddropping, false);
        
        if (isDashing)
        {
            animator.SetBool(IsDashing, true);
            animator.SetBool(IsFlying, false);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsHarddropping, false);
            return;
        }
        animator.SetBool(IsDashing, false);
        
        if (!grounded && velocity.y > 0)
        {
            animator.SetBool(IsFlying, false);
            animator.SetBool(IsJumping, true);
            animator.SetBool(IsHarddropping, false);
            animator.SetBool(IsDashing, false);
            return;
        }
        animator.SetBool(IsJumping, false);

        if (!grounded && velocity.y < 0)
        {
            animator.SetBool(HasFallen, true);
        }
        animator.SetBool(IsWalking, grounded && horizontalMovement != 0 && !isDashing);
    }
}