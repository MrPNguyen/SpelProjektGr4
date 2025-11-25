using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Code Source: Game Code Library: "2D Platformer Unity"
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;
    private float horizontalInput;

    [Header("Jump")]
    public float jumpForce = 10f;
    
    [Header("GroundCheck")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask whatIsGround;
    
    private SpriteRenderer spriteRenderer;
    //private Animator animator;

    public bool canMove = true;

    [Header("Dash")]
    public float DashPower = 5f;
    private bool isDashing = false;
    private float DashDuration = 0.10f;
    private float DashTimer;
    void Start()
    {
         rb = GetComponent<Rigidbody2D>();
         spriteRenderer = GetComponent<SpriteRenderer>();
         //animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (rb.linearVelocity.x < 0)
        {
            spriteRenderer.flipX = true;
            //animator.SetBool("isMoving", true);
        }
        else if (rb.linearVelocity.x > 0)
        {
            spriteRenderer.flipX = false;
            //animator.SetBool("isMoving", true);

        }
        else
        {
            spriteRenderer.flipX = false;
            //animator.SetBool("isMoving", false);
        }
        
        if (isDashing)
        {
            DashTimer -= Time.deltaTime;
            if (DashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalMovement * DashPower, 0f);
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
                if (context.performed)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    //animator.SetBool("hasJumped", true);

                }
                else if (context.canceled)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                    //animator.SetBool("hasJumped", true);
                }
            }
            else
            {
                //animator.SetBool("hasJumped", false);
            }
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            if (context.performed)
            {
                isDashing = true;
                Debug.Log(isDashing);
                DashTimer = DashDuration;
            }
        }
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