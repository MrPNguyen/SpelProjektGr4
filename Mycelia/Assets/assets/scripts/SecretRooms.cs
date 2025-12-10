using UnityEngine;

public class SecretRooms : MonoBehaviour
{
    [SerializeField] private GameObject hiddenWalls;
    [SerializeField] private GameObject playerLight;

    void OnTriggerStay2D(Collider2D other)
    {
        hiddenWalls.SetActive(false);
        playerLight.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hiddenWalls.SetActive(true);
        playerLight.SetActive(false);
    }
}
