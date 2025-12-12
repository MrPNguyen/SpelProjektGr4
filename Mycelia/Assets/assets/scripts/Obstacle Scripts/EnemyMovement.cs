using System;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
     private Rigidbody2D rb;
    private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;

    [SerializeField] private float enemyMoveSpeed = 3f;
    public float WaitBeforeWalking = 2f;

    private float EnemyDirection;
    private bool canMove = false;

    private void Start()
    {
        EnemyDirection = 1f;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(DelayMovementCoroutine());
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector2 newPos = rb.position + new Vector2(enemyMoveSpeed * EnemyDirection, 0) * Time.fixedDeltaTime;
            Debug.Log($"newPosition: {newPos}");
            rb.MovePosition(newPos);
        }
    }

    public void StartMovingAfterRespawn(float riseLength)
    {
        canMove = false;
        anim.SetBool("isWalking", false);
        StartCoroutine(DelayMovementCoroutine());
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

    public void StartMovementAfterDelay()
    {
        StartCoroutine(DelayMovementAfterRespawnCoroutine());
    }

    private IEnumerator DelayMovementAfterRespawnCoroutine()
    {
        canMove = false;
        anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(WaitBeforeWalking);

        yield return new WaitForSeconds(WaitBeforeWalking);
        canMove = true;
        anim.SetBool("isWalking", true);
    }

    private IEnumerator DelayMovementCoroutine()
    {
        canMove = false;
        anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(WaitBeforeWalking); // initial wait
        canMove = true;
        anim.SetBool("isWalking", true);
    }
}