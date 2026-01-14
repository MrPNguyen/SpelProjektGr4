using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSFXManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("SFX")]
    private AudioSource audioSource;
    [SerializeField] private AudioSource OneShotSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip hardLandClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip hurtClip;
    
    [Header("Walking")]
    [SerializeField] private List<AudioClip> walkingClips;
    private PlayerMovement playerMove;
    private float walkingTimer;
    [SerializeField] private float walkingInterval = 0.10f;
    [SerializeField] private float runningMultiplier = 0.6f;
    private Coroutine footstepRoutine;
    private bool coroutineStart = false;

    private bool wasDashing;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        playerMove = FindFirstObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        PlaySounds();
    }
    private void PlaySFX(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = clip;
        }
        audioSource.Play();
    }

    private IEnumerator FootstepCoroutine()
    {
        coroutineStart = true;
        
            float interval = walkingInterval;
            if (playerMove.isRunning)
            {
                interval *= runningMultiplier;
            }
            walkingTimer -= Time.deltaTime;
            if (walkingTimer <= 0)
            {
                if (walkingClips != null && walkingClips.Count > 0 && !OneShotSource.isPlaying)
                {
                    PlayOneShot(walkingClips[Random.Range(0, walkingClips.Count)]);
                }
            }
            yield return new WaitForSeconds(interval + 1f);
        coroutineStart = false;
    }

    private void PlayOneShot(AudioClip clip)
    {
        OneShotSource.PlayOneShot(clip);
       
    }

    private void PlaySounds()
    {
        
        if (playerMove.isWalking )
        {
            if (walkingClips != null && walkingClips.Count > 0 && !OneShotSource.isPlaying)
            {
                OneShotSource.PlayOneShot(walkingClips[Random.Range(0, walkingClips.Count)]);
            }
        }
        
        if (playerMove.Jumped)
        {
            OneShotSource.PlayOneShot(jumpClip);
            
            playerMove.hasPlayed = true; 
            playerMove.Jumped = false;
            return;
        }
        
        if (playerMove.hasPlayed) return;
        
        if (playerMove.hasHardDropped && playerMove.IsGrounded)
        {
            PlaySFX(hardLandClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isDashing && !wasDashing)
        {
            PlaySFX(dashClip);
            playerMove.hasPlayed = true;
        }

        if (playerMove.isKnockedBack)
        {
            PlaySFX(hurtClip);
            playerMove.hasPlayed = true;
        }

       
        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
            playerMove.hasPlayed = true;
        }
        
        wasDashing = playerMove.isDashing;
    }
}
