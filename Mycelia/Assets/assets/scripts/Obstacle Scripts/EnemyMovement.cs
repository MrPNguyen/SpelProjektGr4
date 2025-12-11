using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float enemyMoveSpeed = 3f;
    [SerializeField] public float waitBeforeWalking = 2f;
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private float EnemyDirection = 1f;
    private bool canMove = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(StartMovingAfterDelay(waitBeforeWalking));
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Vector2 newPos = rb.position + new Vector2(enemyMoveSpeed * EnemyDirection, 0) * Time.fixedDeltaTime;
        rb.MovePosition(newPos);
    }

    private IEnumerator StartMovingAfterDelay(float delay)
    {
        canMove = false;
        anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(delay);

        canMove = true;
        anim.SetBool("isWalking", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PointA"))
        {
            EnemyDirection = 1;
            sr.flipX = true;
        }
        else if (other.CompareTag("PointB"))
        {
            EnemyDirection = -1;
            sr.flipX = false;
        }
    }

    // Call this after respawn
    public void StartMovingAfterRespawn(float riseLength)
    {
        StartCoroutine(MoveAfterRespawn(riseLength));
    }

    private IEnumerator MoveAfterRespawn(float riseLength)
    {
        canMove = false;
        anim.SetBool("isWalking", false);

        yield return new WaitForSeconds(riseLength + waitBeforeWalking);

        canMove = true;
        anim.SetBool("isWalking", true);
    }
}