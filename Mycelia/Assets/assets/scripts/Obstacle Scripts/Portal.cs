using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Portal : MonoBehaviour
{
    [SerializeField] private Animator WinScreen;
    [SerializeField] private GameObject PlayerUI;
    
    [Header("Player rising")]
    private bool isRising = false;
    private float riseSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float targetHeight;
    void FixedUpdate()
    {
        if (isRising)
        {
            Vector2 targetPos = new Vector3(rb.position.x, targetHeight);
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, riseSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            if (rb.position.y >= targetHeight - 2f)
            {
                isRising = false;
                PlayerUI.SetActive(false);
                WinScreen.SetTrigger("Win");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartRising();
        }
    }

    public void StartRising()
    {
        isRising = true;
        playerMovement.velocity = Vector2.zero;
        
    }
}
