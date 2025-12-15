using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SvampolinHopp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float Bounce;
    [SerializeField] private Animator animator;
    [SerializeField] private float Delay;


    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (playerMovement.multiplier >= 2)
            {
                //BounceCoroutine(Delay);
                animator.SetBool("isBounce", true);
                playerMovement.hasHardDropped = false;
                playerMovement.velocity.y = Bounce;
            }
        
        }
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        animator.SetBool("isBounce", false);
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
