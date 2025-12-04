using UnityEngine;

public class VerticalGround : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created private Rigidbody2D rb;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private Vector3 Originalpos;
    [SerializeField] private float distance = 1;
    [SerializeField] private PlayerMovement player;
   
   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       
        Originalpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (velocity == Vector2.zero || transform.position.y <= Originalpos.y - distance)
        {
            velocity.y = 1;
        }

        rb.linearVelocity = velocity;

        if (transform.position.y >= Originalpos.y + distance)
        {
            velocity.y = -1;
        }

        rb.linearVelocity = velocity;



    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
}
