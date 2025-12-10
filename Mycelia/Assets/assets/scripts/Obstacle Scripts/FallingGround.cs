using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class FallingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float posB;
    [SerializeField] private float WaitTime;
    [SerializeField] private float multiplier = 1;
    private Vector3 posA;
    private bool falling;
    void Start()
    {
        posA = transform.position;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && falling == false)
        {
            Debug.Log("OnTriggerEnter2D");
            StartCoroutine(Wait(WaitTime));    
        }
    }
    
    IEnumerator Wait(float WaitTime)
    {
        Debug.Log("Wait triggered");
        falling = true;
        yield return new WaitForSeconds(WaitTime);
        
        Vector3 targetPosition = posA + Vector3.down * posB;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, multiplier * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        
        while (Vector3.Distance(transform.position, posA) > 0.01f)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, posA, multiplier * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }
        
        falling = false;
    }
}

