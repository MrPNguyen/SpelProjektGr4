using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private int maxHealth = 3; 
     public int currentHealth;
     public Vector3 originalPosition;
    
    [SerializeField] private Image Heart1;
    [SerializeField] private Image Heart2;
    [SerializeField] private Image Heart3;
    
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text GameOverPrologText;

    [SerializeField] private List<string> DeathText;
    void Start()
    {
        originalPosition = transform.position;
        currentHealth = maxHealth;
        DeathText = new List<string>();
        if (DeathText.Count > 0)
        {
            GameOverPrologText.text = DeathText[Random.Range(0, DeathText.Count)];
        }
        else
        {
            GameOverPrologText.text = "The Faye has lost its light...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        { 
            Destroy(this.gameObject);
           animator.SetTrigger("isDead");
        }

        if (currentHealth == 2)
        {
            Heart1.enabled = false;
        }
        if (currentHealth == 1)
        {
            Heart2.enabled = false;
        }

        if (currentHealth == 0)
        {
            Heart3.enabled = false;
        }

        if (currentHealth == maxHealth)
        {
            Heart1.enabled = true;
            Heart2.enabled = true;
            Heart3.enabled = true;
        }
    }
    public void TakeDamage()
    {
        currentHealth--;
    }
}
