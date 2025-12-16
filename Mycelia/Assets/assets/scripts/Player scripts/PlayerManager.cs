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
    [SerializeField] private GameObject PlayerUI;
    
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
    
    [Header("Score")]
    private int SavedKantarells = 0;
    [SerializeField] private int MaxKantarells;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private Animator portal;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CameraFollow cameraFollow;
    private Vector2 originalCameraOffset;
    private Vector2 newCameraOffset;
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
        PlayerUI.SetActive(true);
        newCameraOffset = new Vector3(85.2f, 2.73f, -4.37f);
        originalCameraOffset = cameraFollow.offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        { 
            Destroy(this.gameObject);
            PlayerUI.SetActive(false);
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

        UpdateUI();
        if (SavedKantarells >= MaxKantarells)
        {
            UnlockWin();
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
            if (playerMovement.multiplier >= playerMovement.HardDropPower)
            {
                Debug.Log($" player collide with when harddropping: {other.name}");
                other.gameObject.SetActive(false);
                Instantiate(extraLife, other.transform.position, other.transform.rotation);
            }
            else
            {
                TakeDamage();
                float HitRecoilX = 10f * (playerMovement.isFacingRight ? -1 : 1);
                float HitRecoilY = HitRecoil;
                playerMovement.velocity = new Vector2(HitRecoilX, HitRecoilY);

                StartCoroutine(KnockbackCoroutine(0.2f));
            }
        }

        if (other.tag == "ExtraLife")
        {
            currentHealth++;
            Destroy(other.gameObject);
        }
    }
    
    private IEnumerator KnockbackCoroutine(float duration)
    {
        player.color = Color.red;
        playerMovement.isKnockedBack = true;
        playerMovement.hasPlayed = false;
        yield return new WaitForSeconds(duration);
        playerMovement.isKnockedBack = false;
        player.color = Color.white;
    }

    public void SaveKantarells()
    {
        SavedKantarells++;
    }
    public void SetUI()
    {
        ScoreText.text = $"{SavedKantarells} / {MaxKantarells}";
    }
    public void UpdateUI()
    {
        ScoreText.text = $"{SavedKantarells} / {MaxKantarells}";
    }

    private IEnumerator WinUnlock()
    {
        playerMovement.canMove = false;
        playerMovement.horizontalMovement = 0f;
        playerMovement.velocity = Vector2.zero;
        playerMovement.rb.linearVelocity = Vector2.zero;
        
        cameraFollow.offset = newCameraOffset;
        yield return new WaitForSeconds(2);
        portal.SetTrigger("Winnable");
        yield return new WaitForSeconds(2);
        cameraFollow.offset = originalCameraOffset;
        
        playerMovement.horizontalMovement = 0f;
        playerMovement.velocity = Vector2.zero;
        playerMovement.rb.linearVelocity = Vector2.zero;
        playerMovement.canMove = true;
        playerMovement.canMove = true;
    }

    private void UnlockWin()
    {
        StartCoroutine(WinUnlock());
    }
}
