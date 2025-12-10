using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    
    [SerializeField] private float enemyMoveSpeed = 25f;
    private float WaitBeforeWalking = 2f;

    private float EnemyDirection;
    
    private void Start()
    {
        EnemyDirection = 1f;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        WaitBeforeWalking -= Time.deltaTime;

        if (WaitBeforeWalking <= 0)
        {
            rb.linearVelocity = new Vector2(EnemyDirection * enemyMoveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PointA"))
        {
            EnemyDirection = 1;
            sr.flipX = true;
        }
        
        else if (other.gameObject.CompareTag("PointB"))
        {
            EnemyDirection = -1;
            sr.flipX = false;
        }
    }


    
   
}
