using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("SFX")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip hardLandClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip hurtClip;
    private AudioSource audioSource;
    
    
    private PlayerMovement playerMove;
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
        playerMove = FindFirstObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove.hasHardDropped && playerMove.isGrounded() && !playerMove.hasPlayed)
        {
           
            PlaySFX(hardLandClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isHardDropping  && !playerMove.hasPlayed)
        {
            PlaySFX(dashClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isDashing && !playerMove.hasPlayed)
        {
            PlaySFX(dashClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isKnockedBack && !playerMove.hasPlayed)
        {
            PlaySFX(hurtClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isJumping && !playerMove.hasPlayed)
        {
            PlaySFX(jumpClip);
            playerMove.hasPlayed = true;
        }
        
        if (!audioSource.isPlaying && !playerMove.hasPlayed)
        {
            audioSource.Stop();
            playerMove.hasPlayed = true;
        }
    }
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
         
        
        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = clip;
        }
        audioSource.Play();
    }
    
}
