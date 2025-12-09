using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FallingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;
    [SerializeField] private float posB;
    [SerializeField] private float WaitTime;
    [SerializeField] private float multiplier =1;
    private Vector3 posA;
    private bool falling;
    void Start()
    {
        posA = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }
    public void fall()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Falling Ground");
            StartCoroutine(Wait(WaitTime));    
        }
    }
    
    IEnumerator Wait(float WaitTime)
    {
        Debug.Log("Hit");
        yield return new WaitForSeconds(WaitTime);
        
        if (rb.gravityScale == 0)
        { 
            rb.gravityScale = multiplier;
        }
        yield return new WaitUntil(() => transform.position.y <= posA.y-posB);
        
        rb.gravityScale = -2;
        
        yield return new WaitUntil(() => transform.position.y >= posA.y);
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
            
        falling = false;
    }
}

