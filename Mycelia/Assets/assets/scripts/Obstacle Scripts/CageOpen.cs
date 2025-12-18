using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class CageOpen : MonoBehaviour

{
    public UnityEvent onTriggerEnter;

    private bool IsOpen = false;
    private bool isInRange = false;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private Animator KantarellAnimator;
    private SpriteRenderer Sr;
    [SerializeField] private BoxCollider2D bx;
    private DialogueTrigger dialogueTrigger;
    [SerializeField] private PlayerManager playerManager;
    
    
   
    private string tagToActivate = "Player";
    void Start()
    {
        Sr = gameObject.GetComponent<SpriteRenderer>();
     
        dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOpen && other.CompareTag(tagToActivate))
        {  
            
            KantarellAnimator.SetBool("sad", false);
            //Sr.sprite = openDoor;
            //Debug.Log("isOpen");
            bx.isTrigger = false;
            Debug.Log(bx.isTrigger);
            /*if (playerManager.SavedKantarells < playerManager.MaxKantarells)
            {
                dialogueTrigger.TriggerDialogue();
            }*/
            onTriggerEnter.Invoke();
        }
    }

    public void OpenDoor()
    {
        //Debug.Log("isOpen=true");
        IsOpen = true;
        bx.enabled = true;
    }

}
