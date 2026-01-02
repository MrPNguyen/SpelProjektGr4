using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SvampolinHopp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;
    [SerializeField] private Animator animator;
    [SerializeField] private float Delay;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip BigBounce;

    void Start()
    {
        audio.clip = BigBounce;
    }

    void Update()
    {
        if (!audio.isPlaying)
        {
            audio.Stop();
        }
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= 3 && Bounce > 0)
            {
                //BounceCoroutine(Delay);
                //animator.SetBool("isBounce", true);
                animator.SetTrigger("Bounce");
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = Bounce;
                audio.Play();
            }
            /*else if (playerMovement.velocity.y <= -3f)
            {
                
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = 0.4f * MathF.Abs(playerMovement.velocity.y);
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                playerMovement.velocity.y = 6.5f;
            }*/
            
        
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        //animator.SetBool("isBounce", false);
    }

    /*private IEnumerator BounceCoroutine(float Delay)
    {
        Debug.Log("Bouncecoroutine");
        animator.SetBool("isBounce", true);
        playerMovement.hasHardDropped = false;
        playerMovement.velocity.y = Bounce;
        yield return new WaitForSeconds(Delay);
        animator.SetBool("isBounce", false);
    }*/
}
