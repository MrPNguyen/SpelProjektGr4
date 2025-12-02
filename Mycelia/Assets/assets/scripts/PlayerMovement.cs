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
    
    void Start()
    {
        canMove = true;
        hasPlayed = true;
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
        Debug.Log($"{gameObject.name} canMove: {canMove}");
        //ifall man inte kan röra sig sätt allt till falskt
        if (!canMove)
        {
            horizontalMovement = 0;
            isRunning = false;
            isFlying = false;
            isHardDropping = false;
            animator.SetBool(IsWalking, false);
            return;
        }
        
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

        //För vanlig rörelse ifall man inte blir knuffad eller inte har tillräckligt med stamina
        if (!isKnockedBack && CurrentStamina != 0 && canMove)
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

        //Kollar vilket håll du kollar åt.
        if (!isDashing && canMove)
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

        //Tar bort stamina gradually när du springer
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
        
        //Samma som spring ovan gäller här
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
        /* Kollar om man inte kan röra sig då är hastigheten noll samt om man är på marken då är gravity = 4
         annars så är det andra värden beroende på vad man gör */
        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
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
        
        //Här hanteras flyget och man lägger till flyingPower till hastighetens y axel under tiden då flyingDuration inte är 0
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
    
    //Läser av ett x-value som kan vara -1 eller 1 som sedan läggs till linearVelocity som hanteras i Update().
    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    
    //Självaste springet hanteras i Update() men denna säger till Update att gå från walking till Running
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

    /*Kollar först om spelaren kan gå, inte flyger och har nog med stamina samt om den är på marken.
     Sedan ändras gravity så att hoppet kan gå upp samt tar stamina och lägger till hoppstyrkan till linearvelocity*/
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
            rb.gravityScale = 1;
            StaminaLoss(JumpCost);
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            hasPlayed = false;

        }

        // Optional variable jump height
        if (context.canceled)
        {
            isJumping = false;
            StartRecharge();
        }
       
    }

    /*Kollar först om man är på marken innan man kan flyga och sedan ändrar bools för att påbörja flyget.
     Detta hanteras också i FixedUpdate*/
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
            hasPlayed = false;
            flyingDuration = 1f;
        }

        if (context.canceled)
        {
            isFlying = false;
        }
    }

    //Aktiverar Coroutinen
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
    
    //Självaste harddroppet hanteras i FixedUpdate, denna uppdaterar bara bools för att aktivera hardroppen.
    public void HardDrop(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        if (CurrentStamina == 0) return;

        if (context.performed)
        {
            isHardDropping = true;
            hasHardDropped = false;
            hasPlayed = false;
            StaminaLoss(HardDropCost);
        }

        if (context.canceled)
        {
            isHardDropping = false;
            hasHardDropped = true;
            StartRecharge();
        }
    }

    /*Coroutine för Dash där man stänger av gravity under dashens gång och roterar spelaren några grader genom Quaternion.Euler
     Man lägger bara till dashPower till linerVelocity samt direction för det håll spelaren kollar åt*/
    private IEnumerator DashCoroutine()
    {
        if (!canMove) yield break;
        
        canDash = false;
        isDashing = true;
        hasPlayed = false;
        
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

    //Coroutine för att fylla på stamina igen med en liten delay i början.
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

    //Kollar om boxen på OnDrawGizmoSelected() överlappar med valna lager (som väljs i inspektorn)
    public bool isGrounded()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, whatIsGround))
        {
            return true;
        }
        return false;
    }
    
    //Målar en liten låda vid spelarens fötter för att se om spelaren nuddar marken eller ej
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    //Stannar coRoutinen för att fylla på stamina igen
    private void StartRecharge()
    {
        if (recharge != null)
        {
            StopCoroutine(recharge);
        }
        recharge = StartCoroutine(RechargeStamina());
    }

    //funktion för att ta bort stamina beroende på vad man gör (anorlunda action har anorlunda stamina bekostnad)
    private void StaminaLoss(float cost)
    {
        CurrentStamina -= cost;

        if (CurrentStamina < 0)
        {
            CurrentStamina = 0;
        }
        StaminaBar.fillAmount = CurrentStamina / MaxStamina;
    }

    //hanterar att flippa spelaren beroende på vilket håll dem går
    private void ApplyFlip()
    {
        transform.localScale = new Vector3(
            isFacingRight ? 0.8f : -0.8f, 
            0.8f, 
            1.6f
            );
    }
}