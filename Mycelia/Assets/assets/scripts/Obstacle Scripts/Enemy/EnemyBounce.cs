using UnityEngine;

public class EnemyBounce : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;
 


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= 2 && Bounce > 0)
            {
               
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = Bounce;
            }
            
        }
    }
}
