using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class CageOpen : MonoBehaviour

{
    //public UnityEvent onTriggerEnter;

    private bool IsOpen = false;
    private bool isInRange = false;
    [SerializeField] private Sprite openDoor;
    private SpriteRenderer Sr;
    [SerializeField] private PlayerManager player;
    
    
   
    private string tagToActivate = "Player";
    void Start()
    {
        Sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*void OnTriggerEnter2D(Collider2D other)
    {
        if (IsOpen && other.CompareTag(tagToActivate))
        {  onTriggerEnter.Invoke();
            Sr.sprite = openDoor;
            Debug.Log("isOpen");
            GetComponent<BoxCollider2D>().isTrigger = false;
            
        }
    }*/

    void OnTriggerEnter2D(Collider2D other)
    {
        isInRange = true;
    }
    public void OpenCage(InputAction.CallbackContext context)
    {
        if (isInRange)
        {
            player.SaveKantarells();
            Sr.sprite = openDoor;
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
