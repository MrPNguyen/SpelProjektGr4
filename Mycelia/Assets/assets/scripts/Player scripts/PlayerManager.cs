using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    public Vector3 originalPosition;
    [SerializeField] private GameObject PlayerUI;
    [SerializeField] private GameObject FinalMessage;
    [SerializeField] private DialogueTrigger dialogueGreen;
    [SerializeField] private DialogueTrigger dialogueRed;
    private SpriteRenderer RenderDis;
    
    [Header("Health")]
    private int maxHealth = 3; 
    public int currentHealth;
    [SerializeField] private Image Heart1;
    [SerializeField] private Image Heart2;
    [SerializeField] private Image Heart3;
    private bool Invincible;
        private float time;
        
    [Header("Death")]
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text GameOverPrologText;
    [SerializeField] private List<string> DeathText;
    [SerializeField] private float HitRecoil = 20;
    [SerializeField] private GameObject enemy;
    private Coroutine knockbackRoutine;
    [SerializeField] private Canvas GameOverCanvas;
    
    [Header("Extra Life")]
    [SerializeField] private GameObject extraLife;
    
    
    [Header("Score")]
    public int SavedKantarells = 0;
    public int MaxKantarells;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private Animator portal;
    [SerializeField] private CameraFollow cameraFollow;
    public Vector3 originalCameraPosition;
    public Vector3 newCameraPosition;
    
    [Header("Winning")]
    private Coroutine UnlockWinRoutine;
    [SerializeField] private GameObject winPortal;
    [SerializeField] private GameObject portalSign;
    public bool IsRed;
  
    
    
    
    void Start()
    {
        GameOverCanvas.GameObject().SetActive(false);
        if(winPortal != null)
        {
            winPortal.SetActive(false);
        }
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        originalPosition = transform.position;
        currentHealth = maxHealth;
        RenderDis = GetComponent<SpriteRenderer>();
        if (DeathText.Count > 0)
        {
            GameOverPrologText.text = DeathText[Random.Range(0, DeathText.Count)];
        }
        else
        {
            GameOverPrologText.text = "The Faye has lost its light...";
        }
        PlayerUI.SetActive(true);
        newCameraPosition = new Vector3(85.2f, 2.73f, -4.37f);
        originalCameraPosition = cameraFollow.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
            PlayerUI.SetActive(false);
            GameOverCanvas.gameObject.SetActive(true);
            
            animator.SetTrigger("isDead");
        }


        if (currentHealth == 2)
        {
            Heart1.enabled = false;
            Heart2.enabled = true;
            Heart3.enabled = true;
        }
        if (currentHealth == 1)
        {
            Heart2.enabled = false;
            Heart3.enabled = true;
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

        if (Invincible)
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                Invincible = false;
                RenderDis.color = Color.white;
            }
        }
        
    }
    public void TakeDamage()
    {
        if (!Invincible)
        {
            currentHealth--;
            time = 1;
            Invincible = true;
            RenderDis.color = Color.grey;

        }
        
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (playerMovement.multiplier >= playerMovement.HardDropPower)
            {
                other.gameObject.SetActive(false);
                Instantiate(extraLife, other.transform.position, other.transform.rotation);
                playerMovement.hasHardDropped = false;
               
            }
            else
            {
                TakeDamage();
                float HitRecoilX = 10f * (playerMovement.isFacingRight ? -1 : 1);
                float HitRecoilY = HitRecoil;
                playerMovement.velocity = new Vector2(HitRecoilX, HitRecoilY);

               KnockBack(0.2f);
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
        playerMovement.isKnockedBack = true;
        playerMovement.hasPlayed = false;
        yield return new WaitForSeconds(duration);
        playerMovement.isKnockedBack = false;
        knockbackRoutine = null;
    }

    private void KnockBack(float duration)
    {
        if (knockbackRoutine != null)
        {
            return;
        }
        knockbackRoutine = StartCoroutine(KnockbackCoroutine(duration));
    }

    public void SaveKantarells()
    {
        SavedKantarells++;
        UpdateUI();
        if (SavedKantarells >= MaxKantarells)
        {
            UnlockWin();
        }
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
        cameraFollow.followPlayer = false;
        playerMovement.horizontalMovement = 0f;
        playerMovement.velocity = Vector2.zero;
        playerMovement.rb.linearVelocity = Vector2.zero;
        cameraFollow.win = true;
        yield return new WaitUntil(() => cameraFollow.transform.position == newCameraPosition);
        cameraFollow.win = false;
        cameraFollow.WhatTime = 0;
        if (winPortal!=null)
        {
            winPortal.SetActive(true);
        }
        portalSign.SetActive(true);
        yield return new WaitForSeconds(2);
        portal.SetTrigger("Winnable");
        yield return new WaitForSeconds(6f);
        portal.SetTrigger("Animate");
        cameraFollow.back = true;
        yield return new WaitUntil(() => cameraFollow.transform.position == cameraFollow.EndPosition);
        cameraFollow.back = false;
        cameraFollow.followPlayer = true;
        FinalMessage.SetActive(true);
        playerMovement.horizontalMovement = 0f;
        playerMovement.velocity = Vector2.zero;
        playerMovement.rb.linearVelocity = Vector2.zero;
        if (winPortal != null)
        {
            if (IsRed)
            {
                dialogueRed.TriggerDialogue();
            }

            if (!IsRed)
            {
                dialogueGreen.TriggerDialogue();
            }
        }

        //TODO: lös så att båda cages kan ha TriggerDialogue.
         playerMovement.canMove = true;
    }

    public void SetRed(bool Red)
    {
        IsRed = Red;
    }

    private void UnlockWin()
    {
        if (UnlockWinRoutine != null) return;
        
        UnlockWinRoutine = StartCoroutine(WinUnlock());
        
    }
}
