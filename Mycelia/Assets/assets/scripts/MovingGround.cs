using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private Rigidbody2D rb;
   private Vector2 velocity;
   private Vector3 Originalpos;
   [SerializeField] private float distance = 1;
   [SerializeField] private PlayerMovement player;
   private BoxCollider2D bc;
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        Originalpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (velocity == Vector2.zero || transform.position.x <= Originalpos.x - distance)
        { 
            velocity.x = 1;
        }
       
        rb.linearVelocity = velocity;
      
        if (transform.position.x >= Originalpos.x + distance)
        {
            velocity.x = -1;
        }
       
        rb.linearVelocity = velocity;
        
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            player.velocity.x = velocity.x;
            
        }
    }
    
}
