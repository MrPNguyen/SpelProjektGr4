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

    }

    
    void Update()
    {

        float i = 0;
        if (falling)
        {
            Debug.Log(rb.gravityScale);
            if (rb.gravityScale == 0)
            { 
                rb.gravityScale = 1;
            
            }
            StartCoroutine(Wait(WaitTime));
        }
    }
    
    public void fall()
    {
        falling = true;
    }

    IEnumerator Wait(float WaitTime)
    {
        yield return new WaitUntil(() => transform.position.y <= posA.y-posB);
        
        Debug.Log("We're in");
        rb.gravityScale = -2;
        
        yield return new WaitUntil(() => transform.position.y >= posA.y);
        Debug.Log("We're out");
            rb.gravityScale = 0;
            rb.linearVelocityY = 0;
            falling = false;
    }
}

