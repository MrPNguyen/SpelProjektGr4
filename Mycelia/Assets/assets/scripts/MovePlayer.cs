using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private Rigidbody2D rb;
   private SpriteRenderer spriteRenderer;
   private Animator animator;
      
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

   private float horizontalMovement;
   [SerializeField] private float speed;
   private Vector2 velocity;
   [SerializeField] private float multiplier;
   
   public bool canMove = true;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

   
    void Update()
    {
        animator.SetBool("hasJumping", isJumping); //fungerar detta?

        if (velocity.y <= 0)
        {
            isJumping = false;
        }
        animator.SetBool("isWalking", false);

        if (Input.GetButtonDown("space") && !isJumping && isGrounded())
        {
            isJumping = true;
            velocity.y += Time.deltaTime * jumpForce;
        }
        if (Input.GetButtonDown("a"))
        {
            velocity.x += Time.deltaTime * speed;
        }
        
        if (Input.GetButtonDown("d"))
        {
            velocity.x -= Time.deltaTime * speed; 
        }
        
    }

    void FixedUpdate()
    {
        ApplyGravity();
        isGrounded();
        rb.linearVelocity = velocity;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            horizontalMovement = context.ReadValue<Vector2>().x;
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
    private void ApplyGravity()
    {
        // Applies a set gravity for when player is grounded
        if (isGrounded() && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        }
// Updates fall speed with gravity if object isn't grounded
        else
        {
// Is Jumping upwards
            if (velocity.y > 0)
            {
                float deceleration = 5;
                
// you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
// Is Falling
            else
            {
// you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * Time.deltaTime;
            }
        }
    }
}
