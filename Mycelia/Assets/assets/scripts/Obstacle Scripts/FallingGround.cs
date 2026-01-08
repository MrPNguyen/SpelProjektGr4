using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class FallingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform objectPos;
    [SerializeField] private float posB;
    [SerializeField] private float WaitTime;
    [SerializeField] private float multiplier = 1;
    private Vector3 posA;
    private bool falling;
    private bool pause;
    [SerializeField] private float fallSpeed = -1f;
    private Coroutine lastRoutine;

    void Start()
    {
        posA = objectPos.position;
    }

    void Update()
    {
        if (objectPos.position.y > posA.y) objectPos.position = posA;
        if (!falling && objectPos.position.y != posA.y) objectPos.position = posA;
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !falling)
        {
            falling = true;
           lastRoutine = StartCoroutine(Wait(WaitTime));
        }

        if (other.tag == "Player" && falling && rb.gravityScale == -2f)
        {
            Debug.Log($"last coroutine: {lastRoutine}");
            if (lastRoutine != null)
            {
                StopCoroutine(lastRoutine);
                lastRoutine = StartCoroutine(Wait(0));
            }
           
        }
     
    }

    IEnumerator Wait(float WaitTime)
    {
        //Debug.Log($"wait 0 seconds");
        yield return new WaitForSeconds(WaitTime);
        
        if (rb.gravityScale == 0)
        {
            rb.gravityScale = multiplier;
            rb.linearVelocity = new Vector2(0, fallSpeed);
        }

        yield return new WaitUntil(() => objectPos.position.y <= posA.y - posB);
      
            rb.gravityScale = -2;
        
   
        yield return new WaitUntil(() => objectPos.position.y >= posA.y);
        //Debug.Log("Up again");
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        objectPos.position = posA;
        
        falling = false;
       
    }
    

}



