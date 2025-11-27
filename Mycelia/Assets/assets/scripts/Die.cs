using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    [SerializeField] private PlayerMovement playerMovement;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player.currentHealth > 0)
            {
                player.transform.position = player.originalPosition;
                playerMovement.CurrentStamina = playerMovement.MaxStamina;
                playerMovement.StaminaBar.fillAmount = playerMovement.CurrentStamina / playerMovement.MaxStamina;

                player.TakeDamage();
            }
        }
    }
}
