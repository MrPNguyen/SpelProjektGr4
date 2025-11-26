using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player.currentHealth > 0)
            {
                player.transform.position = player.originalPosition;
                player.TakeDamage();
            }
        }
    }
}
