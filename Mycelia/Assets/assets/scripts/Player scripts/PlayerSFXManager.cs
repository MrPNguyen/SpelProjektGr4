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
    [SerializeField] private AudioSource OneShotSource;
    
    [Header("Walking")]
    [SerializeField] private List<AudioClip> walkingClips;
    private PlayerMovement playerMove;
    private float walkingTimer;
    [SerializeField] private float walkingInterval = 0.10f;
    [SerializeField] private float runningMultiplier = 0.6f;
    [SerializeField] private AudioClip GruntClip;
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
        Debug.Log($"PlayerMove Grounded: {playerMove.IsGrounded}");
        Debug.Log($"PlayerMove Moving: {playerMove.horizontalMovement}");
        Debug.Log($"PlayerMove Dashing: {playerMove.isDashing}");
        Debug.Log($"Coroutine start: {coroutineStart}");
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
        Debug.Log("Start footstep Coroutine");
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
        if (OneShotSource.clip != clip || !OneShotSource.isPlaying)
        {
            OneShotSource.Stop();
            OneShotSource.clip = clip;
        }
        OneShotSource.Play();
    }

    private void PlaySounds()
    {
        if (playerMove.hasPlayed) return;
        
        if (playerMove.IsGrounded  && playerMove.horizontalMovement !=0  && !playerMove.isDashing && !coroutineStart)
        {
            Debug.Log("Start footstep Coroutine");
            StartCoroutine(FootstepCoroutine());
        }
        else
        {
            StopCoroutine(FootstepCoroutine());
            coroutineStart = false;
            footstepRoutine = null;
            Debug.Log($"Stop Coroutine. footstepRoutine: {footstepRoutine}");
            
        }
        
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

        if (playerMove.Jumped == true)
        {
            OneShotSource.PlayOneShot(jumpClip);
            OneShotSource.PlayOneShot(GruntClip);
            playerMove.hasPlayed = true; playerMove.Jumped = false;
        }
       
        
        
        if (!audioSource.isPlaying)
        {
            audioSource.Stop();
            playerMove.hasPlayed = true;
        }
        
        wasDashing = playerMove.isDashing;
    }
}
