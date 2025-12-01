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
        if (playerMove.hasHardDropped && playerMove.isGrounded())
        {
            playerMove.hasHardDropped = false;
            PlaySFX(hardLandClip);
        }

        if (playerMove.isHardDropping)
        {
            PlaySFX(dashClip);
        }

        if (playerMove.isDashing)
        {
            PlaySFX(dashClip);
        }

        if (playerMove.isKnockedBack)
        {
            PlaySFX(hurtClip);
        }

        if (playerMove.isJumping)
        {
            PlaySFX(jumpClip);
        }
        
        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
}
