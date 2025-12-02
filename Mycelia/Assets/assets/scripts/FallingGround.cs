using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FallingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;
    [SerializeField] private float posB;
    [SerializeField] private float WaitTime;
    private Vector3 posA;
    private bool falling;




    void Start()
    {
        posA = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;

    }

    
    void Update()
    {

        if (falling)
        {
           
            
            StartCoroutine(Wait(WaitTime));
        }
    }
    
    public void fall()
    {
        falling = true;
    }

    IEnumerator Wait(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (rb.gravityScale == 0)
        { 
            rb.gravityScale = 1;
            
        }
        yield return new WaitUntil(() => transform.position.y <= posA.y-posB);
        
      
        rb.gravityScale = -2;
        
        yield return new WaitUntil(() => transform.position.y >= posA.y);
     
            rb.gravityScale = 0;
            rb.linearVelocityY = 0;
            rb.bodyType = RigidbodyType2D.Static;
            falling = false;
    }
}

