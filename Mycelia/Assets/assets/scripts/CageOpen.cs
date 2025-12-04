using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;


public class CageOpen : MonoBehaviour

{
    public UnityEvent onTriggerEnter;

    private bool IsOpen = false;
    private Animator doorAnimator;
    
    
   
    private string tagToActivate = "Player";
    void Start()
    {
        //doorAnimator = GetComponent<Animator>();
        //doorAnimator.SetBool.("Open", false)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void OnTriggerEnter2D(Collider2D other)
    {
       
        if (IsOpen && other.CompareTag(tagToActivate))
        {
            onTriggerEnter.Invoke();
            //doorAnimator.SetBool("isOpen", true);
            Debug.Log("isOpen");
            GetComponent<BoxCollider2D>().isTrigger = false;
        }

        
    }

    public void OpenDoor()
    {
        Debug.Log("isOpen=true");
        IsOpen = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

}
