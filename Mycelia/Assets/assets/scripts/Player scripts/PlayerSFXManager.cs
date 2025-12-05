using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Walking")]
    [SerializeField] private List<AudioClip> walkingClips;
    private PlayerMovement playerMove;
    private float walkingTimer;
    [SerializeField] private float walkingInterval = 0.10f;
    [SerializeField] private float runningMultiplier = 0.6f;
    [SerializeField] private AudioClip GruntClip;
    private Coroutine footstepRoutine;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
        playerMove = FindFirstObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove.hasHardDropped && playerMove.IsGrounded && !playerMove.hasPlayed)
        {
            PlaySFX(hardLandClip);
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
            PlaySFX(GruntClip);
            playerMove.hasPlayed = true;
        }
        
        if (playerMove.IsGrounded && playerMove.horizontalMovement != 0 && !playerMove.isDashing)
        {
            if (footstepRoutine == null)
            {
                footstepRoutine = StartCoroutine(FootsteopCoroutine());
            }
        }
        else
        {
            if (footstepRoutine != null)
            {
                StopCoroutine(footstepRoutine);
                footstepRoutine = null;
            }
        }
        
        if (!audioSource.isPlaying && !playerMove.hasPlayed)
        {
            audioSource.Stop();
            playerMove.hasPlayed = true;
        }
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

    private IEnumerator FootsteopCoroutine()
    {
        while (true)
        {
            float interval = walkingInterval;
            if (playerMove.isRunning)
            {
                interval *= runningMultiplier;
            }
            walkingTimer -= Time.deltaTime;
            if (walkingTimer <= 0)
            {
                if (walkingClips != null && walkingClips.Count > 0)
                {
                    PlayOneShot(walkingClips[Random.Range(0, walkingClips.Count)]);
                }
            }
            yield return new WaitForSeconds(interval + 4f);
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
