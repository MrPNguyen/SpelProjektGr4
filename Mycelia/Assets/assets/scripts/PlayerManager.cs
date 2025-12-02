using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    public Vector3 originalPosition;
    
    [Header("Health")]
    private int maxHealth = 3; 
    public int currentHealth;
    [SerializeField] private Image Heart1;
    [SerializeField] private Image Heart2;
    [SerializeField] private Image Heart3;
    
    [Header("Death")]
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text GameOverPrologText;
    [SerializeField] private List<string> DeathText;
    [SerializeField] private float HitRecoil = 20;
    private Material player;
    [SerializeField] private GameObject enemy;
    
    [Header("Extra Life")]
    [SerializeField] private GameObject extraLife;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        player = GameObject.Find("Player").GetComponent<Renderer>().material;
        originalPosition = transform.position;
        currentHealth = maxHealth;
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

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (playerMovement.rb.gravityScale >= playerMovement.HardDropPower && !playerMovement.isGrounded())
            {
                Destroy(enemy);
                Instantiate(extraLife, other.transform.position, other.transform.rotation);
                
            }
            else
            {
                TakeDamage();
                player.color = Color.red;
                float HitRecoilX = 10f * (playerMovement.isFacingRight ? -1 : 1);
                float HitRecoilY = HitRecoil;
                rb.linearVelocity = new Vector2(HitRecoilX, HitRecoilY);

                StartCoroutine(KnockbackCoroutine(0.2f));
            }
        }

        if (other.tag == "ExtraLife")
        {
            currentHealth++;
            Destroy(other.gameObject);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            player.color = Color.white;
        }
    }
    
    private IEnumerator KnockbackCoroutine(float duration)
    {
        playerMovement.isKnockedBack = true;
        playerMovement.hasPlayed = false;
        yield return new WaitForSeconds(duration);
        playerMovement.isKnockedBack = false;
    }
}
