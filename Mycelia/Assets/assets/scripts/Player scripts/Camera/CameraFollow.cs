using System;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    [SerializeField] private float smoothSpeed = 0.125f;
    public Vector3 offset;
    [SerializeField] private PlayerManager playerManager;
    public bool followPlayer = false;
    public bool win;
    public bool back;
   

    [Header("Camera Preferences")] 
    [SerializeField] private float MinX;
    [SerializeField] private float MaxX;
    [SerializeField] private float MinY;
    [SerializeField] private float MaxY;
    
    [Header("Win")]
    [SerializeField] private float InterpolationTime = 10f;
    public float WhatTime = 0;
    public Vector3 EndPosition;

    void Start()
    {
        followPlayer = true;
    }
    private void FixedUpdate()
    {
        if (playerManager.currentHealth > 0 && followPlayer)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, MinX, MaxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, MinY, MaxY);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        if (win)
        {
            Vector3 StartPosition = playerManager.transform.position + offset;
            
                float interpolation = WhatTime/InterpolationTime;
                //interpolation värdet ska vara mellan 0-1 och beroende på vart emellan ska objektet befinna sig någonstans däremellan.
                //0 returnerar start, 1 returnerar slutet
            
                transform.position = Vector3.Lerp(StartPosition, playerManager.newCameraPosition, interpolation);
                WhatTime += Time.deltaTime;
        }
        
        if (back)
        {
             EndPosition = playerManager.transform.position + offset;
            float interpolation = WhatTime/InterpolationTime;
            //interpolation värdet ska vara mellan 0-1 och beroende på vart emellan ska objektet befinna sig någonstans däremellan.
            //0 returnerar start, 1 returnerar slutet
            transform.position = Vector3.Lerp(playerManager.newCameraPosition,EndPosition , interpolation);
            WhatTime += Time.deltaTime;
        }
    }
}
