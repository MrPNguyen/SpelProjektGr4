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
    [SerializeField] private Sprite HappySprite;
    private SpriteRenderer Sr;
    private BoxCollider2D bx;
    private DialogueTrigger dialogueTrigger;
    [SerializeField] private PlayerManager playerManager;
    
    
   
    private string tagToActivate = "Player";
    void Start()
    {
        Sr = gameObject.GetComponent<SpriteRenderer>();
        bx = gameObject.GetComponent<BoxCollider2D>();
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
            onTriggerEnter.Invoke();
            Sr.sprite = openDoor;
            //Debug.Log("isOpen");
            bx.enabled = false;
            if (playerManager.SavedKantarells < playerManager.MaxKantarells)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }

    public void OpenDoor()
    {
        //Debug.Log("isOpen=true");
        IsOpen = true;
        bx.enabled = true;
    }

}
