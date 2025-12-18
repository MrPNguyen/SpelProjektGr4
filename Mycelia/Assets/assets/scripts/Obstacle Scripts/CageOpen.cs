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
            Debug.Log("Sr: " + Sr);
            Debug.Log("openDoor: " + openDoor);
            Sr.sprite = openDoor;
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
