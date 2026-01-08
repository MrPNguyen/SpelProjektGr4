using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class CageOpen : MonoBehaviour

{
    [Header("References")]
    public UnityEvent onTriggerEnter;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private Animator KantarellAnimator;
    [SerializeField] private SpriteRenderer Sr;
    [SerializeField] private SpriteRenderer KantaRender;
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private PlayerManager playerManager;
    
    private bool IsOpen = false;
    private bool isInRange = false;
    
    private string tagToActivate = "Player";
    private bool isTriggered = false;
    void Start()
    {
        dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;
        if (IsOpen && other.CompareTag(tagToActivate))
        {  
            isTriggered = true;
            onTriggerEnter.Invoke();
            KantarellAnimator.SetBool("sad", false);
            Sr.sprite = openDoor;
            KantaRender.sortingOrder = 3;
            if (playerManager.SavedKantarells < playerManager.MaxKantarells)
            {
                dialogueTrigger.TriggerDialogue();
            }
        }
    }

    public void OpenDoor()
    {
        IsOpen = true;
    }

}
