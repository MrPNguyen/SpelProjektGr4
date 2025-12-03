using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;


//Code Source: Game Code Library: "2D Platformer Unity"
public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int HasJumping = Animator.StringToHash("hasJumping");
    
    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Vector2 originalColliderOffset;
    public bool hasPlayed;
    
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

    public bool canMove;

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
    
    [NonSerialized] public Vector2 velocity;
    private float multiplier;
    private bool moveLeft = true;
    private bool moveRight = true;
    private Vector3 SafePosition = Vector3.zero;
    
    [Header("WallCheck")]
    [SerializeField] private Transform LeftWallCheck;
    
    
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         animator = GetComponent<Animator>();
         tr = GetComponent<TrailRenderer>();
         cc = GetComponent<CapsuleCollider2D>();
         CurrentStamina = MaxStamina;
         originalColliderOffset = cc.offset;
    }

    void Update()
    {
        //animator.SetBool("hasJumping", isJumping); //fungerar detta?

        if (!canMove)
        {
            horizontalMovement = 0;
            isRunning = false;
            isFlying = false;
            isHardDropping = false;
            animator.SetBool(IsWalking, false);
            return;
        }
        
        if (velocity.y <= 0)
        {
            isJumping = false;
        }
        
        //animator.SetBool("isWalking", false);
        
        if (isDashing)
        {
            return;
        }

        if (!isKnockedBack && CurrentStamina != 0 && canMove)
        {
            if (isRunning)
            {
                velocity = new Vector2(horizontalMovement * runSpeed, velocity.y);
            }
            
            else
            {
                velocity = new Vector2(horizontalMovement * moveSpeed, velocity.y);
            }
        }

        if (!isDashing && canMove)
        {
            if (horizontalMovement < 0)
            {
                isFacingRight = false;
                //animator.SetBool("isWalking", true);
            }
            else if (horizontalMovement > 0)
            {
                isFacingRight = true;
               //animator.SetBool("isWalking", true);
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
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        if (isGrounded())
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
        if (isFlying)
        {
            flyingDuration -= Time.deltaTime;

            if (flyingDuration <= 0)
            {
                isFlying = false;
            }
            else
            {
                velocity = new Vector2(velocity.x, flyingPower);
            }
        }
        
        ApplyGravity();
        IsWalled();
       
        rb.linearVelocity = velocity;
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    
    public void Run(InputAction.CallbackContext context)
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
            multiplier = 1;
            StaminaLoss(JumpCost);
            
            velocity = new Vector2(velocity.x, jumpForce);
            isJumping = true;
          
        }

        // Optional variable jump height
        if (context.canceled)
        {
            if (velocity.y > 0)
            {
                isJumping = false;
                StartRecharge();
            }
        }
     }

    public void Fly(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
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
        if (!canMove) return;
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
    
    public void HardDrop(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        if (CurrentStamina == 0) return;

        if (context.performed)
        {
            isHardDropping = true;
            StaminaLoss(HardDropCost);
        }

        if (context.canceled)
        {
            isHardDropping = false;
            StartRecharge();
        }
    }

    private IEnumerator DashCoroutine()
    {

        canDash = false;
        isDashing = true;
        
        float originalGravity = multiplier;
        multiplier = 0;
        
        tr.emitting = true;
        
        float dashDirection = isFacingRight ? 1 : -1;
        
        Debug.Log($"Start of Coroutine: {dashDirection}");

        if (dashDirection == -1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 60);
        }
        else if (dashDirection == 1)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -60);
        }
        
        Debug.Log($"Middle of Coroutine Rotation: {transform.localRotation}");
        
        velocity = new Vector2(dashDirection * DashPower, 0f);
        
        //Debug.Log($"During Dash: {velocity}");
        yield return new WaitForSeconds(DashDuration);
        
        velocity = new Vector2(0f, 0f);
        multiplier = originalGravity;

        isDashing = false;
        tr.emitting = false;
        
        yield return new WaitForSeconds(DashCooldown);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        ApplyFlip();
        
        canDash = true;
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

    public bool isGrounded()
    {
        if (hasHardDropped)
        {
            groundCheck.position = new Vector3(groundCheck.position.x, groundCheck.position.y - 2f, groundCheck.position.z);
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
            {
                return true;
            }
        }
        else
        {
            groundCheck.position = new Vector3(groundCheck.position.x, groundCheck.position.y, groundCheck.position.z);
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
            {
                return true;
            }
        }
        
        return false;
    }

    private void OnDrawGizmosSelected()
    
    {
            if (isFacingRight)
            {
                LeftorRight = 0.3f;
            }
            else
            {
                LeftorRight = -0.3f;
            }
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube( new Vector2(transform.position.x + LeftorRight, transform.position.y),  new Vector2(0.05f, 0.6f));
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

    private void ApplyGravity()
    {
        if (isGrounded() && !isJumping && !isKnockedBack)
        {
            velocity.y = 0;
            
        }

        else
        {
            velocity.y += Physics2D.gravity.y * multiplier * Time.deltaTime;
        }
    }

    float LeftorRight = -2;
        
    

    private void IsWalled()
    {
        Vector3 position = transform.position;
        Vector2 Size = new Vector2(0.05f, 0.6f);
        
        if (isFacingRight)
        {
            LeftorRight = 0.3f;
        }
        else
        {
            LeftorRight = -0.3f;
        }
        Vector2 dir = new Vector2(transform.position.x + LeftorRight, transform.position.y);
       
        if (Physics2D.OverlapBox(dir, Size, 0, whatIsGround))
        { Collider2D colliders = Physics2D.OverlapBox(dir, Size, 0, whatIsGround);
            Debug.Log($"colliders: {colliders.gameObject.name}");
            if (LeftorRight == 0.3f)
            {
                moveRight = false;
            }
            else
            {
                moveLeft = false;
            }
            position.x = SafePosition.x;
            transform.position = position;
        }
        else
        {
            SafePosition = transform.position;
        }
    }
    
}