using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    [SerializeField] private PlayerMovement playerMovement;
    private Vector3 RespawnPosition;

    void Start()
    {
        RespawnPosition = player.originalPosition;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player.currentHealth > 0)
            {
                player.transform.position = RespawnPosition;
                playerMovement.CurrentStamina = playerMovement.MaxStamina;
                playerMovement.StaminaBar.fillAmount = playerMovement.CurrentStamina / playerMovement.MaxStamina;

                player.TakeDamage();
            }
        }
    }

    public void SetSpawnPosition(Transform transform)
    {
        RespawnPosition = transform.position;
    }

   
}
