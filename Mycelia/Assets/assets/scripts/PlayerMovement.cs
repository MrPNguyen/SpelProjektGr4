using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;


//Code Source: Game Code Library: "2D Platformer Unity"
public class PlayerMovement : MonoBehaviour
{
    //Press down key to fall down quicker
    [HideInInspector] public Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Vector2 originalColliderOffset;
    
    public bool isFacingRight = true;
    public bool isKnockedBack = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;
    private float horizontalInput;

    [Header("Running")]
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] public bool isRunning;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] public float HardDropPower = 4;
    public bool isHardDropping = false;
    public bool hasHardDropped = false;
    public bool isJumping = false;
    
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private List<LayerMask> whatIsGround;
    
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

    private Vector2 velocity;
    private float multiplier;
    private bool moveLeft = true;
    private bool moveRight = true;
    
    
    private Coroutine recharge;
    
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

        if (!isGrounded())
        {
            isJumping = false;
        }
        //animator.SetBool("isWalking", false);
        if (isDashing)
        {
            return;
        }

        if (!isKnockedBack && CurrentStamina != 0)
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

        if (!isDashing)
        {
            if (horizontalMovement < 0)
            {
                spriteRenderer.flipX = true;
                isFacingRight = false;
                cc.offset = new Vector2(-Mathf.Abs(originalColliderOffset.x), originalColliderOffset.y);
                //animator.SetBool("isWalking", true);
            }
            else if (velocity.x > 0)
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;
                cc.offset = new Vector2(Mathf.Abs(originalColliderOffset.x), originalColliderOffset.y);
               //animator.SetBool("isWalking", true);
            }
        }

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
        wallCheck();
        rb.linearVelocity = velocity;
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x > 1 && !moveRight) horizontalMovement = 0;
        if (context.ReadValue<Vector2>().x < -1 && !moveLeft) horizontalMovement = 0;
        
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
                velocity = new Vector2(velocity.x, velocity.y * 0.3f);
                StartRecharge();
                isJumping = true;
            }

            
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
        //Debug.Log($"Start of Coroutine: {velocity}");

        canDash = false;
        isDashing = true;
        
        float originalGravity = multiplier;
        multiplier = 0;
        
        tr.emitting = true;
        
        float dashDirection = isFacingRight ? 1 : -1;

        cc.offset = new Vector2(dashDirection * Mathf.Abs(originalColliderOffset.x), originalColliderOffset.y);

        if (dashDirection == -1)
        {
            //animator.SetBool("isDashingLeft", true);
        }
        else if (dashDirection == 1)
        {
            //animator.SetBool("isDashingRight", true);
        }
        
        velocity = new Vector2(dashDirection * DashPower, 0f);
        
        //Debug.Log($"During Dash: {velocity}");
        yield return new WaitForSeconds(DashDuration);
        
        velocity = new Vector2(0f, 0f);
        multiplier = originalGravity;

        cc.offset = new Vector2(dashDirection * Mathf.Abs(originalColliderOffset.x), originalColliderOffset.y);
        
        isDashing = false;
        tr.emitting = false;
        
        if (dashDirection == -1)
        {
            animator.SetBool("isDashingLeft", false);
        }
        else if (dashDirection == 1)
        {
            //animator.SetBool("isDashingRight", false);
        }        
        
        Debug.Log($"After Dash: {velocity}");

        yield return new WaitForSeconds(DashCooldown);
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
        foreach (var mask in whatIsGround)
        {
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, mask))
            {
                return true;
            }
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

    private void ApplyGravity()
    {
        if (isGrounded() && !isJumping)
        {
            velocity.y = 0;
            foreach (var mask in whatIsGround)
            {
                Vector2 pos = new Vector2(transform.position.x + Vector2.up.x, transform.position.y + Vector2.up.y);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 5f, mask);
                if (hit.collider != null)
                {
                    Vector2.Angle(transform.position, hit.point);
                    Debug.Log("Angle of slope: " + Vector2.Angle(transform.position, hit.normal));
                }
            }
        }

        else
        {
            velocity.y += Physics2D.gravity.y * multiplier * Time.deltaTime;
        }
        
        
    }

    private void wallCheck()
    {
        foreach (var mask in whatIsGround)
        {
            if (Physics2D.Raycast(transform.position, Vector2.right, 0.2f, mask))
            {
                Debug.Log("No More Right");
                velocity.x = 0;
            }
            else moveRight = false;
            
            if (Physics2D.Raycast(transform.position, Vector2.left, 0.2f, mask))
            {
                velocity.x =0;
            }
            else moveLeft = false;
            if (Physics2D.Raycast(transform.position, Vector2.up, 0.2f, mask))
            {
                velocity.y = 0;
            }
            
        }
    }
}