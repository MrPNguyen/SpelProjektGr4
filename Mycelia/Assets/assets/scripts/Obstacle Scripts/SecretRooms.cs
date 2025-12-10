using System;
using UnityEngine;
using UnityEngine.UI;

public class SecretRooms : MonoBehaviour
{
    [SerializeField] private GameObject hiddenWalls;
    [SerializeField] private GameObject playerLight;
    [SerializeField] private GameObject KeyNCage;
    [SerializeField] private GameObject Enemy;
    [SerializeField] private GameObject Lock;
    [SerializeField] private Image Background;
    private Color originalColor;

    private void Start()
    {
        originalColor = Background.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            hiddenWalls.SetActive(false);
            playerLight.SetActive(true);
            KeyNCage.SetActive(true);
            if (Enemy != null)
            {
                Enemy.SetActive(true);
            }
            if (Lock != null)
            {
                Lock.SetActive(false);
            }
            Background.color = new Color32(77, 77, 77, 255);
        }
    }
    

    void OnTriggerExit2D(Collider2D other)
    {
        hiddenWalls.SetActive(true);
        playerLight.SetActive(false);
        KeyNCage.SetActive(false);
        if (Enemy != null)
        {
            Enemy.SetActive(true);
        }
        if (Lock != null)
        {
            Lock.SetActive(true);
        }
        Background.color = originalColor;
    }
}
