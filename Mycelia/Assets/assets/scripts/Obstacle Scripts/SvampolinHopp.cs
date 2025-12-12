using Unity.VisualScripting;
using UnityEngine;

public class SvampolinHopp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;




    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= 2)
            {
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = Bounce;
           
            }
        
        }
    }

}
