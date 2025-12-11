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

    void Start()
    {
        posA = objectPos.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && falling == false)
        {
            falling = true;
            StartCoroutine(Wait(WaitTime));
        }
    }


    IEnumerator Wait(float WaitTime)
    {
        Debug.Log($"wait 0 seconds");
        yield return new WaitForSeconds(WaitTime);
        
        if (rb.gravityScale == 0)
        {
            rb.gravityScale = multiplier;
        }

        yield return new WaitUntil(() => objectPos.position.y <= posA.y - posB);
        Debug.Log("fallen");
        rb.gravityScale = -2;
       
        yield return new WaitUntil(() => objectPos.position.y >= posA.y);
        Debug.Log("Up again");
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        objectPos.position = posA;
        
        falling = false;
       
    }

}



