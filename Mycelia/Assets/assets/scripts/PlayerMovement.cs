using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;


//Code Source: Game Code Library: "2D Platformer Unity"
public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int HasJumping = Animator.StringToHash("hasJumping");
    private static readonly int IsDashingLeft = Animator.StringToHash("isDashingLeft");
    private static readonly int IsDashingRight = Animator.StringToHash("isDashingRight");

    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
    private CapsuleCollider2D bc;
    private Vector2 originalColliderOffset;
    
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool isKnockedBack = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Running")]
    [SerializeField] private float runSpeed = 10f;
    public bool isRunning { get; private set; }
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] public float HardDropPower = 4;
    public bool isHardDropping = false;
    public bool hasHardDropped = false;
    public bool isJumping = false;
    
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask whatIsGround;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public bool canMove = true;

    [Header("Dash")]
    [SerializeField] private float DashPower = 20f;
    public bool isDashing;
    private bool canDash = true;
    private float DashDuration = 0.10f;
    private float DashCooldown = 0.1f;
    private TrailRenderer tr;
    
    [Header("Flying")]
    [SerializeField] private float flyingPower = 15f;
    private float flyingDuration;
    private bool isFlying = false;
    private bool GroundedBeforeFlying;

    [Header("Stamina")] 
    [SerializeField] public Image StaminaBar;
    [SerializeField] public float CurrentStamina, MaxStamina;
    [SerializeField] private float JumpCost;
    [SerializeField] private float RunCost;
    [SerializeField] private float DashCost;
    [SerializeField] private float FlyingCost;
    [SerializeField] private float HardDropCost;
    [SerializeField] private float ChargeRate;
    
    private Coroutine recharge;
    
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         animator = GetComponent<Animator>();
         tr = GetComponent<TrailRenderer>();
         bc = GetComponent<CapsuleCollider2D>();
         CurrentStamina = MaxStamina;
         originalColliderOffset = bc.offset;
    }

    void Update()
    {
        Debug.Log(horizontalMovement);
        //animator.SetBool(HasJumping, isJumping); //fungerar detta?

        if (rb.linearVelocity.y <= 0)
        {
            isJumping = false;
        }
        
        animator.SetBool(IsWalking, false);
        
        if (isDashing)
        {
            return;
        }

        if (!isKnockedBack && CurrentStamina != 0)
        {
            if (isRunning)
            {
                rb.linearVelocity = new Vector2(horizontalMovement * runSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            }
        }

        if (!isDashing)
        {
            if (horizontalMovement > 0.01f)
            {
                isFacingRight = true;
                animator.SetBool(IsWalking, true);
            }
            else if (rb.linearVelocity.x < -0.01f)
            {
                isFacingRight = false;
                animator.SetBool(IsWalking, true);
            }
        }
        
        ApplyFlip();

        if (isRunning  && CurrentStamina != 0)
        {
            CurrentStamina -= RunCost * Time.deltaTime;
            if (CurrentStamina < 0)
            {
                CurrentStamina = 0;
            }
            StaminaBar.fillAmount = CurrentStamina / MaxStamina;

            StartRecharge();
        }
        else
        {
            isRunning = false;
        }
        
        if (isFlying && CurrentStamina != 0)
        {
            CurrentStamina -= FlyingCost * Time.deltaTime;
            if (CurrentStamina < 0)
            {
                CurrentStamina = 0;
            }
            StaminaBar.fillAmount = CurrentStamina / MaxStamina;

            StartRecharge();
        }
        else
        {
            isFlying = false;
        }
    }

    void FixedUpdate()
    {
        if (isGrounded())
        {
            rb.gravityScale = 4;
        }
        else
        {
            if (isHardDropping)
            {
                rb.gravityScale = HardDropPower;
            }
            else
            {
                rb.gravityScale = 1;
            }
        }
        if (isFlying)
        {
            flyingDuration -= Time.deltaTime;

            if (flyingDuration <= 0)
            {
                isFlying = false;
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyingPower);
            }
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
        }
    }
    
    public void Run(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed)
            {
                isRunning = true;
            }

            if (context.canceled)
            {
                isRunning = false;
            }
        }
    }


    public void Jump(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
        if (isFlying) return;

        if (CurrentStamina == 0) return;
        

        // Prevent jumping in the air
        if (!isGrounded() && context.performed)
        {
            return;
        }

        // Normal jump
        if (context.performed)
        {
            Debug.Log("Can Jump");
            rb.gravityScale = 1;
            StaminaLoss(JumpCost);
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
          
        }

        // Optional variable jump height
        if (context.canceled)
        {
            Debug.Log("Can't Jump");
            StartRecharge();
        }
       
    }

    public void Fly(InputAction.CallbackContext context)
    {
        if (context.started)
        {
           GroundedBeforeFlying = isGrounded();
        }

        if (context.performed)
        {
            if(!GroundedBeforeFlying) return;
            
            isFlying = true;
            flyingDuration = 1f;
        }

        if (context.canceled)
        {
            isFlying = false;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (CurrentStamina == 0) return;
            
            if (context.performed && canDash)
            {
                StaminaLoss(DashCost);
                StartCoroutine(DashCoroutine());
            }

            if (context.canceled)
            {
                StartRecharge();
            }
        }
    }
    
    public void HardDrop(InputAction.CallbackContext context)
    {
        if (CurrentStamina == 0) return;

        if (context.performed)
        {
            isHardDropping = true;
            hasHardDropped = false;
            StaminaLoss(HardDropCost);
        }

        if (context.canceled)
        {
            isHardDropping = false;
            hasHardDropped = true;
            StartRecharge();
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        
        tr.emitting = true;
        animator.SetBool(IsWalking, false);
        
        float dashDirection = isFacingRight ? 1 : -1;

        if (dashDirection == -1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 60);
        }
        else if (dashDirection == 1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -60);
        }
        
        rb.linearVelocity = new Vector2(dashDirection * DashPower, 0f);
        
        yield return new WaitForSeconds(DashDuration);
        
        rb.linearVelocity = new Vector2(0f, 0f);
        rb.gravityScale = originalGravity;

        isDashing = false;
        tr.emitting = false;
        
        yield return new WaitForSeconds(DashCooldown);
        
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        ApplyFlip();
        
        canDash = true;
    }

    private IEnumerator RechargeStamina()
    {
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

    public bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
        {
            return true;
        }
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    private void StartRecharge()
    {
        if (recharge != null)
        {
            StopCoroutine(recharge);
        }
        recharge = StartCoroutine(RechargeStamina());
    }

    private void StaminaLoss(float cost)
    {
        CurrentStamina -= cost;

        if (CurrentStamina < 0)
        {
            CurrentStamina = 0;
        }
        StaminaBar.fillAmount = CurrentStamina / MaxStamina;
    }

    private void ApplyFlip()
    {
        transform.localScale = new Vector3(
            isFacingRight ? 0.8f : -0.8f, 
            0.8f, 
            1.6f
            );
    }
}