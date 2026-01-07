using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Portal : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator WinScreen;
    [SerializeField] private GameObject PlayerUI;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SpriteRenderer PlayerSprite;
    [SerializeField] private GameObject portalSign;
        
    [Header("Player rising")]
    private bool isRising = false;
    private bool hasTriggered = false;
    [SerializeField] private float riseSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float targetHeight;
    private Collider2D coll;

    void Start()
    {
        coll = GetComponent<Collider2D>();
        coll.isTrigger = false;
    }
    void FixedUpdate()
    {
        if (isRising)
        {
            
            Vector2 targetPos = new Vector3(rb.position.x, targetHeight);
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, riseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (rb.position.y >= targetHeight - 0.05f)
            {
                isRising = false;
                PlayerUI.SetActive(false);
                WinScreen.SetTrigger("Win");
                
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(hasTriggered) return;
        if (other.tag == "Player")
        {
            portalSign.SetActive(true);
            Debug.Log("Portal Sign active in hirearchy"+ portalSign.activeInHierarchy);
            hasTriggered = true;
            StartRising();
        }
    }

    public void StartRising()
    {
        Debug.Log("StartRising");
       
        isRising = true;
        coll.isTrigger = true;
        PlayerSprite.sortingOrder = -1;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
    }
}
