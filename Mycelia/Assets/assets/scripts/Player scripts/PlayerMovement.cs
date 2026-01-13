using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFlying = Animator.StringToHash("isFlying");
    private static readonly int IsHarddropping = Animator.StringToHash("isHarddropping");
    private static readonly int IsDashing = Animator.StringToHash("isDashing");
    private static readonly int HasFallen = Animator.StringToHash("hasFallen");

    
    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
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
    public bool Jumped;
    private bool canJump;
    public bool isWalking;
    
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform CeilingCheck;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Vector2 ceilingCheckSize;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private Vector2 originalWallCheckSize;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask Svamp;
    public bool IsGrounded;
    
    [HideInInspector] public Animator animator;

    [HideInInspector] public bool canMove;

    [Header("Dash")]
    [SerializeField] private float DashPower = 20f;
    [HideInInspector] public bool isDashing;
    private bool canDash = true;
    private TrailRenderer tr;
    
    [Header("Flying")]
    [SerializeField] private float flyingPower = 15f;
    private float flyingDuration;
    private bool isFlying;
    private bool GroundedBeforeFlying;

    [Header("Stamina")] 
    [SerializeField] public Image StaminaBar;
    [SerializeField] public Volume staminaVolume;
    [SerializeField] public float CurrentStamina, MaxStamina;
    [SerializeField] private float RunCost;
    [SerializeField] private float DashCost;
    [SerializeField] private float FlyingCost;
    [SerializeField] private float ChargeRate;
    
    private Coroutine recharge;
    
    [NonSerialized] public Vector2 velocity;
    [NonSerialized] public float multiplier;
	[SerializeField] private float fallSpeed = 1.5f;
    
    private Vector3 SafeHardDropPosition = Vector3.zero;
    private Vector3 SafeCeilingPosition = Vector3.zero;
    private Vector3 SafeWallPosition = Vector3.zero;
   

    [SerializeField] private Vector3 originalOffset;
    [SerializeField] Vector3 dashOffset;
    [SerializeField] private Vector3 headOriginalOffset;
    [SerializeField] Vector3 headDashOffset;
    
    [SerializeField] private float CoyoteTime;
    bool CoroutineStart;
    
    [Header("Particle System")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem harddropParticles;

   

    void Start()
    {
        staminaVolume.weight = 0;
         rb = GetComponent<Rigidbody2D>();
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
          
            
        }
        else
        {
            WallCheck.position = transform.position + temporaryOffset;
            CeilingCheck.position = transform.position + headTemporaryOffset;
            
            wallCheckSize.y = originalWallCheckSize.y;
           
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

        if (!isDashing)
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

        //GradualStaminaUse(RunCost, isRunning);
        
        GradualStaminaUse(FlyingCost, isFlying);
        
        GradualStaminaUse(DashCost, isDashing);
        
        if (IsGrounded)
        {
            multiplier = 1;
        }
        else
        {
            if (isHardDropping)
            {
                multiplier = HardDropPower;
                isHardDropping = false;
            }
            else if(!hasHardDropped)
            {
                multiplier = fallSpeed;
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
        
        if (!isKnockedBack && canMove)
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
            if (CurrentStamina <= 0)
            {
                StopDash();
            }
            else
            {
                velocity.x = isFacingRight? DashPower : -DashPower;
            }
        }
        
         isGrounded();
         
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

        //if (CurrentStamina == 0) return;

        // Prevent jumping in the air
        if (!canJump && context.performed)
        {
            return;
        }

        if (context.performed)
        {
            multiplier = 1;
            velocity = new Vector2(velocity.x, jumpForce);
            isJumping = true;
            Jumped = true;
            hasPlayed = false;
            jumpDust();
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
            GroundedBeforeFlying = canJump;
        }

        if (context.performed)
        {
            if(!GroundedBeforeFlying) return;

            isFlying = true;
            isJumping = false;
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
            isDashing = false;
            return;
        }
            
        if (context.performed && canDash)
        {
            isDashing = true;
            if (tr != null)
            {
                tr.emitting = true;
            }
            
        }

        if (context.canceled)
        {
            StopDash();
        }
    }
    private void StopDash()
    {
        if (!isDashing) return;

        isDashing = false;
        velocity.x = 0;

        if (tr != null) tr.emitting = false;
    }
    public void HardDrop(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        //if (CurrentStamina == 0) return;

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
            float one = 1;
            staminaVolume.weight = one -(CurrentStamina / MaxStamina);
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
            float one = 1;
            staminaVolume.weight = one -(CurrentStamina / MaxStamina);
            StartRecharge();
        }
    }
    public void isGrounded()
    {
        
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
        {
            IsGrounded = true;
        }
        else IsGrounded = false;
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround)||
            Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, Svamp))
        {
            canJump = true;
        }
        else
        {
            if (!isJumping && !CoroutineStart)
            {
                StartCoroutine(SetJumpBool());
            }
            if (isJumping){canJump = false;}
        }
    }

    private IEnumerator SetJumpBool()
    {
        CoroutineStart = true;
        yield return new WaitForSeconds(CoyoteTime);
        CoroutineStart = false;
        canJump = false;
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

    void jumpDust()
    {
        jumpParticles.Play();
    }

    void harddropDust()
    {
        harddropParticles.Play();
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
        // Bara vägg
        if (cc.IsTouchingLayers(whatIsGround) ||Physics2D.OverlapBox(WallCheck.position, wallCheckSize, 0, whatIsGround))
        {
            velocity.x = 0;
                
            Vector3 Wallpos = transform.position;
            Wallpos.x = SafeWallPosition.x;
            transform.position = Wallpos;
        }
        else
        {
            SafeWallPosition = transform.position;
        }
        
        if (Physics2D.OverlapBox(CeilingCheck.position, ceilingCheckSize, 0, whatIsGround))
        {
            Vector3 CeilingPosition = transform.position;
            CeilingPosition.y = SafeCeilingPosition.y;
            transform.position = CeilingPosition;
            
        }
        else
        {
            SafeCeilingPosition = transform.position;
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
        else animator.SetBool(IsFlying, false);
       
        
        if (hasHardDropped)
        {
            animator.SetBool(IsFlying, false);
            animator.SetBool(IsJumping, false);
            animator.SetBool(IsHarddropping, true);
            animator.SetBool(IsDashing, false);
            return;
        } else animator.SetBool(IsHarddropping, false);
        
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

        if (!grounded && velocity.y < 0 && !hasHardDropped)
        {
            animator.SetBool(HasFallen, true);
        }
        else   animator.SetBool(HasFallen, false);
        animator.SetBool(IsWalking, grounded && horizontalMovement != 0 && !isDashing);
        isWalking = animator.GetBool("isWalking");
    }
}