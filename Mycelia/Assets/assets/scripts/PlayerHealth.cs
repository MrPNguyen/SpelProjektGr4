using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private int maxHealth = 3; 
     private int currentHealth;
    
    [SerializeField] private Image Heart1;
    [SerializeField] private Image Heart2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            //GameOver
        }

        if (currentHealth == 2)
        {
            Heart1.enabled = false;
        }
        if (currentHealth == 1)
        {
            Heart2.enabled = false;
        }

        if (currentHealth == maxHealth)
        {
            Heart2.enabled = true;
            Heart2.enabled = true;
        }
    }
    public void TakeDamage()
    {
        currentHealth--;
    }
}
