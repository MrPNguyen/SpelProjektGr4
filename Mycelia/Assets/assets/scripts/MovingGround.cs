using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private Rigidbody2D rb;
   private Vector2 velocity;
   private float distance;
   [SerializeField] private PlayerMovement player;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void onTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.
        }
    }

    private IEnumerator Move()
    {
        velocity.x = 1;
        rb.linearVelocity = velocity;
        yield return new WaitUntil(() => transform.position.x >= transform.position.x + distance);
        velocity.x = -1;
        rb.linearVelocity = velocity;
        yield return new WaitUntil(() => transform.position.x <= transform.position.x - distance);
    }
}
