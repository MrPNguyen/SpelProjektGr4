using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnEnabled : MonoBehaviour
{
    public UnityEvent customEvent;

    private bool toggle = true;

    private void OnEnable()
    {
        if (toggle)
            customEvent.Invoke();
        
        
        toggle = false;
    }

    private void OnDisable()
    {
        toggle = true;
    }

    public void DebugMessage()
    {
       
    }
}
