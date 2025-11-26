using System.Collections;
using UnityEngine;

public class FallingGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody2D rb;
    [SerializeField] private Vector3 posB;
    [SerializeField] private float Seconds;
    private float SecondsPassed = 0;
    private Vector3 posA;
    private bool falling;
    void Start()
    {
        posA = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (falling)
        {
            if (SecondsPassed < Seconds)
            {
                SecondsPassed += Time.deltaTime;
                float t = SecondsPassed / Seconds; //Normaliserar värder till [o, 1]
                transform.position = Vector3.Lerp(posA, posB, t);
            }
            if (SecondsPassed < Seconds)
            {
                SecondsPassed += Time.deltaTime;
                float t = SecondsPassed / Seconds; //Normaliserar värder till [o, 1]
                transform.position = Vector3.Lerp(posB, posA, t);
            }
            falling = false;
        }
    }

    public void fall()
    {
        Debug.Log("falling = true");
        falling = true;
    }


    
}
