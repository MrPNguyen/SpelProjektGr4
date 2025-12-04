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
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsFlying = Animator.StringToHash("isFlying");
    private static readonly int IsHarddropping = Animator.StringToHash("isHarddropping");
    private static readonly int HasFallen = Animator.StringToHash("hasFallen");

    
    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
    private CapsuleCollider2D cc;
    [HideInInspector] public bool hasPlayed;
    
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool isKnockedBack = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;

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
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private Vector2 ceilingCheckSize = new Vector2(0.2f,0.002f);
    [SerializeField] private LayerMask whatIsGround;
    
    
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [HideInInspector] public bool canMove;

    [Header("Dash")]
    [SerializeField] private float DashPower = 20f;
    [HideInInspector] public bool isDashing;
    private bool canDash = true;
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

    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         animator = GetComponent<Animator>();
         tr = GetComponent<TrailRenderer>();
         cc = GetComponent<CapsuleCollider2D>();
         CurrentStamina = MaxStamina;
         isFlying = false;
         isHardDropping = false;
         hasHardDropped = false;
         isJumping = false;
    }

    void Update()
    {
        if (isDashing)
        {
            ceilingCheckSize = new Vector2(0.002f,0.2f);
        }
        else
        {
            ceilingCheckSize = new Vector2(0.2f,0.002f);
        }
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

        if (!isDashing && canMove)
        {
            if (horizontalMovement < 0)
            {
                isFacingRight = false;
                animator.SetBool(IsWalking, true);
            }
            else if (horizontalMovement > 0)
            {
                isFacingRight = true;
                animator.SetBool(IsWalking, true);
            }
            else
            {
                animator.SetBool(IsWalking, false);
            }
        }

        ApplyFlip();

        GradualStaminaUse(RunCost, isRunning);
        
        GradualStaminaUse(FlyingCost, isFlying);
        
        GradualStaminaUse(DashCost, isDashing);
        
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
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            velocity = Vector2.zero;
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
       
        if (isFlying)
        {
            flyingDuration -= Time.fixedDeltaTime;

            if (flyingDuration <= 0)
            {
                isFlying = false;
                animator.SetBool(IsFlying, false);
                animator.SetBool(HasFallen, true);
            }
            else
            {
                animator.SetBool(IsFlying, true);
                velocity = new Vector2(velocity.x, flyingPower);
            }
        }
        
        if (isDashing)
        {
            DashDuration -= Time.deltaTime;

            if (DashDuration <= 0)
            {
                isDashing = false;
            }
            else
            {
                if (isFacingRight)
                {
                    velocity = new Vector2(DashPower, velocity.y);
                }
                else
                {
                    velocity = new Vector2(-DashPower, velocity.y);
                }
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
            
            velocity = new Vector2(velocity.x, jumpForce);
            isJumping = true;
            animator.SetBool(IsJumping, true);
        }
        
        if (context.canceled)
        {
            if (velocity.y > 0)
            {
                isJumping = false;
                animator.SetBool(IsJumping, false);
                animator.SetBool(HasFallen, true);
            }
        }
    }

    public void Fly(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
        if (CurrentStamina == 0) return;
        
        if (context.started)
        {
            GroundedBeforeFlying = isGrounded();
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
        if (CurrentStamina == 0) return;
            
        if (context.performed && canDash)
        {
            CeilingCheck.localRotation = Quaternion.Euler(0, 180, -60);
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
            tr.emitting = true;
        }

        if (context.canceled)
        {
            isDashing = false;
            tr.emitting = false;

            DashDuration = 1f;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    public void HardDrop(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        if (CurrentStamina == 0) return;

        if (context.performed)
        {
            animator.SetBool(IsHarddropping, true);
            hasHardDropped = true;
            isHardDropping = true;
        }

        if (context.canceled)
        {
            animator.SetBool(IsHarddropping, false);
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube( CeilingCheck.position, ceilingCheckSize);
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
        if (!isFlying)
        {
            velocity.y += Physics2D.gravity.y * multiplier * Time.fixedDeltaTime;
        }

        // Stop downward velocity if grounded
        if (isGrounded() && velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    float LeftorRight = -2;

    private void IsWalled()
    {
        Vector3 position = transform.position;
        Vector3 CeilingPosition = transform.position;
        Vector2 WallCheckSize = new Vector2(0.05f, 0.55f);
        
        if (isFacingRight)
        {
            LeftorRight = 0.3f;
        }
        else
        {
            LeftorRight = -0.3f;
        }
        Vector2 dir = new Vector2(transform.position.x + LeftorRight, transform.position.y);
       
        if (Physics2D.OverlapBox(dir, WallCheckSize, 0, whatIsGround))
        {
            position.x = SafePosition.x;
            transform.position = position;
        }
        else
        {
            SafePosition = transform.position;
        }
       
        if (Physics2D.OverlapBox(CeilingCheck.position, ceilingCheckSize, 0, whatIsGround))
        {
            if (isDashing)
            {
                velocity.x = 0;
                
                CeilingPosition.x = SafeCeilingPosition.x;
                transform.position = CeilingPosition;
            }
            else
            {
                CeilingPosition.y = SafeCeilingPosition.y;
                transform.position = CeilingPosition;
            }
        }
        else
        {
            SafeCeilingPosition = transform.position;
        }
    }
    
}