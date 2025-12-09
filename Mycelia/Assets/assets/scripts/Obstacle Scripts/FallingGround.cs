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
    
    void Update()
    {
       
    }
    
    public void fall()
    {
        if (falling) return;
        falling = true;
        StartCoroutine(Wait(WaitTime));
    }
    
    IEnumerator Wait(float WaitTime)
    {
        
        yield return new WaitForSeconds(WaitTime);
        
        if (rb.gravityScale == 0)
        { 
            rb.gravityScale = multiplier;
        }
        yield return new WaitUntil(() => transform.position.y <= posA.y-posB);
        
        rb.gravityScale = -2;
        Debug.Log("Falling");
        yield return new WaitUntil(() => transform.position.y >= posA.y);
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        transform.position = posA; 
        falling = false;
    }
}

