using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Code Source: Game Code Library: "2D Platformer Unity"
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float horizontalMovement;
    private float horizontalInput;

    [Header("Jump")]
    public float jumpForce = 10f;
    
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private LayerMask whatIsGround;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public bool canMove = true;

    [Header("Dash")]
    [SerializeField] private float DashPower = 20f;
    private bool isDashing;
    private bool canDash = true;
    private float DashDuration = 0.10f;
    private float DashCooldown = 0.1f;
    
    [Header("Flying")]
    [SerializeField] private float flyingPower = 15f;
    [SerializeField] private float flyingCooldown = 5f;
    private bool isFlying = false;
    
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
     
        if (rb.linearVelocity.x < 0)
        {
            spriteRenderer.flipX = true;
            isFacingRight = false;
            //animator.SetBool("isMoving", true);
        }
        else if (rb.linearVelocity.x > 0)
        {
            spriteRenderer.flipX = false;
            isFacingRight = true;
            //animator.SetBool("isMoving", true);
        }
        else
        {
            spriteRenderer.flipX = false;
            isFacingRight = true;
            //animator.SetBool("isMoving", false);
        }
    }

    void FixedUpdate()
    {
        if (isFlying && flyingCooldown > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, flyingPower);
            flyingCooldown -= Time.deltaTime;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (isGrounded())
            {
                if (context.started)
                {
                    Debug.Log("Started");
                    isFlying = true;
                    //Add Interaction: Hold i InputSystem för flying
                }
                
                if (context.performed)
                {
                    Debug.Log("Performed");
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    //animator.SetBool("hasJumped", true);

                }
                
                if (context.canceled)
                {
                    if (rb.linearVelocity.y > 0)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.3f);
                    }
                    isFlying = false;
                    //animator.SetBool("hasJumped", true);
                }
            }
            else
            {
                //animator.SetBool("hasJumped", false);
            }
        }
    }

    public void Fly(InputAction.CallbackContext context)
    {
        
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed && canDash)
            {
                Debug.Log("Q");
                StartCoroutine(DashCoroutine());
            }
        }
    }

    private IEnumerator DashCoroutine()
    {
        Debug.Log("DashCoroutine");
        canDash = false;
        isDashing = true;
        
        float dashDirection = isFacingRight ? 1 : -1;

        if (dashDirection == -1)
        {
            animator.SetBool("isDashingLeft", true);
        }
        else if (dashDirection == 1)
        {
            animator.SetBool("isDashingRight", true);
        }
        
        rb.linearVelocity = new Vector2(dashDirection * DashPower, rb.linearVelocity.y);
        
        yield return new WaitForSeconds(DashDuration);
        
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        
        isDashing = false;
        if (dashDirection == -1)
        {
            animator.SetBool("isDashingLeft", false);
        }
        else if (dashDirection == 1)
        {
            animator.SetBool("isDashingRight", false);
        }        
        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private bool isGrounded()
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
}