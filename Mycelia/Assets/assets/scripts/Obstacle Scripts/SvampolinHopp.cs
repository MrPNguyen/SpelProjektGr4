using Unity.VisualScripting;
using UnityEngine;

public class SvampolinHopp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;
    void Start()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= playerMovement.HardDropPower)
            {
                playerMovement.isJumping = true;
                playerMovement.velocity.y = Bounce;
            }
            else playerMovement.isJumping = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
