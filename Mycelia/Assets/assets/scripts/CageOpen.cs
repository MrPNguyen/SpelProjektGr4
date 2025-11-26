using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CageOpen : MonoBehaviour
{
    public bool IsOpen;
    private Animator doorAnimator;
    private Collider2D other;
    void Start()
    {
        //doorAnimator = GetComponent<Animator>();
        //doorAnimator.SetBool.("Open", false)
        other = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOpen && other.gameObject.tag == "Player")
        {
            //doorAnimator.SetBool("isOpen", true);
            Console.WriteLine("isOpen");
        }
    }

    public void OpenDoor()
    {
        IsOpen = true;
    }

}
